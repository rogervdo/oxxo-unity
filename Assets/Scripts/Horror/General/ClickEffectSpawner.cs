using UnityEngine;
using UnityEngine.EventSystems; // Required for checking UI clicks

public class ClickEffectSpawner : MonoBehaviour
{
    public GameObject clickEffectPrefab; // Assign your effect prefab in the Inspector
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("ClickEffectSpawner: Main Camera not found! Make sure your camera is tagged 'MainCamera'.");
        }
    }

    void Update()
    {
        // Check for left mouse button down
        if (Input.GetMouseButtonDown(0)) // 0 is the left mouse button
        {
            // --- Step 1: Check for UI clicks FIRST ---
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Click is on UI, do nothing related to the game world
                return;
            }

            // --- Step 2: Get click position in the world ---
            Vector3 worldPosition = GetClickWorldPosition();
            if (worldPosition == Vector3.positiveInfinity) // Check if camera is missing
            {
                return; // Exit if we couldn't get a valid position
            }


            // --- Step 3: Raycast to see what was clicked ---
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero); // Raycast at the click point

            bool anomalyClicked = false;
            if (hit.collider != null) // Did the raycast hit something with a Collider2D?
            {
                // --- Step 4: Check if the hit object is an Anomaly ---
                if (hit.collider.CompareTag("Anomaly")) // Use CompareTag for efficiency
                {
                    // It's an anomaly! Destroy it.
                    Destroy(hit.collider.gameObject);
                    anomalyClicked = true;
                    Debug.Log("Anomaly destroyed!"); // Optional: for confirmation
                    // Optional: Add points, play a success sound, etc. here
                }
                // Optional: You could add else if conditions here to check for other tags
                // else if (hit.collider.CompareTag("SomeOtherInteractable")) { ... }
            }

            // --- Step 5: Spawn the visual click effect ---
            // You might want to spawn the effect regardless of hitting an anomaly,
            // or maybe only when NOT hitting an anomaly. Decide based on your game feel.
            // This example spawns it always on a valid (non-UI) click:
            SpawnEffect(worldPosition);

            // Alternative: Only spawn effect if NOT clicking an anomaly
            // if (!anomalyClicked)
            // {
            //     SpawnEffect(worldPosition);
            // }
        }
    }

    Vector3 GetClickWorldPosition()
    {
        if (mainCamera == null) return Vector3.positiveInfinity; // Indicate error

        Vector3 screenPosition = Input.mousePosition;
        // Ensure Z is set correctly for ScreenToWorldPoint in 2D
        screenPosition.z = 0 - mainCamera.transform.position.z;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        // Force Z to 0 (or your desired 2D plane) after conversion
        worldPosition.z = 0;
        return worldPosition;
    }


    // Renamed SpawnEffect to take the position as an argument
    void SpawnEffect(Vector3 position)
    {
        if (clickEffectPrefab == null)
        {
            Debug.LogWarning("ClickEffectSpawner: Click Effect Prefab not set.");
            return;
        }

        Instantiate(clickEffectPrefab, position, Quaternion.identity);
    }
}