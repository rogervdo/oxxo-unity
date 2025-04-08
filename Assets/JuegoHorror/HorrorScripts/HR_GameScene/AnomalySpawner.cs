using UnityEngine;
using System.Collections.Generic; // Necesario para Listas
using System.Linq; // Necesario para .Count(item => item != null)

public class AnomalySpawner : MonoBehaviour
{
    // --- Configuración (Ajustar en Inspector) ---
    [Tooltip("Tiempo mínimo (en segundos) entre intentos de aparición.")]
    public float minSpawnInterval = 3.0f;
    [Tooltip("Tiempo máximo (en segundos) entre intentos de aparición.")]
    public float maxSpawnInterval = 8.0f;
    [Tooltip("Número máximo de anomalías activas permitidas en la escena a la vez.")]
    public int maxActiveAnomalies = 10;

    // --- Internals ---
    // Lista de todas las áreas de spawn disponibles en la escena.
    private List<SpawnArea> spawnAreas = new List<SpawnArea>();
    // Lista para rastrear las anomalías actualmente activas.
    private List<GameObject> activeAnomalies = new List<GameObject>();
    // Temporizador para el próximo intento de spawn.
    private float spawnTimer;

    void Start()
    {
        // Busca TODOS los componentes SpawnArea activos en la escena al inicio.
        spawnAreas.AddRange(FindObjectsOfType<SpawnArea>());

        // Comprueba si se encontró alguna área.
        if (spawnAreas.Count == 0)
        {
            Debug.LogError("¡AnomalySpawner no encontró ningún GameObject con el script 'SpawnArea'! No se podrán generar anomalías.", this);
            // Desactiva este script si no hay áreas para evitar errores.
            enabled = false;
            return;
        }
        else
        {
            Debug.Log($"AnomalySpawner encontró {spawnAreas.Count} áreas de spawn.");
        }

        // Inicializa el temporizador con un valor aleatorio para la primera aparición.
        ResetSpawnTimer();
    }

    void Update()
    {
        // Si no hay áreas, no hacer nada.
        if (spawnAreas.Count == 0) return;

        // Actualiza el temporizador.
        spawnTimer -= Time.deltaTime;

        // Si el temporizador llegó a cero, intenta generar una anomalía.
        if (spawnTimer <= 0f)
        {
            // Antes de intentar generar, limpia la lista de anomalías destruidas.
            CleanupDestroyedAnomalies();

            // Comprueba si ya hemos alcanzado el máximo de anomalías activas.
            if (activeAnomalies.Count < maxActiveAnomalies)
            {
                // Intenta generar una nueva anomalía.
                TrySpawnAnomaly();
            }
            else
            {
                 // Opcional: Log si se alcanzó el límite
                 // Debug.Log("Se alcanzó el máximo de anomalías activas.");
            }

            // Reinicia el temporizador para la próxima aparición.
            ResetSpawnTimer();
        }
    }

    // Intenta generar una anomalía en una de las áreas disponibles.
    void TrySpawnAnomaly()
    {
        // Elige un área de spawn al azar de la lista de áreas encontradas.
        int randomAreaIndex = Random.Range(0, spawnAreas.Count);
        SpawnArea selectedArea = spawnAreas[randomAreaIndex];

        // Comprueba si el área seleccionada es válida (tiene collider y está activa).
        if (selectedArea == null || !selectedArea.gameObject.activeInHierarchy || !selectedArea.enabled)
        {
             Debug.LogWarning($"Se intentó generar en un área inválida o inactiva ({selectedArea?.name}). Saltando este intento.", selectedArea);
             return; // Salta este intento si el área no es válida
        }


        // Obtiene un prefab de anomalía aleatorio *específico para esa área*.
        GameObject prefabToSpawn = selectedArea.GetRandomAnomalyPrefab();

        // Si el área no tenía prefabs asignados o devolvió null...
        if (prefabToSpawn == null)
        {
            // Debug.Log($"El área seleccionada '{selectedArea.name}' no tiene prefabs válidos para generar.");
            return; // No se puede generar nada desde esta área.
        }

        // Obtiene una posición aleatoria DENTRO del área seleccionada.
        Vector3 spawnPosition = selectedArea.GetRandomPointInArea();

        // --- Opcional: Comprobación de Solapamiento ---
        // Si quieres evitar que aparezcan encima de otros colliders (ej. fondo, otras anomalías),
        // puedes hacer un Physics2D.OverlapCircle o similar aquí antes de instanciar.
        // float checkRadius = 0.5f; // Ajusta el radio según el tamaño de tus anomalías
        // Collider2D overlap = Physics2D.OverlapCircle(spawnPosition, checkRadius);
        // if (overlap != null) {
        //     Debug.Log($"Intento de spawn en {spawnPosition} bloqueado por solapamiento con {overlap.name}. Reintentando la próxima vez.");
        //     return; // No generar si hay algo en medio
        // }
        // --- Fin Opcional ---


        // ¡Genera (Instancia) la anomalía!
        GameObject newAnomaly = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        // Asegúrate de que la nueva anomalía tenga la etiqueta correcta si no la tiene el prefab.
        // newAnomaly.tag = "Anomaly"; // Descomenta si es necesario

        // Añade la nueva anomalía a nuestra lista de seguimiento.
        activeAnomalies.Add(newAnomaly);

        // Debug.Log($"Anomalía '{newAnomaly.name}' generada en el área '{selectedArea.name}' en la posición {spawnPosition}");
    }

    // Reinicia el temporizador con un nuevo valor aleatorio dentro del intervalo definido.
    void ResetSpawnTimer()
    {
        spawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    // Elimina referencias a anomalías que ya fueron destruidas de la lista activeAnomalies.
    void CleanupDestroyedAnomalies()
    {
        // Es más eficiente iterar y comprobar null o usar RemoveAll.
        // Iterar hacia atrás es seguro si eliminas elementos mientras iteras.
        for (int i = activeAnomalies.Count - 1; i >= 0; i--)
        {
            if (activeAnomalies[i] == null) // Si el GameObject fue destruido...
            {
                activeAnomalies.RemoveAt(i); // Elimina la referencia de la lista.
            }
        }
        // Alternativa usando Linq (más conciso pero genera algo de "basura"):
        // activeAnomalies.RemoveAll(item => item == null);
    }

    // --- Opcional: Método público si necesitas que algo externo fuerce una aparición ---
    // public void ForceSpawnAnomaly()
    // {
    //     CleanupDestroyedAnomalies();
    //     if (activeAnomalies.Count < maxActiveAnomalies)
    //     {
    //          TrySpawnAnomaly();
    //          ResetSpawnTimer(); // Reiniciar timer después de forzar
    //     }
    // }
}