using UnityEngine;
using UnityEngine.EventSystems; // Necesario para comprobar clics sobre la UI

public class ClickEffectSpawner : MonoBehaviour
{
    // --- Variables Públicas (Asignar en Inspector) ---
    [Header("Effects")] // Puedes quitar [Header]
    public GameObject clickEffectPrefab;    // Efecto de clic normal
    public GameObject heartEffectPrefab;    // Efecto al destruir anomalía

    [Header("Game Logic References")] // Puedes quitar [Header]
    public HealthController healthController; // Referencia al controlador de vida
    public float healthToAddOnClick = 15.0f; // Vida a añadir por clic

    [Header("Spawner Reference")] // Puedes quitar [Header]
    public AnomalySpawner anomalySpawner; // Referencia al gestor de anomalías

    // --- Variables Privadas ---
    private Camera mainCamera; // Referencia a la cámara principal

    // Se llama una vez al inicio.
    void Start()
    {
        mainCamera = Camera.main; // Guarda referencia a la cámara

        // Comprueba si las referencias importantes fueron asignadas en el Inspector.
        if (healthController == null) {
             Debug.LogError("Referencia 'healthController' no asignada en ClickEffectSpawner!", this);
        }
        if (anomalySpawner == null) {
             Debug.LogError("Referencia 'anomalySpawner' no asignada en ClickEffectSpawner!", this);
        }
        if (clickEffectPrefab == null) {
             Debug.LogWarning("Referencia 'clickEffectPrefab' no asignada.", this);
        }
         if (heartEffectPrefab == null) {
             Debug.LogWarning("Referencia 'heartEffectPrefab' no asignada.", this);
        }
    }

    // Se llama cada fotograma.
    void Update()
    {
        // Comprueba clic izquierdo.
        if (Input.GetMouseButtonDown(0))
        {
            // Ignora si es sobre UI.
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) {
                return;
            }

            // Obtiene posición en el mundo.
            Vector3 worldPosition = GetClickWorldPosition();
            if (worldPosition == Vector3.positiveInfinity) { return; } // Sale si hay error

            // Lanza rayo.
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
            bool anomalyClicked = false;

            // Si golpea algo...
            if (hit.collider != null)
            {
                // Y ese algo es una anomalía...
                if (hit.collider.CompareTag("Anomaly"))
                {
                    anomalyClicked = true;
                    GameObject anomalyToDestroy = hit.collider.gameObject; // Guarda referencia antes de destruir

                    // --- Lógica al Cliquear Anomalía ---
                    SpawnHeartEffect(worldPosition); // Efecto corazón
                    // AddHealth(healthToAddOnClick);    // Añade vida

                    // Notifica al Spawner que esta anomalía será destruida.
                    if (anomalySpawner != null) {
                        anomalySpawner.NotifyAnomalyDestroyed(anomalyToDestroy);
                    }

                    Destroy(anomalyToDestroy); // Destruye la anomalía
                    // --- Fin Lógica Anomalía ---
                }
            }

            // Si NO se hizo clic en una anomalía, muestra efecto normal.
            if (!anomalyClicked) {
                SpawnClickEffect(worldPosition);
            }
        }
    }

    // --- Métodos Auxiliares ---
    Vector3 GetClickWorldPosition() {
        if (mainCamera == null) {
             Debug.LogError("ClickEffectSpawner: Cámara principal perdida.", this);
             return Vector3.positiveInfinity;
        }
        Vector3 screenPosition = Input.mousePosition;
        screenPosition.z = 0f - mainCamera.transform.position.z;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;
        return worldPosition;
    }

    void SpawnClickEffect(Vector3 position) {
        if (clickEffectPrefab != null) {
            Instantiate(clickEffectPrefab, position, Quaternion.identity);
        }
    }

    void SpawnHeartEffect(Vector3 position) {
        if (heartEffectPrefab != null) {
            Instantiate(heartEffectPrefab, position, Quaternion.identity);
        }
    }

    void AddHealth(float amount) {
        if (healthController != null) {
            healthController.AddHealth(amount);
        } else {
             // El error ya se loguea en Start si falta
        }
    }
}