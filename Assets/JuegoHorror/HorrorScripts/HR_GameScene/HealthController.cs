using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // <--- Necesario para SceneManager
using System.Collections;       // <--- Necesario para IEnumerator (Coroutines)

[RequireComponent(typeof(AudioSource))] // Asegura que haya un AudioSource
public class HealthController : MonoBehaviour
{
    // --- Variables Públicas (Asignar en Inspector) ---
    public Image healthBarFill;
    public float maxHealth = 100f;
    public float decreaseRate = 3f;
    public GameObject screamerImageObject; // El GameObject de la imagen del screamer
    public AudioClip screamerSoundClip; // El sonido del screamer

    // --- Variables Internas ---
    private float currentHealth;
    private bool andatti = false;
    private bool healthChangedThisFrame = false;
    private bool isGameOver = false;
    private AudioSource audioSource; // Para reproducir el sonido

    void Start()
    {
        // Obtiene el AudioSource
        audioSource = GetComponent<AudioSource>();
        if(audioSource != null) {
            audioSource.playOnAwake = false;
        } else {
             Debug.LogError("HealthController necesita un componente AudioSource!", this);
        }

        // Inicialización normal
        currentHealth = maxHealth;
        andatti = false;
        isGameOver = false;
        if (screamerImageObject != null) {
            screamerImageObject.SetActive(false); // Asegura que esté oculto al inicio
        }
        UpdateHealthBar();
    }

    void Update()
    {
        if (isGameOver) return; // No hacer nada si ya terminó el juego

        // Descenso de vida
        if (!andatti) {
            float previousHealth = currentHealth;
            currentHealth -= decreaseRate * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            if (currentHealth != previousHealth) {
                healthChangedThisFrame = true;
            }
        }

        // Prueba con Espacio
        if (Input.GetKeyDown(KeyCode.Space)) {
           AddHealth(10f);
        }
    }

    void LateUpdate()
    {
        if (isGameOver) return;

        if (healthChangedThisFrame) {
            UpdateHealthBar();
            healthChangedThisFrame = false;

            // Comprueba si la vida llegó a cero
            if (currentHealth <= 0f) {
                TriggerGameOverSequence(); // Inicia la secuencia de fin
            }
        }
    }

    // Añade vida
    public void AddHealth(float amountToAdd) {
        if (isGameOver || amountToAdd <= 0) return;
        float previousHealth = currentHealth;
        if (currentHealth < maxHealth) {
            currentHealth += amountToAdd;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            if (currentHealth != previousHealth) {
                healthChangedThisFrame = true;
            }
        }
    }

    // Actualiza la barra visual
    private void UpdateHealthBar() {
        if (healthBarFill != null && maxHealth > 0) {
            healthBarFill.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        }
    }

    // Inicia la secuencia del screamer y Game Over
    private void TriggerGameOverSequence() {
        if (isGameOver) return; // Evita ejecución múltiple
        isGameOver = true;
        Debug.Log("¡Vida a cero! Iniciando secuencia Game Over...");

        // Muestra el Screamer
        if (screamerImageObject != null) {
            screamerImageObject.SetActive(true);
        }

        // Reproduce Sonido
        if (audioSource != null && screamerSoundClip != null) {
            audioSource.PlayOneShot(screamerSoundClip);
        }

        // Pausa el juego AHORA
        Time.timeScale = 0f;

        // --- NUEVO: Inicia la Coroutine para cambiar de escena ---
        StartCoroutine(LoadGameOverAfterDelay(3.0f)); // Llama a la coroutine con 3 segundos de espera
    }

    // --- NUEVO: Coroutine para esperar y cargar la escena ---
    private IEnumerator LoadGameOverAfterDelay(float delay)
    {
        Debug.Log($"Esperando {delay} segundos de tiempo real antes de cargar Game Over...");

        // Espera 'delay' segundos, ignorando la escala de tiempo (Time.timeScale)
        yield return new WaitForSecondsRealtime(delay);

        Debug.Log("Tiempo de espera finalizado. Restaurando TimeScale y cargando HR_GameOver...");

        // ¡¡MUY IMPORTANTE!! Restaurar la escala de tiempo antes de cargar la nueva escena.
        Time.timeScale = 1f;

        // Cargar la escena de Game Over por su nombre
        SceneManager.LoadScene("HR_GameOver");
    }
    // --- FIN Coroutine ---


    // --- Lógica Andatti ---
    public void ConsumeAndatti() {
        if (isGameOver || !andatti) { // No consumir si el juego terminó
             if(!isGameOver) andatti = true;
             if(andatti) Invoke(nameof(ResetAndatti), 5f);
        }
    }
    private void ResetAndatti() {
        andatti = false;
    }
    // --- Fin Lógica Andatti ---
}