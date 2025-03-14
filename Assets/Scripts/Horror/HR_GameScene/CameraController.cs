using UnityEngine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{

    public Camera mainCamera; // Assign the camera in the Inspector
    private ViewManager viewManager;
    private int currentIndex = 0;
    private List<int> viewIDs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void cameraControllerStartup()
    {
        viewManager = FindFirstObjectByType<ViewManager>();
        if (viewManager == null)
        {
            Debug.LogError("ViewManager not found in the scene!");
            return;
        }

        viewIDs = viewManager.GetViewIDs();

        if (viewIDs.Count == 0)
        {
            Debug.LogWarning("No views found in ImageManager!");
        }
    }

    public void moveCamera()
    {
        Debug.Log("Cyclecamera called");
        if (viewIDs.Count == 0) return;

        currentIndex = (currentIndex + 1) % viewIDs.Count;
        int nextID = viewIDs[currentIndex];

        Vector3 newPos = viewManager.GetViewPosition(nextID);
        mainCamera.transform.position = new Vector3(newPos.x, newPos.y, -10f);
        Debug.Log("Camera moved instantly to View ID: " + nextID + " Position: " + newPos);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
