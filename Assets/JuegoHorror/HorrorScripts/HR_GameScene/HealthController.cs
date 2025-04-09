using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

// Asegura que el GameObject tenga al menos un AudioSource (para el screamer principalmente).
// El script añadirá un segundo AudioSource para el latido.
[RequireComponent(typeof(AudioSource))]
public class HealthController : MonoBehaviour
{
    // --- Variables de Configuración (Asignar en Inspector) ---

    [Header("UI y Referencias")]
    [Tooltip("La imagen UI que actúa como barra de vida.")]
    public Image healthBarFill;
    [Tooltip("El GameObject con la imagen del screamer (debe estar desactivado inicialmente).")]
    public GameObject screamerImageObject;
    [Tooltip("Referencia al script AnomalySpawner para obtener el conteo.")]
    public AnomalySpawner anomalySpawner;
    [Tooltip("El componente Text (o TextMeshProUGUI) para mostrar el puntaje.")]
    public Text scoreText; // O: public TMPro.TextMeshProUGUI scoreText;

    [Header("Vida y Descenso")]
    [Tooltip("Vida máxima inicial.")]
    public float maxHealth = 100f;
    [Tooltip("Vida perdida por segundo por CADA anomalía activa.")]
    public float drainPerAnomaly = 0.75f;
    [Tooltip("Opcional: Descenso base constante de vida por segundo (poner a 0 si no se desea).")]
    public float baseDecreaseRate = 0f;

    [Header("Sonido")]
    [Tooltip("Sonido que se reproduce al morir (screamer).")]
    public AudioClip screamerSoundClip;
    [Tooltip("Sonido de latido del corazón.")]
    public AudioClip heartbeatSoundClip;
    [Tooltip("Porcentaje de vida (0.0 a 1.0) por debajo del cual empieza el latido.")]
    [Range(0f, 1f)]
    public float heartbeatStartThreshold = 0.3f; // 30%
    [Tooltip("Velocidad (pitch) mínima del latido.")]
    public float minHeartbeatPitch = 0.8f;
    [Tooltip("Velocidad (pitch) máxima del latido (cuando la vida está cerca de 0).")]
    public float maxHeartbeatPitch = 2.0f;

    [Header("Puntaje")]
    [Tooltip("Puntos ganados por cada segundo de supervivencia.")]
    public float pointsPerSecond = 10f;

    // --- Variables Internas ---
    private float currentHealth;                  // Vida actual
    private bool andatti = false;                 // Efecto Andatti activo?
    private bool healthChangedThisFrame = false;  // ¿Cambió vida este frame?
    private bool isGameOver = false;              // ¿Terminó el juego?
    private AudioSource audioSource;              // Para sonidos 'one-shot' (screamer)
    private AudioSource heartbeatAudioSource;     // Para el loop del latido
    private float currentScore = 0f;              // Puntaje actual

    // --- Métodos de Unity ---

    void Start()
    {
        // Configuración inicial del AudioSource principal (requerido por [RequireComponent])
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null) {
            audioSource.playOnAwake = false; // No queremos que suene nada al inicio
        } else {
             // Esto no debería pasar debido a RequireComponent, pero por si acaso
             Debug.LogError("¡Falta el componente AudioSource principal!", this);
        }

        // Configuración del AudioSource secundario para el latido
        SetupHeartbeatAudioSource();

        // Inicialización del estado del juego
        currentHealth = maxHealth;
        currentScore = 0f;
        andatti = false;
        isGameOver = false;

        // Comprobación y configuración inicial de UI y otros elementos
        CheckReferences();
        if (screamerImageObject != null) screamerImageObject.SetActive(false);
        UpdateHealthBar();
        UpdateScoreDisplay();
    }

    void Update()
    {
        // No ejecutar lógica de juego si ya terminó
        if (isGameOver) return;

        // Actualiza el puntaje basado en el tiempo
        UpdateScore();

        // Actualiza la vida basado en anomalías y tiempo
        UpdateHealthDrain();

        // Actualiza el sonido del latido basado en la vida actual
        UpdateHeartbeatSound();

        // (Prueba opcional con barra espaciadora)
        if (Input.GetKeyDown(KeyCode.Space)) { AddHealth(10f); }
    }

    // Actualizaciones visuales (UI) se hacen en LateUpdate
    void LateUpdate()
    {
        if (isGameOver) return;

        // Si la vida cambió este fotograma, actualiza la barra
        if (healthChangedThisFrame)
        {
            UpdateHealthBar();
            healthChangedThisFrame = false; // Resetea la bandera

            // Comprueba si el jugador murió después de la actualización
            if (currentHealth <= 0f)
            {
                TriggerGameOverSequence();
            }
        }
    }

    // --- Lógica Principal ---

    // Configura el segundo AudioSource para el latido
    void SetupHeartbeatAudioSource() {
         heartbeatAudioSource = gameObject.AddComponent<AudioSource>(); // Añade un nuevo AudioSource
         if (heartbeatSoundClip != null) {
             heartbeatAudioSource.clip = heartbeatSoundClip;
             heartbeatAudioSource.loop = true;
             heartbeatAudioSource.playOnAwake = false;
             heartbeatAudioSource.volume = 1f; // Ajusta si es necesario
             heartbeatAudioSource.pitch = minHeartbeatPitch;
             // heartbeatAudioSource.spatialBlend = 0f; // Sonido 2D
         } else {
             Debug.LogWarning("No se asignó 'heartbeatSoundClip'. El sonido de latido no funcionará.", this);
             heartbeatAudioSource.enabled = false; // Desactiva si no hay clip
         }
    }

    // Comprueba si las referencias clave están asignadas en el Inspector
    void CheckReferences() {
        if (anomalySpawner == null) Debug.LogError("¡Referencia 'anomalySpawner' no asignada!", this);
        if (scoreText == null) Debug.LogWarning("Referencia 'scoreText' no asignada.", this);
        if (screamerImageObject == null) Debug.LogWarning("Referencia 'screamerImageObject' no asignada.", this);
        if (screamerSoundClip == null) Debug.LogWarning("Referencia 'screamerSoundClip' no asignada.", this);
        if (healthBarFill == null) Debug.LogError("¡Referencia 'healthBarFill' no asignada!", this);
    }

    // Calcula y aplica el descenso de vida
    void UpdateHealthDrain() {
        if (!andatti) // Solo si no está activo el efecto Andatti
        {
            float previousHealth = currentHealth;
            int activeAnomalyCount = (anomalySpawner != null) ? anomalySpawner.ActiveAnomalyCount : 0;
            float totalDecrease = baseDecreaseRate + (activeAnomalyCount * drainPerAnomaly);

            if (totalDecrease > 0 && currentHealth > 0) // Aplica solo si hay descenso y vida > 0
            {
                currentHealth -= totalDecrease * Time.deltaTime;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Limita a 0-max

                if (currentHealth != previousHealth) {
                    healthChangedThisFrame = true; // Marca para actualizar UI
                }
            }
        }
    }

    // Incrementa el puntaje con el tiempo
    void UpdateScore() {
        currentScore += pointsPerSecond * Time.deltaTime;
        UpdateScoreDisplay(); // Actualiza la UI del puntaje
    }

    // Controla el sonido del latido (inicio, parada, pitch)
    void UpdateHeartbeatSound() {
        if (heartbeatAudioSource == null || !heartbeatAudioSource.enabled || isGameOver) {
            // Detiene si ya no es válido o terminó el juego
            if (heartbeatAudioSource != null && heartbeatAudioSource.isPlaying) heartbeatAudioSource.Stop();
            return;
        }

        float healthRatio = (maxHealth > 0) ? (currentHealth / maxHealth) : 0f;

        if (healthRatio <= heartbeatStartThreshold && currentHealth > 0) { // Por debajo del umbral y vivo
            float progress = 1f - Mathf.Clamp01(healthRatio / heartbeatStartThreshold);
            heartbeatAudioSource.pitch = Mathf.Lerp(minHeartbeatPitch, maxHeartbeatPitch, progress);
            if (!heartbeatAudioSource.isPlaying) heartbeatAudioSource.Play(); // Inicia si no sonaba
        } else { // Por encima del umbral o muerto
            if (heartbeatAudioSource.isPlaying) heartbeatAudioSource.Stop(); // Detiene si sonaba
        }
    }

    // --- Métodos Públicos ---

    // Añade vida (llamado externamente)
    public void AddHealth(float amountToAdd) {
        if (isGameOver || amountToAdd <= 0) return; // No hacer nada si terminó o no es positivo
        float previousHealth = currentHealth;
        if (currentHealth < maxHealth) {
            currentHealth += amountToAdd;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            if (currentHealth != previousHealth) {
                healthChangedThisFrame = true; // Marca para actualizar UI
            }
        }
    }

    // Activa el efecto Andatti (pausa descenso de vida)
    public void ConsumeAndatti() {
        if (isGameOver || !andatti) {
             if(!isGameOver) andatti = true;
             if(andatti) Invoke(nameof(ResetAndatti), 5f); // Desactivar después de 5 seg
        }
    }

    // --- Actualizaciones de UI ---

    // Actualiza la barra de vida visual
    private void UpdateHealthBar() {
        if (healthBarFill != null && maxHealth > 0) {
            healthBarFill.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        }
    }

    // Actualiza el texto del puntaje
    private void UpdateScoreDisplay() {
        if (scoreText != null) {
            scoreText.text = "Puntos: " + Mathf.FloorToInt(currentScore).ToString();
        }
    }

    // --- Secuencia de Game Over ---

    // Inicia el fin del juego
    private void TriggerGameOverSequence() {
        if (isGameOver) return; // Ejecutar solo una vez
        isGameOver = true;

        // Guardar puntaje final
        PlayerPrefs.SetInt("LastScore", Mathf.FloorToInt(currentScore));
        PlayerPrefs.Save();
        Debug.Log($"Puntaje final guardado: {PlayerPrefs.GetInt("LastScore")}");

        // Activar efectos visuales y sonoros
        if (screamerImageObject != null) screamerImageObject.SetActive(true);
        if (audioSource != null && screamerSoundClip != null) audioSource.PlayOneShot(screamerSoundClip);
        if (heartbeatAudioSource != null && heartbeatAudioSource.isPlaying) heartbeatAudioSource.Stop(); // Detiene latido

        // Pausa el juego
        Time.timeScale = 0f;

        // Inicia coroutine para cargar escena de Game Over
        StartCoroutine(LoadGameOverAfterDelay(3.0f));
    }

    // Coroutine para esperar tiempo real y cargar escena
    private IEnumerator LoadGameOverAfterDelay(float delay) {
        // Debug.Log($"Esperando {delay} segundos (tiempo real)...");
        yield return new WaitForSecondsRealtime(delay); // Espera ignorando Time.timeScale

        // Debug.Log("Restaurando TimeScale y cargando HR_GameOver...");
        Time.timeScale = 1f; // ¡¡Restaurar TimeScale ANTES de cargar!!
        SceneManager.LoadScene("HR_GameOver"); // Carga la escena
    }

    // Desactiva el efecto Andatti (llamado por Invoke)
    private void ResetAndatti() {
        andatti = false;
    }
}