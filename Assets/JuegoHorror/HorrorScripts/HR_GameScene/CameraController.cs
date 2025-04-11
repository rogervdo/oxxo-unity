using UnityEngine;
using System.Collections.Generic; 

public class CameraController : MonoBehaviour
{
    // Referencia al objeto de la cámara principal.
    public Camera mainCamera;
    // Referencia al ViewManager.
    private ViewManager viewManager;
    // Índice actual en la lista viewIDs para el ciclo.
    private int currentIndex = 0;
    // Lista de IDs de vista disponibles proporcionada por ViewManager.
    private List<int> viewIDs;

    // Método de inicialización llamado por ViewManager después de que esté listo.
    public void cameraControllerStartup()
    {
        viewManager = FindFirstObjectByType<ViewManager>();
        if (viewManager == null) {

             return;
        }
        // Obtiene la lista de IDs de vista del manager.
        viewIDs = viewManager.GetViewIDs();
        if (viewIDs == null || viewIDs.Count == 0) {

        } 
    }


    void Update()
    {
        // No hacer nada si los IDs de vista aún no se han cargado.
        if (viewIDs == null || viewIDs.Count == 0)
        {
            return;
        }

        // Comprueba las teclas numéricas del 1 al 5.
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            TryMoveToViewAtIndex(0); // Intenta mover a la vista en el índice 0 de la lista.
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            TryMoveToViewAtIndex(1); // Intenta mover a la vista en el índice 1 de la lista.
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            TryMoveToViewAtIndex(2); // Intenta mover a la vista en el índice 2 de la lista.
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            TryMoveToViewAtIndex(3); // Intenta mover a la vista en el índice 3 de la lista.
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            TryMoveToViewAtIndex(4); // Intenta mover a la vista en el índice 4 de la lista.
        }
    }

    // Método auxiliar para mover la cámara a la vista correspondiente a un índice de la lista.
    private void TryMoveToViewAtIndex(int index)
    {
        // Comprueba si el índice solicitado es válido dentro de los límites de la lista.
        if (index >= 0 && index < viewIDs.Count)
        {
            // Obtiene el ID de vista real almacenado en ese índice.
            int targetViewID = viewIDs[index];
            // Llama a la función existente para mover la cámara usando el ID.
            MoveCameraToView(targetViewID);
        }
    }

    // Mueve la cámara cíclicamente a la siguiente vista en la lista.
    public void moveCamera()
    {
        // No hacer nada si no hay IDs de vista disponibles.
        if (viewIDs == null || viewIDs.Count == 0) {

            return;
        }

        // Calcula el siguiente índice, volviendo al principio usando módulo.
        currentIndex = (currentIndex + 1) % viewIDs.Count;
        int nextID = viewIDs[currentIndex];

        // Usa la función de movimiento específico por consistencia.
        MoveCameraToView(nextID);

    }

    // Mueve la cámara directamente a la posición asociada con un ID de vista específico.
    public void MoveCameraToView(int viewID)
    {
        // Asegura que ViewManager y la referencia a la cámara sean válidos.
        if (viewManager == null || mainCamera == null) {
             // Debug.LogError("MoveCameraToView: ViewManager o MainCamera no están listos/asignados.", this); // ELIMINADO
             return;
        }

        // Obtiene la posición objetivo desde ViewManager usando el ID.
        Vector3 newPos = viewManager.GetViewPosition(viewID);

        // Comprueba si ViewManager devolvió una posición inválida (p.ej., Vector3.zero si no se encontró el ID).
        if (newPos == Vector3.zero && !viewManager.GetViewIDs().Contains(viewID)) {

             return; 
        }

        // Aplica la nueva posición, manteniendo Z constante.
        // Usa la X e Y de la posición de la vista.
        mainCamera.transform.position = new Vector3(newPos.x, newPos.y, -10f);


        int foundIndex = viewIDs.IndexOf(viewID);
        if(foundIndex != -1) {
            currentIndex = foundIndex;
        } else {

             currentIndex = 0;

        }


    }
} 