using UnityEngine;
using UnityEngine.EventSystems; // Necesario para comprobar clics sobre la UI

public class ClickEffectSpawner : MonoBehaviour
{
    // --- Variables Públicas (Asignar en Inspector) ---
    public GameObject clickEffectPrefab;    // Prefab del efecto visual al hacer clic normal.
    public GameObject heartEffectPrefab;    // Prefab del efecto visual (corazón) al hacer clic en anomalía.
    public HealthController healthController; // Referencia al script que controla la vida/tiempo.
    public float healthToAddOnClick = 15.0f; // Vida/tiempo a añadir al hacer clic en anomalía.

    // --- Variables Privadas ---
    private Camera mainCamera; // Referencia a la cámara principal (cacheada).

    // Se llama una vez al inicio.
    void Start()
    {
        // Guarda la referencia a la cámara principal para eficiencia.
        mainCamera = Camera.main;
    }

    // Se llama cada fotograma.
    void Update()
    {
        // Comprueba si se presionó el botón izquierdo del ratón.
        if (Input.GetMouseButtonDown(0))
        {
            // 1. Ignora el clic si está sobre un elemento de la UI.
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // 2. Obtiene la posición del clic en el mundo del juego.
            Vector3 worldPosition = GetClickWorldPosition();
            if (worldPosition == Vector3.positiveInfinity) // Si hubo error al obtener la posición
            {
                return;
            }

            // 3. Lanza un rayo en la posición del clic para ver qué objeto 2D hay.
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            bool anomalyClicked = false; // Indica si se hizo clic en una anomalía este fotograma.

            // 4. Comprueba si el rayo golpeó algún objeto con Collider2D.
            if (hit.collider != null)
            {
                // 5. Comprueba si el objeto golpeado tiene la etiqueta "Anomaly".
                if (hit.collider.CompareTag("Anomaly"))
                {
                    anomalyClicked = true; // Marca que se clickeó una anomalía.

                    // --- Lógica al Cliquear Anomalía ---
                    // a. Genera el efecto de corazón en la posición del clic.
                    SpawnHeartEffect(worldPosition);
                    // b. Añade vida/tiempo llamando al HealthController.
                    AddHealth(healthToAddOnClick);
                    // c. Destruye el objeto de la anomalía.
                    Destroy(hit.collider.gameObject);
                    // --- Fin Lógica Anomalía ---
                }
            }

            // 6. Genera el efecto de clic normal SOLAMENTE si NO se hizo clic en una anomalía.
            if (!anomalyClicked)
            {
                SpawnClickEffect(worldPosition);
            }
        }
    }

    // Convierte la posición del ratón en pantalla a una posición en el mundo 2D (plano Z=0).
    Vector3 GetClickWorldPosition()
    {
        if (mainCamera == null) return Vector3.positiveInfinity; // Retorna error si no hay cámara

        Vector3 screenPosition = Input.mousePosition;
        screenPosition.z = 0f - mainCamera.transform.position.z; // Ajusta Z para la conversión
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f; // Asegura que esté en el plano Z=0
        return worldPosition;
    }

    // Crea una instancia del prefab de efecto de clic normal en la posición dada.
    void SpawnClickEffect(Vector3 position)
    {
        if (clickEffectPrefab != null)
        {
            Instantiate(clickEffectPrefab, position, Quaternion.identity);
        }
    }

    // Crea una instancia del prefab de efecto de corazón en la posición dada.
    void SpawnHeartEffect(Vector3 position)
    {
        if (heartEffectPrefab != null)
        {
            Instantiate(heartEffectPrefab, position, Quaternion.identity);
        }
    }

    // Llama al método AddHealth del HealthController asignado.
    void AddHealth(float amount)
    {
        if (healthController != null)
        {
            healthController.AddHealth(amount);
        }
    }
}