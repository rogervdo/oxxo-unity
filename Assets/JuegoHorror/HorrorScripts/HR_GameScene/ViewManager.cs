using UnityEngine;
using System.Collections.Generic;

public class ViewManager : MonoBehaviour
{

    private Dictionary<int, Vector3> viewPositions = new Dictionary<int, Vector3>();

    private CameraController cameraController;

    void Start()
    {
        // Obtiene la instancia de CameraController.
        cameraController = FindFirstObjectByType<CameraController>();
        // Retrasa ligeramente la inicialización para asegurar que los componentes ViewData estén listos.
        Invoke(nameof(InitializeManager), 0.1f);
    }

    // Encuentra ViewDatas, puebla el diccionario y notifica a CameraController.
    private void InitializeManager()
    {
        // Encuentra todos los objetos hijos con componentes ViewData.
        ViewData[] views = GetComponentsInChildren<ViewData>();
        foreach (ViewData view in views)
        {
            // Almacena la posición usando el ID de ViewData.
            viewPositions[view.viewID] = view.viewPosition;
        }

        if (cameraController != null)
        {
            cameraController.cameraControllerStartup();
        } 
    }

    // Devuelve la posición almacenada para un ID de vista dado.
    public Vector3 GetViewPosition(int viewID)
    {
        if (viewPositions.TryGetValue(viewID, out Vector3 position))
            return position; // Devuelve la posición si se encuentra el ID.

        return Vector3.zero;
    }


    public List<int> GetViewIDs()
    {
        return new List<int>(viewPositions.Keys);
    }


} 