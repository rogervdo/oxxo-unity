using UnityEngine;
using System.Collections.Generic; // Necesario para List

public class CameraController : MonoBehaviour
{
    public Camera mainCamera; // Asigna la cámara en el Inspector
    private ViewManager viewManager;
    private int currentIndex = 0; // Usado por moveCamera (cíclico)
    private List<int> viewIDs; // Lista de IDs de vistas disponibles

    // Se llama DESPUÉS de que ViewManager haya encontrado las vistas.
    public void cameraControllerStartup()
    {
        viewManager = FindFirstObjectByType<ViewManager>();
        if (viewManager == null) {
             Debug.LogError("CameraController no pudo encontrar ViewManager!", this);
             return;
        }
        viewIDs = viewManager.GetViewIDs();
        if (viewIDs == null || viewIDs.Count == 0) {
            Debug.LogWarning("CameraController no recibió IDs de vista desde ViewManager.", this);
        } else {
             // Opcional: Ordenar los IDs si quieres que Tecla 1 siempre vaya al ID más bajo, etc.
             // viewIDs.Sort();
             Debug.Log($"CameraController inicializado con {viewIDs.Count} IDs de vista.");
        }
    }

    // NUEVO: Se llama cada fotograma para comprobar input del teclado.
    void Update()
    {
        // No hacer nada si aún no tenemos los IDs de las vistas.
        if (viewIDs == null || viewIDs.Count == 0)
        {
            return;
        }

        // Comprobar tecla 1 (mapeada al índice 0 de la lista viewIDs)
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            TryMoveToViewAtIndex(0); // Intenta ir a la vista en el índice 0
        }
        // Comprobar tecla 2 (mapeada al índice 1 de la lista viewIDs)
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            TryMoveToViewAtIndex(1); // Intenta ir a la vista en el índice 1
        }
        // Comprobar tecla 3 (mapeada al índice 2 de la lista viewIDs)
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            TryMoveToViewAtIndex(2); // Intenta ir a la vista en el índice 2
        }
        // Comprobar tecla 4 (mapeada al índice 3 de la lista viewIDs)
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            TryMoveToViewAtIndex(3); // Intenta ir a la vista en el índice 3
        }
        // Comprobar tecla 5 (mapeada al índice 4 de la lista viewIDs)
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            TryMoveToViewAtIndex(4); // Intenta ir a la vista en el índice 4
        }
        // Puedes añadir más 'else if' si necesitas soportar más teclas (6, 7, etc.)
    }

    // NUEVO: Método auxiliar para mover la cámara a la vista correspondiente
    // al índice dado en la lista 'viewIDs'.
    private void TryMoveToViewAtIndex(int index)
    {
        // Comprueba si el índice solicitado existe dentro de los límites de la lista.
        if (index >= 0 && index < viewIDs.Count)
        {
            // Obtiene el ID real de la vista guardado en ese índice.
            int targetViewID = viewIDs[index];
            // Llama a la función que ya tenías para mover la cámara.
            MoveCameraToView(targetViewID);
        }
        else
        {
            // Aviso si se pulsa una tecla para la que no hay vista configurada.
             Debug.LogWarning($"Se presionó la tecla {index + 1}, pero no hay una vista configurada para el índice {index} en la lista viewIDs.");
        }
    }

    // Mueve la cámara cíclicamente (función original)
    public void moveCamera()
    {
        // No hacer nada si no hay IDs
        if (viewIDs == null || viewIDs.Count == 0) {
            Debug.LogWarning("moveCamera llamado pero no hay viewIDs disponibles.", this);
            return;
        }

        currentIndex = (currentIndex + 1) % viewIDs.Count;
        int nextID = viewIDs[currentIndex];

        // Usar la función existente para asegurar consistencia
        MoveCameraToView(nextID);
        Debug.Log("Cámara ciclada a ID " + nextID); // Log más específico
    }

    // Mueve la cámara a un ID específico (función original, ahora llamada por Update y moveCamera)
    public void MoveCameraToView(int viewID)
    {
        // Asegurarse de que ViewManager y la cámara están listos
        if (viewManager == null || mainCamera == null) {
             Debug.LogError("MoveCameraToView: ViewManager o MainCamera no están listos/asignados.", this);
             return;
        }
        // No es necesario comprobar viewIDs.Contains aquí, GetViewPosition ya lo hace

        Vector3 newPos = viewManager.GetViewPosition(viewID);

        // Comprueba si GetViewPosition devolvió la posición por defecto (error)
        // (Asumiendo que Vector3.zero es la señal de error de GetViewPosition)
        // O mejor, podrías modificar GetViewPosition para devolver un booleano o lanzar una excepción.
        if (newPos == Vector3.zero && !viewManager.GetViewIDs().Contains(viewID)) { // Comprueba si realmente es porque no existe
             Debug.LogWarning("MoveCameraToView: No se pudo obtener una posición válida para View ID " + viewID);
             return; // No mover si la posición es inválida
        }


        // Aplica la posición. Considera si la posición X debe ser siempre 0 como en moveCamera
        // o si debe usar la X de la vista como parece indicar esta función.
        // Voy a mantener la lógica original de esta función: usar la X de la vista.
        // Si quieres que siempre sea X=0, cambia newPos.x por 0f aquí.
        mainCamera.transform.position = new Vector3(newPos.x, newPos.y, -10f); // Mantiene Z constante

        // Actualiza currentIndex si es necesario para que moveCamera continúe desde aquí
        currentIndex = viewIDs.IndexOf(viewID);
        if(currentIndex == -1) currentIndex = 0; // Resetea si el ID no estaba (raro)

        Debug.Log("Cámara movida directamente a ID " + viewID + " | Posición: " + mainCamera.transform.position);
    }
}