using UnityEngine;
using UnityEngine.EventSystems; 

public class ClickEffectSpawner : MonoBehaviour
{
    public GameObject clickEffectPrefab; 
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

        if (Input.GetMouseButtonDown(0)) 
        {

            if (EventSystem.current.IsPointerOverGameObject())
            {

                return;
            }


            SpawnEffect();
        }
    }

    void SpawnEffect()
    {
        if (clickEffectPrefab == null || mainCamera == null)
        {
            Debug.LogWarning("ClickEffectSpawner: Prefab or Camera not set.");
            return;
        }

        Vector3 screenPosition = Input.mousePosition;

        screenPosition.z = 0 - mainCamera.transform.position.z; 
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);


        worldPosition.z = 0; 

        Instantiate(clickEffectPrefab, worldPosition, Quaternion.identity); 
    }
}