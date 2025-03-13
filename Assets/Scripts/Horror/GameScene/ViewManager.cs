using UnityEngine;
using System.Collections.Generic;

public class ViewManager : MonoBehaviour
{
    private Dictionary<int, Vector3> viewPositions = new Dictionary<int, Vector3>();
    private CameraController cameraController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraController = FindFirstObjectByType<CameraController>();
        Invoke(nameof(InitializeManager), 0.1f);
    }

    private void InitializeManager()
    {
        ViewData[] views = GetComponentsInChildren<ViewData>();
        foreach (ViewData view in views)
        {
            int viewID = view.viewID;
            Vector3 viewPosition = view.viewPosition;

            viewPositions[viewID] = viewPosition;
        }

        foreach (var kvp in viewPositions)
        {
            Debug.Log("View ID: " + kvp.Key + " | Position: " + kvp.Value);
        }

        cameraController.cameraControllerStartup();
    }

    public Vector3 GetViewPosition(int viewID)
    {
        if (viewPositions.TryGetValue(viewID, out Vector3 position))
            return position;

        Debug.LogWarning("View ID " + viewID + " not found!");
        return Vector3.zero;
    }

    public List<int> GetViewIDs()
    {
        return new List<int>(viewPositions.Keys);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
