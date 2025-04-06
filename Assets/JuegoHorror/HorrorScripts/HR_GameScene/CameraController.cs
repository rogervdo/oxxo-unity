using UnityEngine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{

    public Camera mainCamera; // Assign the camera in the Inspector
    private ViewManager viewManager;
    private int currentIndex = 0;
    private List<int> viewIDs;

    public void cameraControllerStartup()
    {
        viewManager = FindFirstObjectByType<ViewManager>();
        viewIDs = viewManager.GetViewIDs();
    }

    public void moveCamera()
    {
        Debug.Log("Ciclar camara");
        if (viewIDs.Count == 0) return;

        currentIndex = (currentIndex + 1) % viewIDs.Count;
        int nextID = viewIDs[currentIndex];

        Vector3 newPos = viewManager.GetViewPosition(nextID);
        mainCamera.transform.position = new Vector3(0f, newPos.y, -10f); // Reiniciar x
        Debug.Log("Camera movida a ID" + nextID + " Posicion: " + newPos);
    }

    public void MoveCameraToView(int viewID)
    {
        if (!viewIDs.Contains(viewID))
        {
            Debug.LogWarning("View ID " + viewID + " no existe");
            return;
        }

        Vector3 newPos = viewManager.GetViewPosition(viewID);
        mainCamera.transform.position = new Vector3(newPos.x, newPos.y, -10f);
        Debug.Log("Camera movida a ID" + viewID + " Posicion: " + newPos);
    }
}
