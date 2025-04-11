using UnityEngine;
using UnityEngine.UI;          // Para Image, Text
using UnityEngine.SceneManagement; // Para cargar escenas
using System.Collections;       // Para Coroutines (IEnumerator)
using UnityEngine.Networking;

// Asegura que haya al menos un AudioSource (para el screamer/efectos one-shot).
// El script añadirá un segundo AudioSource si es necesario para el latido.
[RequireComponent(typeof(AudioSource))]
public class HealthController : MonoBehaviour
{
    // --- Variables de Configuración (Asignar en Inspector) ---

    [Header("UI y Referencias")]
    [Tooltip("La imagen UI que actúa como barra de vida.")]
    public Image healthBarFill;
    [Tooltip("El GameObject con la imagen del screamer (debe estar desactivado inicialmente).")]
    public GameObject screamerImageObject;
    [Tooltip("Referencia al script AnomalySpawner para obtener el conteo de anomalías.")]
    public AnomalySpawner anomalySpawner;
    [Tooltip("El componente Text (o TextMeshProUGUI) para mostrar el puntaje.")]
    public Text scoreText; // O si usas TextMeshPro: public TMPro.TextMeshProUGUI scoreText;
    [Tooltip("El AudioSource que reproduce la música/ambiente principal. Asignar en Inspector.")]
    public AudioSource mainMusicAudioSource;

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
    [Tooltip("Sonido de latido del corazón (debe ser un loop corto idealmente).")]
    public AudioClip heartbeatSoundClip;
    [Tooltip("Porcentaje de vida (0.0 a 1.0) por debajo del cual empieza el latido.")]
    [Range(0f, 1f)]
    public float heartbeatStartThreshold = 0.3f; // Ej: 30%
    [Tooltip("Velocidad (pitch) mínima del latido (1 = normal).")]
    public float minHeartbeatPitch = 0.8f;
    [Tooltip("Velocidad (pitch) máxima del latido (cuando la vida está cerca de 0).")]
    public float maxHeartbeatPitch = 2.0f;
    [Tooltip("Volumen máximo que alcanzará el latido (0 a 1).")]
    [Range(0f, 1f)]
    public float maxHeartbeatVolume = 1.0f;
    [Tooltip("A qué fracción del volumen original bajará la música (0=silencio, 0.1=10%).")]
    [Range(0f, 1f)]
    public float loweredMusicVolumeMultiplier = 0.1f; // Multiplicador para volumen bajo música

    [Header("Puntaje")]
    [Tooltip("Puntos ganados por cada segundo de supervivencia.")]
    public float pointsPerSecond = 10f;

    [Header("Identificación del Juego")] // Opcional
    [Tooltip("El ID de este juego (ej. 3 para Terror) como está en la tabla 'juegos' de la BD.")]
    public int currentGameId = 3; // <-- ¡AJUSTA ESTE ID!

    [Header("API Settings")] // Opcional
    [Tooltip("URL completa del endpoint API para guardar el puntaje.")]
    public string saveScoreApiUrl = "https://localhost:7058/Score/SaveGameResult"; // <-- ¡¡CAMBIA ESTA URL!!

    // --- Variables Internas (No tocar en Inspector) ---
    private float currentHealth;                  // Vida actual
    private bool andatti = false;                 // Efecto Andatti activo?
    private bool healthChangedThisFrame = false;  // ¿Cambió vida este frame? (Para actualizar UI)
    private bool isGameOver = false;              // ¿Terminó el juego?
    private AudioSource audioSource;              // AudioSource principal (para screamer/one-shots)
    private AudioSource heartbeatAudioSource;     // AudioSource dedicado al loop del latido
    private float currentScore = 0f;              // Puntaje actual
    private float initialMusicVolume;             // Volumen original de la música principal

    // --- Métodos de Ciclo de Vida de Unity ---

    void Start()
    {
        // Obtiene el AudioSource principal
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null) audioSource.playOnAwake = false;
        else Debug.LogError("¡HealthController necesita un componente AudioSource principal!", this);

        // Configura el AudioSource secundario para el latido
        SetupHeartbeatAudioSource();

        // Guarda el volumen inicial de la música si está asignada
        if (mainMusicAudioSource != null) initialMusicVolume = mainMusicAudioSource.volume;
        else initialMusicVolume = 1f; // Valor por defecto si no se asigna

        // Inicializa el estado del juego
        currentHealth = maxHealth;
        currentScore = 0f;
        andatti = false;
        isGameOver = false;

        // Comprueba referencias críticas y configura estado inicial de UI
        CheckReferences();
        if (screamerImageObject != null) screamerImageObject.SetActive(false);
        UpdateHealthBar();
        UpdateScoreDisplay();
    }

    void Update()
    {
        if (isGameOver) return; // No hacer nada si el juego terminó

        UpdateScore();         // Actualiza puntaje
        UpdateHealthDrain();     // Actualiza descenso de vida
        UpdateHeartbeatSound();  // Actualiza sonido latido y música

        // (Prueba opcional con barra espaciadora)
        if (Input.GetKeyDown(KeyCode.Space)) { AddHealth(10f); }
    }

    // Actualiza UI en LateUpdate para reflejar cambios del frame
    void LateUpdate()
    {
        if (isGameOver) return;
        if (healthChangedThisFrame)
        {
            UpdateHealthBar(); // Actualiza barra visual
            healthChangedThisFrame = false; // Resetea bandera
            // Comprueba condición de muerte
            if (currentHealth <= 0f) TriggerGameOverSequence();
        }
    }

    // --- Lógica Principal del Controlador ---

    // Añade y configura el AudioSource para el latido.
    void SetupHeartbeatAudioSource()
    {
        heartbeatAudioSource = gameObject.AddComponent<AudioSource>();
        if (heartbeatSoundClip != null)
        {
            heartbeatAudioSource.clip = heartbeatSoundClip;
            heartbeatAudioSource.loop = true;
            heartbeatAudioSource.playOnAwake = false;
            heartbeatAudioSource.volume = 0f; // Empieza en silencio
            heartbeatAudioSource.pitch = minHeartbeatPitch;
        }
        else
        {
            Debug.LogWarning("No se asignó 'heartbeatSoundClip'. El sonido de latido no funcionará.", this);
            if (heartbeatAudioSource != null) heartbeatAudioSource.enabled = false;
        }
    }

    // Comprueba si las referencias necesarias están asignadas en el Inspector
    void CheckReferences()
    {
        if (anomalySpawner == null) Debug.LogError("Referencia 'anomalySpawner' no asignada.", this);
        if (scoreText == null) Debug.LogWarning("Referencia 'scoreText' no asignada.", this);
        if (screamerImageObject == null) Debug.LogWarning("Referencia 'screamerImageObject' no asignada.", this);
        if (screamerSoundClip == null) Debug.LogWarning("Referencia 'screamerSoundClip' no asignada.", this);
        if (healthBarFill == null) Debug.LogError("Referencia 'healthBarFill' no asignada.", this);
        if (mainMusicAudioSource == null) Debug.LogWarning("Referencia 'mainMusicAudioSource' no asignada.", this);
        if (heartbeatSoundClip == null && heartbeatAudioSource != null && heartbeatAudioSource.enabled) Debug.LogWarning("Referencia 'heartbeatSoundClip' no asignada", this);
        if (string.IsNullOrEmpty(saveScoreApiUrl)) Debug.LogError("Falta configurar 'saveScoreApiUrl'.", this);
    }

    // Calcula y aplica el descenso de vida basado en anomalías
    void UpdateHealthDrain()
    {
        if (!andatti)
        {
            float previousHealth = currentHealth;
            int activeAnomalyCount = (anomalySpawner != null) ? anomalySpawner.ActiveAnomalyCount : 0;
            float totalDecrease = baseDecreaseRate + (activeAnomalyCount * drainPerAnomaly);
            if (totalDecrease > 0 && currentHealth > 0)
            {
                currentHealth -= totalDecrease * Time.deltaTime;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                if (currentHealth != previousHealth) healthChangedThisFrame = true;
            }
        }
    }

    // Incrementa el puntaje basado en el tiempo
    void UpdateScore()
    {
        currentScore += pointsPerSecond * Time.deltaTime;
        UpdateScoreDisplay();
    }

    // Ajusta el pitch/volumen del latido y el volumen de la música principal
    void UpdateHeartbeatSound()
    {
        bool canPlayHeartbeat = (heartbeatAudioSource != null && heartbeatAudioSource.enabled && !isGameOver);
        float healthRatio = (maxHealth > 0) ? Mathf.Clamp01(currentHealth / maxHealth) : 0f;
        float progress = 1f - Mathf.Clamp01(healthRatio / heartbeatStartThreshold); // 0=umbral, 1=cerca de 0 vida

        // --- Ajustar Sonido Latido ---
        if (canPlayHeartbeat && healthRatio <= heartbeatStartThreshold && currentHealth > 0)
        { // Debajo umbral y vivo
            heartbeatAudioSource.pitch = Mathf.Lerp(minHeartbeatPitch, maxHeartbeatPitch, progress); // Acelera pitch
            heartbeatAudioSource.volume = maxHeartbeatVolume; // Volumen latido al máximo
            if (!heartbeatAudioSource.isPlaying) heartbeatAudioSource.Play(); // Inicia si no sonaba
        }
        else
        { // Encima del umbral, muerto, o no usable
            if (heartbeatAudioSource != null && heartbeatAudioSource.isPlaying)
            {
                heartbeatAudioSource.Stop(); // Detiene latido
                heartbeatAudioSource.volume = 0f; // Silencia al parar
            }
        }

        // --- Ajustar Volumen Música Principal ---
        if (mainMusicAudioSource != null)
        {
            if (healthRatio <= heartbeatStartThreshold && currentHealth > 0 && !isGameOver)
            { // Dentro del umbral y vivo
                // <<< CAMBIO AQUÍ: Interpola volumen música gradualmente >>>
                mainMusicAudioSource.volume = Mathf.Lerp(initialMusicVolume, initialMusicVolume * loweredMusicVolumeMultiplier, progress);
            }
            else if (!isGameOver)
            { // Encima del umbral y no es game over
              // Restaura volumen música al original
                mainMusicAudioSource.volume = initialMusicVolume;
            }
            // Si es Game Over, el volumen se quedará como estaba (probablemente bajo tras el Trigger)
        }
    }

    // --- Métodos Públicos ---

    // Añade vida (llamado externamente)
    public void AddHealth(float amountToAdd)
    {
        if (isGameOver || amountToAdd <= 0) return;
        float previousHealth = currentHealth;
        if (currentHealth < maxHealth)
        {
            currentHealth += amountToAdd;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            if (currentHealth != previousHealth) healthChangedThisFrame = true;
        }
    }

    // Activa el efecto Andatti
    public void ConsumeAndatti()
    {
        if (isGameOver || !andatti)
        {
            if (!isGameOver) andatti = true;
            if (andatti) Invoke(nameof(ResetAndatti), 5f);
        }
    }

    // --- Actualizaciones de UI ---

    // Actualiza la barra de vida visual
    private void UpdateHealthBar()
    {
        if (healthBarFill != null && maxHealth > 0)
        {
            healthBarFill.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        }
    }

    // Actualiza el texto del puntaje
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + Mathf.FloorToInt(currentScore).ToString();
        }
    }

    // --- Secuencia de Game Over ---

    // Inicia el fin del juego
        // --- Secuencia de Game Over ---
    // Inicia el fin del juego, guarda puntaje, llama a la API, muestra efectos y carga escena final.
    private void TriggerGameOverSequence()
    {
        // Evita que la secuencia se ejecute más de una vez
        if (isGameOver) return;
        isGameOver = true; // Marca el juego como terminado

        Debug.Log("¡Vida a cero! Iniciando secuencia Game Over...");

        // --- 1. Calcula y guarda puntaje (localmente primero) ---
        // Calcula el puntaje final como entero
        int finalScoreInt = Mathf.FloorToInt(currentScore);
        // Guarda en PlayerPrefs (útil para la escena GameOver o como respaldo)
        PlayerPrefs.SetInt("LastScore", finalScoreInt);
        PlayerPrefs.Save(); // Asegura que se guarde
        Debug.Log($"Puntaje final (PlayerPrefs): {finalScoreInt}");

        // --- 2. Intenta enviar puntaje a la API ---
        // Verifica si tenemos un usuario logueado (a través de UserManager) y una URL de API configurada
        if (UserManager.Instance != null && UserManager.Instance.CurrentUserId.HasValue && !string.IsNullOrEmpty(saveScoreApiUrl))
        {
            // Obtiene el ID del usuario actual
            int userId = UserManager.Instance.CurrentUserId.Value;
            // Inicia la Coroutine para enviar los datos a la API de forma asíncrona
            // Pasa la URL, el ID de usuario, el ID del juego actual y el puntaje final calculado
            StartCoroutine(APIScoreSender.SendScore(saveScoreApiUrl, userId, currentGameId, finalScoreInt));
        }
        else
        {
            // Muestra un aviso si no se puede enviar a la API
            Debug.LogWarning("No se enviará puntaje a API (Falta UserID/UserManager/URL).");
        }
        // --- Fin Llamada API ---

        // --- 3. Efectos inmediatos de Game Over ---
        // Muestra la imagen del screamer si está asignada
        if (screamerImageObject != null) screamerImageObject.SetActive(true);
        // Reproduce el sonido del screamer si está asignado
        if (audioSource != null && screamerSoundClip != null) audioSource.PlayOneShot(screamerSoundClip);
        // Detiene el sonido de latido si estaba sonando
        if (heartbeatAudioSource != null && heartbeatAudioSource.isPlaying) heartbeatAudioSource.Stop();
        // Detiene la música principal si estaba sonando
        if (mainMusicAudioSource != null) mainMusicAudioSource.Stop();

        // --- 4. Pausa el juego ---
        // Detiene el tiempo del juego (física, Update normal, animaciones basadas en tiempo)
        // Se hace DESPUÉS de iniciar las coroutines que necesitan seguir corriendo un poco
        Time.timeScale = 0f;

        // --- 5. Carga escena final después de delay ---
        // Inicia la Coroutine que esperará unos segundos (tiempo real) antes de cargar la escena Game Over
        StartCoroutine(LoadGameOverAfterDelay(3.0f)); // Espera 3 segundos
    } // --- Fin del método TriggerGameOverSequence ---

    // Coroutine para esperar tiempo real y cargar escena
    private IEnumerator LoadGameOverAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f; // ¡¡Restaurar TimeScale ANTES de cargar!!
        SceneManager.LoadScene("HR_GameOver");
    }

    // Desactiva el efecto Andatti (llamado por Invoke)
    private void ResetAndatti()
    {
        andatti = false;
    }

} // Fin de la clase HealthController