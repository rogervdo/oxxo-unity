using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Necesario para RemoveAll
using UnityEngine.UI; // Necesario para UI Text

public class AnomalySpawner : MonoBehaviour
{
    // --- Configuración (Asignar en Inspector) ---
    [Header("Intervalo de Aparición")]
    public float initialMinSpawnInterval = 5.0f; // Mínimo inicial
    public float initialMaxSpawnInterval = 10.0f;// Máximo inicial

    [Header("Aceleración de Aparición")]
    public float minPossibleInterval = 0.5f;     // Intervalo más rápido
    public float timeToReachMinInterval = 180f;  // Segundos para máxima velocidad

    [Header("Límite")]
    public int maxActiveAnomalies = 10;          // Máximo de anomalías a la vez

    [Header("UI (Opcional)")]
    public Text anomalyCountText;                // Texto para mostrar el contador

    // --- Variables Internas ---
    private List<SpawnArea> spawnAreas = new List<SpawnArea>();         // Lista de áreas disponibles
    private List<GameObject> activeAnomalies = new List<GameObject>();  // Lista de anomalías activas
    private float spawnTimer;                     // Temporizador para la próxima aparición
    private float currentMinInterval;             // Intervalo mínimo actual (calculado)
    private float currentMaxInterval;             // Intervalo máximo actual (calculado)
    private float timeElapsed = 0f;               // Tiempo de juego transcurrido

    // --- Propiedad Pública de Conteo ---
    // Permite a otros scripts leer cuántas anomalías hay activas.
    public int ActiveAnomalyCount => activeAnomalies.Count;

    // --- Métodos de Unity ---
    void Start()
    {
        // Busca las áreas de spawn al inicio.
        spawnAreas.AddRange(FindObjectsOfType<SpawnArea>());
        if (spawnAreas.Count == 0) {
            Debug.LogError("AnomalySpawner: ¡No se encontraron SpawnAreas!", this);
            enabled = false; // Desactiva si no hay áreas
            return;
        }

        // Configura los intervalos y el temporizador inicial.
        currentMinInterval = initialMinSpawnInterval;
        currentMaxInterval = initialMaxSpawnInterval;
        ResetSpawnTimer();
        timeElapsed = 0f;

        // Actualiza la UI del contador al inicio.
        UpdateAnomalyCountDisplay();
        if (anomalyCountText == null) {
             Debug.LogWarning("Referencia 'anomalyCountText' no asignada en AnomalySpawner.", this);
        }
    }

    void Update()
    {
        if (spawnAreas.Count == 0) return; // No hacer nada si no hay áreas

        // Actualiza el tiempo transcurrido y los intervalos de aparición.
        timeElapsed += Time.deltaTime;
        UpdateSpawnIntervals();

        // Descuenta el temporizador.
        spawnTimer -= Time.deltaTime;

        // Si el temporizador llega a cero...
        if (spawnTimer <= 0f)
        {
            CleanupDestroyedAnomalies(); // Limpia referencias nulas (por si acaso)

            // Intenta generar una nueva anomalía si no se ha alcanzado el límite.
            if (ActiveAnomalyCount < maxActiveAnomalies)
            {
                TrySpawnAnomaly(); // Genera y actualiza contador visual
            }

            ResetSpawnTimer(); // Reinicia el temporizador para la próxima vez
        }
    }

    // --- Lógica de Aparición ---
    // Ajusta los intervalos de spawn para que sean más rápidos con el tiempo.
    void UpdateSpawnIntervals()
    {
        float progress = Mathf.Clamp01(timeElapsed / timeToReachMinInterval);
        currentMinInterval = Mathf.Lerp(initialMinSpawnInterval, minPossibleInterval, progress);
        currentMaxInterval = Mathf.Lerp(initialMaxSpawnInterval, minPossibleInterval * 1.5f, progress);
        currentMaxInterval = Mathf.Max(currentMaxInterval, currentMinInterval + 0.1f); // Asegura max > min
    }

    // Intenta generar una anomalía en un área aleatoria.
    void TrySpawnAnomaly()
    {
        int randomAreaIndex = Random.Range(0, spawnAreas.Count);
        SpawnArea selectedArea = spawnAreas[randomAreaIndex];
        if (selectedArea == null || !selectedArea.gameObject.activeInHierarchy || !selectedArea.enabled) return; // Salta si el área es inválida

        GameObject prefabToSpawn = selectedArea.GetRandomAnomalyPrefab();
        if (prefabToSpawn == null) return; // Salta si no hay prefab válido en el área

        Vector3 spawnPosition = selectedArea.GetRandomPointInArea(); // Obtiene posición ajustada por padding
        GameObject newAnomaly = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity); // Crea la anomalía
        activeAnomalies.Add(newAnomaly); // Añade a la lista de seguimiento

        UpdateAnomalyCountDisplay(); // Actualiza el contador visual
    }

    // Reinicia el temporizador con un valor aleatorio basado en los intervalos actuales.
    void ResetSpawnTimer() {
        spawnTimer = Random.Range(currentMinInterval, currentMaxInterval);
    }

    // --- Gestión de la Lista y Notificación ---
    // Elimina referencias nulas (GameObjects destruidos) de la lista.
    void CleanupDestroyedAnomalies() {
        int countBefore = activeAnomalies.Count;
        activeAnomalies.RemoveAll(item => item == null);
        // Actualiza display solo si hubo cambios al limpiar (menos probable ahora con Notify)
        if (activeAnomalies.Count != countBefore) {
             UpdateAnomalyCountDisplay();
        }
    }

    // Método PÚBLICO llamado por otros scripts (ClickEffectSpawner) cuando destruyen una anomalía.
    public void NotifyAnomalyDestroyed(GameObject destroyedAnomaly)
    {
        // Si la anomalía destruida está en nuestra lista...
        if (activeAnomalies.Remove(destroyedAnomaly)) // Remove devuelve true si la eliminó
        {
            // Actualiza el contador visual INMEDIATAMENTE.
            UpdateAnomalyCountDisplay();
        }
    }

    // --- Actualización de UI ---
    // Actualiza el componente Text con el número actual de anomalías.
    void UpdateAnomalyCountDisplay()
    {
        if (anomalyCountText != null) {
            anomalyCountText.text = ActiveAnomalyCount.ToString();
        }
    }
}