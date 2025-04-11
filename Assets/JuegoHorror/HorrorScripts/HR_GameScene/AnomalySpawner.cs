using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class AnomalySpawner : MonoBehaviour
{
    // --- Configuración de Aparición ---
    public float initialMinSpawnInterval = 5.0f; // Tiempo mínimo inicial entre apariciones.
    public float initialMaxSpawnInterval = 10.0f;// Tiempo máximo inicial entre apariciones.
    public float minPossibleInterval = 0.5f;     // Intervalo de aparición más rápido posible.
    public float timeToReachMinInterval = 180f;  // Segundos para alcanzar la velocidad máxima de aparición.
    public int maxActiveAnomalies = 10;          // Número máximo de anomalías permitidas simultáneamente.
    public Text anomalyCountText;                // Elemento UI Text para mostrar el contador actual.


    private List<SpawnArea> spawnAreas = new List<SpawnArea>();         // Lista de ubicaciones de aparición disponibles.
    private List<GameObject> activeAnomalies = new List<GameObject>();  // Lista de instancias de anomalías actualmente activas.
    private float spawnTimer;                     // Temporizador hasta el próximo intento de aparición.
    private float currentMinInterval;             // Intervalo mínimo de aparición actual calculado.
    private float currentMaxInterval;             // Intervalo máximo de aparición actual calculado.
    private float timeElapsed = 0f;               // Tiempo transcurrido desde el inicio del juego.


    public int ActiveAnomalyCount => activeAnomalies.Count;

    void Start()
    {
        // Encuentra todos los componentes SpawnArea en la escena.
        spawnAreas.AddRange(FindObjectsOfType<SpawnArea>());
        if (spawnAreas.Count == 0) {

            enabled = false; // Desactiva el spawner si no existen áreas.
            return;
        }

        // Inicializa los intervalos y el temporizador de aparición.
        currentMinInterval = initialMinSpawnInterval;
        currentMaxInterval = initialMaxSpawnInterval;
        ResetSpawnTimer();
        timeElapsed = 0f;

        // Inicializa la visualización del contador de anomalías.
        UpdateAnomalyCountDisplay();
    }

    void Update()
    {
        if (spawnAreas.Count == 0) return; // No hacer nada si está desactivado o no hay áreas.

        // Actualiza el tiempo de juego y ajusta los intervalos de aparición según el tiempo.
        timeElapsed += Time.deltaTime;
        UpdateSpawnIntervals();

        // Descuenta el temporizador de aparición.
        spawnTimer -= Time.deltaTime;

        // Cuando el temporizador llega a cero...
        if (spawnTimer <= 0f)
        {
            // Elimina cualquier anomalía destruida de la lista de seguimiento.
            CleanupDestroyedAnomalies();

            // Intenta generar una nueva anomalía si está por debajo del límite máximo.
            if (ActiveAnomalyCount < maxActiveAnomalies)
            {
                TrySpawnAnomaly();
            }

            // Reinicia el temporizador para la próxima aparición.
            ResetSpawnTimer();
        }
    }

    // --- Lógica de Aparición ---


    void UpdateSpawnIntervals()
    {
        float progress = Mathf.Clamp01(timeElapsed / timeToReachMinInterval); // Progreso hacia velocidad máx (0 a 1).
        currentMinInterval = Mathf.Lerp(initialMinSpawnInterval, minPossibleInterval, progress);
        currentMaxInterval = Mathf.Lerp(initialMaxSpawnInterval, minPossibleInterval * 1.5f, progress);
        // Asegura que el intervalo máximo sea ligeramente mayor que el mínimo.
        currentMaxInterval = Mathf.Max(currentMaxInterval, currentMinInterval + 0.1f);
    }

    // Intenta generar una anomalía en un área de aparición válida y aleatoria.
    void TrySpawnAnomaly()
    {
        int randomAreaIndex = Random.Range(0, spawnAreas.Count);
        SpawnArea selectedArea = spawnAreas[randomAreaIndex];

        // Salta si el área seleccionada es inválida o está desactivada.
        if (selectedArea == null || !selectedArea.gameObject.activeInHierarchy || !selectedArea.enabled) return;

        GameObject prefabToSpawn = selectedArea.GetRandomAnomalyPrefab();
        // Salta si el área no proporcionó un prefab válido.
        if (prefabToSpawn == null) return;

        // Obtiene una posición aleatoria dentro del área seleccionada (respetando el padding).
        Vector3 spawnPosition = selectedArea.GetRandomPointInArea();
        // Instancia el prefab de la anomalía.
        GameObject newAnomaly = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        // Añade la nueva instancia a la lista de seguimiento.
        activeAnomalies.Add(newAnomaly);

        // Actualiza la visualización del contador UI.
        UpdateAnomalyCountDisplay();
    }

    // Reinicia el temporizador de aparición a un valor aleatorio dentro del rango de intervalo actual.
    void ResetSpawnTimer() {
        spawnTimer = Random.Range(currentMinInterval, currentMaxInterval);
    }

    void CleanupDestroyedAnomalies() {
        int countBefore = activeAnomalies.Count;
        // Elimina todos los elementos donde item es null.
        activeAnomalies.RemoveAll(item => item == null);
        // Actualiza la visualización solo si el contador cambió realmente durante la limpieza.
        if (activeAnomalies.Count != countBefore) {
             UpdateAnomalyCountDisplay();
        }
    }

    // Método público llamado por otros scripts cuando destruyen una anomalía.
    public void NotifyAnomalyDestroyed(GameObject destroyedAnomaly)
    {
        // Intenta eliminar la anomalía especificada de la lista.
        if (activeAnomalies.Remove(destroyedAnomaly)) // Remove() devuelve true si tuvo éxito.
        {
            // Actualiza el contador UI inmediatamente.
            UpdateAnomalyCountDisplay();
        }
    }

    // --- Actualización de UI ---

    // Actualiza el componente Text del contador de anomalías.
    void UpdateAnomalyCountDisplay()
    {
        if (anomalyCountText != null) {
            anomalyCountText.text = ActiveAnomalyCount.ToString();
        }
    }
} 