using UnityEngine;
using UnityEngine.UI;         
using UnityEngine.SceneManagement; 
using System.Collections;       
using UnityEngine.Networking;  


[RequireComponent(typeof(AudioSource))]
public class HealthController : MonoBehaviour
{
    // --- UI y Referencias ---
    public Image healthBarFill;         // Imagen UI que actúa como barra de vida.
    public GameObject screamerImageObject; // GameObject con imagen screamer (desactivado inicialmente).
    public AnomalySpawner anomalySpawner;  // Referencia a AnomalySpawner para conteo.
    public Text scoreText;              // Componente Text para mostrar puntaje.
    public AudioSource mainMusicAudioSource; // AudioSource de música/ambiente principal.

    // --- Vida y Descenso ---
    public float maxHealth = 100f;          // Vida máxima inicial.
    public float drainPerAnomaly = 0.75f;   // Vida perdida/seg por CADA anomalía activa.
    public float baseDecreaseRate = 0f;     // Descenso base constante de vida/seg (0 si no se desea).

    // --- Sonido ---
    public AudioClip screamerSoundClip;     // Sonido al morir (screamer).
    public AudioClip heartbeatSoundClip;    // Sonido de latido (loop corto idealmente).
    [Range(0f, 1f)]
    public float heartbeatStartThreshold = 0.3f; // Porcentaje de vida (0-1) bajo el cual empieza latido.
    public float minHeartbeatPitch = 0.8f;      // Velocidad (pitch) mínima del latido (1=normal).
    public float maxHeartbeatPitch = 2.0f;      // Velocidad (pitch) máxima del latido (vida cerca de 0).
    [Range(0f, 1f)]
    public float maxHeartbeatVolume = 1.0f;     // Volumen máximo del latido (0 a 1).
    [Range(0f, 1f)]
    public float loweredMusicVolumeMultiplier = 0.1f; // Multiplicador para bajar volumen música (0=silencio).

    // --- Puntaje ---
    public float pointsPerSecond = 10f;     // Puntos ganados por segundo de supervivencia.

    // --- Identificación del Juego ---
    public int currentGameId = 3;           // ID de este juego en base datos

    // --- Configuración API ---
    public string saveScoreApiUrl = "https://localhost:7058/Score/SaveGameResult"; // URL endpoint API para guardar puntaje.

    // --- Variables Internas ---
    private float currentHealth;                  // Vida actual.
    private bool andatti = false;                 // ¿Efecto Andatti activo?
    private bool healthChangedThisFrame = false;  // ¿Cambió vida este frame? (para actualizar UI).
    private bool isGameOver = false;              // ¿Terminó el juego?
    private AudioSource audioSource;              // AudioSource principal (screamer/one-shots).
    private AudioSource heartbeatAudioSource;     // AudioSource dedicado al loop del latido.
    private float currentScore = 0f;              // Puntaje actual.
    private float initialMusicVolume;             // Volumen original de la música principal.

    void Start()
    {
        // Obtiene el AudioSource principal.
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null) audioSource.playOnAwake = false;


        // Configura el AudioSource secundario para el latido.
        SetupHeartbeatAudioSource();

        // Guarda el volumen inicial de la música si está asignada.
        if (mainMusicAudioSource != null) initialMusicVolume = mainMusicAudioSource.volume;
        else initialMusicVolume = 1f; // Valor por defecto si no se asigna.

        // Inicializa el estado del juego.
        currentHealth = maxHealth;
        currentScore = 0f;
        andatti = false;
        isGameOver = false;

        // Comprueba referencias críticas y configura estado inicial de UI.
        CheckReferences(); // Se mantiene la llamada, pero los logs internos se eliminan.
        if (screamerImageObject != null) screamerImageObject.SetActive(false);
        UpdateHealthBar();
        UpdateScoreDisplay();
    }

    void Update()
    {
        if (isGameOver) return; // No hacer nada si el juego terminó.

        UpdateScore();         // Actualiza puntaje.
        UpdateHealthDrain();     // Actualiza descenso de vida.
        UpdateHeartbeatSound();  // Actualiza sonido latido y música.

        // (Prueba opcional con barra espaciadora - MANTENIDA)
        if (Input.GetKeyDown(KeyCode.Space)) { AddHealth(10f); }
    }

    // Actualiza UI en LateUpdate para reflejar cambios del frame.
    void LateUpdate()
    {
        if (isGameOver) return;
        if (healthChangedThisFrame)
        {
            UpdateHealthBar(); // Actualiza barra visual.
            healthChangedThisFrame = false; // Resetea bandera.
            // Comprueba condición de muerte.
            if (currentHealth <= 0f) TriggerGameOverSequence();
        }
    }

    // --- Lógica Principal del Controlador ---


    void SetupHeartbeatAudioSource()
    {
        heartbeatAudioSource = gameObject.AddComponent<AudioSource>();
        if (heartbeatSoundClip != null)
        {
            heartbeatAudioSource.clip = heartbeatSoundClip;
            heartbeatAudioSource.loop = true;
            heartbeatAudioSource.playOnAwake = false;
            heartbeatAudioSource.volume = 0f; 
            heartbeatAudioSource.pitch = minHeartbeatPitch;
        }
        else
        {

            if (heartbeatAudioSource != null) heartbeatAudioSource.enabled = false;
        }
    }


    void CheckReferences()
    {

    }

    // Calcula y aplica el descenso de vida basado en anomalías.
    void UpdateHealthDrain()
    {
        if (!andatti) // Solo drena si el efecto 'andatti' no está activo.
        {
            float previousHealth = currentHealth;
            int activeAnomalyCount = (anomalySpawner != null) ? anomalySpawner.ActiveAnomalyCount : 0;
            float totalDecrease = baseDecreaseRate + (activeAnomalyCount * drainPerAnomaly);
            if (totalDecrease > 0 && currentHealth > 0)
            {
                currentHealth -= totalDecrease * Time.deltaTime;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Limita entre 0 y maxHealth.
                if (currentHealth != previousHealth) healthChangedThisFrame = true; // Marca para actualizar UI.
            }
        }
    }

    // Incrementa el puntaje basado en el tiempo.
    void UpdateScore()
    {
        currentScore += pointsPerSecond * Time.deltaTime;
        UpdateScoreDisplay();
    }

    // Ajusta el pitch/volumen del latido y el volumen de la música principal.
    void UpdateHeartbeatSound()
    {
        bool canPlayHeartbeat = (heartbeatAudioSource != null && heartbeatAudioSource.enabled && !isGameOver);
        float healthRatio = (maxHealth > 0) ? Mathf.Clamp01(currentHealth / maxHealth) : 0f; // Proporción de vida (0 a 1).
        // Progreso inverso dentro del umbral (0=en umbral, 1=cerca de 0 vida).
        float progress = 1f - Mathf.Clamp01(healthRatio / heartbeatStartThreshold);

        // --- Ajustar Sonido Latido ---
        if (canPlayHeartbeat && healthRatio <= heartbeatStartThreshold && currentHealth > 0)
        { // Debajo umbral y vivo.
            heartbeatAudioSource.pitch = Mathf.Lerp(minHeartbeatPitch, maxHeartbeatPitch, progress); // Acelera pitch.
            heartbeatAudioSource.volume = maxHeartbeatVolume; // Volumen latido al máximo.
            if (!heartbeatAudioSource.isPlaying) heartbeatAudioSource.Play(); // Inicia si no sonaba.
        }
        else
        { // Encima del umbral, muerto, o no usable.
            if (heartbeatAudioSource != null && heartbeatAudioSource.isPlaying)
            {
                heartbeatAudioSource.Stop(); // Detiene latido.
                heartbeatAudioSource.volume = 0f; // Silencia al parar.
            }
        }

        // --- Ajustar Volumen Música Principal ---
        if (mainMusicAudioSource != null)
        {
            if (healthRatio <= heartbeatStartThreshold && currentHealth > 0 && !isGameOver)
            { // Dentro del umbral y vivo.
                // Interpola volumen música gradualmente.
                mainMusicAudioSource.volume = Mathf.Lerp(initialMusicVolume, initialMusicVolume * loweredMusicVolumeMultiplier, progress);
            }
            else if (!isGameOver)
            { // Encima del umbral y no es game over.
              // Restaura volumen música al original.
                mainMusicAudioSource.volume = initialMusicVolume;
            }
            // Si es Game Over, el volumen se quedará como estaba.
        }
    }

    // --- Métodos Públicos ---

    // Añade vida (llamado externamente, p.ej., por consumibles).
    public void AddHealth(float amountToAdd)
    {
        if (isGameOver || amountToAdd <= 0) return; // No añadir si muerto o cantidad inválida.
        float previousHealth = currentHealth;
        if (currentHealth < maxHealth) // Solo añade si no está al máximo.
        {
            currentHealth += amountToAdd;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Limita a maxHealth.
            if (currentHealth != previousHealth) healthChangedThisFrame = true; // Marca para actualizar UI.
        }
    }

    // Activa el efecto Andatti (invulnerabilidad temporal).
    public void ConsumeAndatti()
    {
        if (isGameOver || andatti) return; // No activar si muerto o ya activo.
        if (!isGameOver) andatti = true;
        if (andatti) Invoke(nameof(ResetAndatti), 5f); // Desactivar después de 5 segundos.
    }

    // --- Actualizaciones de UI ---

    // Actualiza la barra de vida visual.
    private void UpdateHealthBar()
    {
        if (healthBarFill != null && maxHealth > 0)
        {
            healthBarFill.fillAmount = Mathf.Clamp01(currentHealth / maxHealth); // Ajusta el fillAmount (0 a 1).
        }
    }

    // Actualiza el texto del puntaje.
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + Mathf.FloorToInt(currentScore).ToString(); // Muestra puntaje entero.
        }
    }

    // --- Secuencia de Game Over ---

    // Inicia el fin del juego, guarda puntaje, llama API, muestra efectos y carga escena final.
    private void TriggerGameOverSequence()
    {
        // Evita ejecución múltiple.
        if (isGameOver) return;
        isGameOver = true; // Marca juego como terminado.


        // --- 1. Calcula y guarda puntaje (localmente primero) ---
        int finalScoreInt = Mathf.FloorToInt(currentScore); // Puntaje final entero.
        PlayerPrefs.SetInt("LastScore", finalScoreInt); // Guarda en PlayerPrefs (para escena GameOver/respaldo).
        PlayerPrefs.Save(); // Asegura guardado.


        // --- 2. Intenta enviar puntaje a la API ---
        // Verifica usuario logueado (UserManager) y URL de API.
        if (UserManager.Instance != null && UserManager.Instance.CurrentUserId.HasValue && !string.IsNullOrEmpty(saveScoreApiUrl))
        {
            int userId = UserManager.Instance.CurrentUserId.Value; // Obtiene ID usuario.
            // Inicia Coroutine para enviar datos a API asíncronamente.
            StartCoroutine(APIScoreSender.SendScore(saveScoreApiUrl, userId, currentGameId, finalScoreInt));
        }
        else
        {
            Debug.LogWarning("No se enviará puntaje a API (Falta UserID/UserManager/URL).");
        }
        // --- Fin Llamada API ---

        // --- 3. Efectos inmediatos de Game Over ---
        if (screamerImageObject != null) screamerImageObject.SetActive(true); // Muestra imagen screamer.
        if (audioSource != null && screamerSoundClip != null) audioSource.PlayOneShot(screamerSoundClip); // Reproduce sonido screamer.
        if (heartbeatAudioSource != null && heartbeatAudioSource.isPlaying) heartbeatAudioSource.Stop(); // Detiene latido.
        if (mainMusicAudioSource != null) mainMusicAudioSource.Stop(); // Detiene música principal.

        // --- 4. Pausa el juego ---
        // Detiene tiempo del juego (física, Update, animaciones basadas en tiempo).
        Time.timeScale = 0f;

        // --- 5. Carga escena final después de delay ---
        // Inicia Coroutine que espera tiempo real antes de cargar escena Game Over.
        StartCoroutine(LoadGameOverAfterDelay(3.0f)); // Espera 3 segundos reales.
    } 

    // Coroutine para esperar tiempo real y cargar escena.
    private IEnumerator LoadGameOverAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // Espera tiempo real.
        Time.timeScale = 1f; 
        SceneManager.LoadScene("HR_GameOver"); // Carga escena final.
    }

    private void ResetAndatti()
    {
        andatti = false;
    }

} 