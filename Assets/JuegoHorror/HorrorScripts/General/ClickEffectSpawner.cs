using UnityEngine;
using UnityEngine.EventSystems; // Required for checking UI clicks

public class ClickEffectSpawner : MonoBehaviour
{
    [Header("Effects")]
    [Tooltip("The visual effect prefab to spawn on any non-UI click.")]
    public GameObject clickEffectPrefab;

    [Tooltip("The visual effect prefab (e.g., heart animation) to spawn when an anomaly is clicked.")]
    public GameObject heartEffectPrefab;

    // --- ADD THESE LINES ---
    [Header("Game Logic References")]
    [Tooltip("Reference to the script managing the game timer/health.")]
    public HealthController healthController; // Assign this in the Inspector!

    [Tooltip("Amount of time/health to add when an anomaly is successfully clicked.")]
    public float healthToAddOnClick = 15.0f; // Renamed from timeToAdd for clarity, set your desired value
    // --- END OF ADDED LINES ---


    // Private reference to the main camera, cached for efficiency
    private Camera mainCamera;

    /// <summary>
    /// Called once when the script instance is first enabled.
    /// Caches references and performs initial checks.
    /// </summary>
    void Start()
    {
        // Cache the main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError($"{nameof(ClickEffectSpawner)}: Main Camera not found! Make sure your camera is tagged 'MainCamera'.", this);
        }

        // --- ADD THIS CHECK ---
        // Check if the HealthController reference was assigned in the Inspector
        if (healthController == null)
        {
            Debug.LogError($"{nameof(ClickEffectSpawner)}: Health Controller reference not set in the Inspector! Please assign the GameObject containing HealthController.", this);
            // As a fallback, you could try to find it, but Inspector assignment is preferred:
            // healthController = FindObjectOfType<HealthController>();
            // if (healthController == null) Debug.LogError("ClickEffectSpawner: Could not find HealthController in scene!");
        }
        // --- END OF ADDED CHECK ---


        // Check if effect prefabs are assigned
        if (clickEffectPrefab == null)
        {
             Debug.LogWarning($"{nameof(ClickEffectSpawner)}: Click Effect Prefab is not assigned in the Inspector.", this);
        }
        if (heartEffectPrefab == null)
        {
             Debug.LogWarning($"{nameof(ClickEffectSpawner)}: Heart Effect Prefab is not assigned in the Inspector.", this);
        }
    }

    /// <summary>
    /// Called every frame. Checks for player input (left mouse click).
    /// Handles raycasting, anomaly detection, effect spawning, and object destruction.
    /// </summary>
        void Update()
    {
        // Check if the left mouse button was pressed down this frame
        if (Input.GetMouseButtonDown(0)) // 0 corresponds to the left mouse button
        {
            // --- Step 1: Check if the click is over a UI element ---
            // Prevents clicks on UI (like buttons) from triggering game world actions.
            // Requires an EventSystem in the scene.
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                // Pointer is over a UI element, so do nothing in the game world.
                return;
            }

            // --- Step 2: Get the click position in world coordinates ---
            // This 'worldPosition' is where the mouse cursor actually clicked within the game's 2D space.
            Vector3 worldPosition = GetClickWorldPosition();
            if (worldPosition == Vector3.positiveInfinity) // Check if GetClickWorldPosition indicated an error
            {
                // Error occurred getting world position (likely camera issue), stop processing this click.
                return;
            }

            // --- Step 3: Perform a 2D Raycast ---
            // Cast a ray from the exact click position to see what 2D collider is at that point.
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            bool anomalyClicked = false; // Flag to track if an anomaly was successfully clicked this frame

            // --- Step 4: Check if the Raycast hit something ---
            if (hit.collider != null) // Did the raycast intersect with any Collider2D?
            {
                // --- Step 5: Check if the hit object is tagged as "Anomaly" ---
                if (hit.collider.CompareTag("Anomaly")) // Use CompareTag for efficiency and to avoid typos
                {
                    anomalyClicked = true; // Mark that we successfully clicked an anomaly

                    // Optional: Get the anomaly's own position for logging or other potential future use
                    Vector3 anomalyObjectPosition = hit.transform.position;

                    // Temporarily increase gain significantly for testing
                    // Use 50 instead of healthToAddOnClick during this debugging phase
                    float effectiveHealthToAdd = 50f;

                    // --- Anomaly Clicked Logic ---
                    // DETAILED DEBUG LOG: Print info when an anomaly click is processed
                    Debug.Log($"--- ClickEffectSpawner Frame {Time.frameCount}: Anomaly clicked ({hit.collider.gameObject.name}) at obj pos {anomalyObjectPosition}! ClickPos={worldPosition}. Attempting to call AddHealth({effectiveHealthToAdd}). Current time = {Time.time:F3} ---");

                    // 1. Spawn the Heart Effect AT THE EXACT MOUSE CLICK POSITION
                    SpawnHeartEffect(worldPosition); // Use the calculated mouse world position, not the anomaly's center

                    // 2. Add health/time by calling the AddHealth function in THIS script,
                    //    which then calls the HealthController's version. Use the large test value.
                    AddHealth(effectiveHealthToAdd);

                    // 3. Destroy the anomaly GameObject that was hit
                    Destroy(hit.collider.gameObject);

                    // --- End Anomaly Clicked Logic ---
                }
                else
                {
                    // Optional: Log if something else with a collider was clicked
                    // Debug.Log($"Clicked on non-anomaly object: {hit.collider.gameObject.name} with tag {hit.collider.tag}");
                }
            }

            // --- Step 6: Spawn the regular click visual effect ---
            // Spawn the standard click effect ONLY if an anomaly was NOT clicked during this frame.
            // This prevents the regular click effect from overlapping the heart effect.
            if (!anomalyClicked)
            {
                SpawnClickEffect(worldPosition);
            }
        }
    }

    // --- GetClickWorldPosition, SpawnClickEffect, SpawnHeartEffect functions remain the same ---
    // (Include them from the previous full script version)

    Vector3 GetClickWorldPosition()
    {
        if (mainCamera == null)
        {
            Debug.LogError($"{nameof(ClickEffectSpawner)}: Cannot get world position, Main Camera is missing!");
            return Vector3.positiveInfinity;
        }
        Vector3 screenPosition = Input.mousePosition;
        screenPosition.z = 0f - mainCamera.transform.position.z;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;
        return worldPosition;
    }

    void SpawnClickEffect(Vector3 position)
    {
        if (clickEffectPrefab != null)
        {
            Instantiate(clickEffectPrefab, position, Quaternion.identity);
        }
    }

    void SpawnHeartEffect(Vector3 position)
    {
        if (heartEffectPrefab != null)
        {
            Instantiate(heartEffectPrefab, position, Quaternion.identity);
        }
    }


    // --- ADD THIS FUNCTION ---
    /// <summary>
    /// Calls the HealthController (if available) to add health/time.
    /// </summary>
    /// <param name="amount">The amount of health/time to add.</param>
    void AddHealth(float amount)
    {
        if (healthController != null)
        {
            // Call the public AddHealth method on the referenced HealthController script
            healthController.AddHealth(amount);
            // Debug.Log($"Called HealthController.AddHealth({amount})"); // Optional log
        }
        else
        {
            // Error already logged in Start
            Debug.LogError($"{nameof(ClickEffectSpawner)}: Cannot add health, Health Controller reference is missing!", this);
        }
    }
    // --- END OF ADDED FUNCTION ---
}