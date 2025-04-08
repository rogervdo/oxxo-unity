using UnityEngine;
using UnityEngine.UI;

// Asegura que el GameObject tenga un AudioSource adjunto.
// Si no lo tiene al añadir este script, Unity lo añadirá automáticamente.
[RequireComponent(typeof(AudioSource))]
public class HealthController : MonoBehaviour
{
    // --- Variables de Configuración ---
    public Image healthBarFill;
    public float maxHealth = 100f;
    public float decreaseRate = 3f;
    public GameObject screamerImageObject;
    public AudioClip screamerSoundClip; // <<-- NUEVO: Asigna tu archivo de sonido aquí en el Inspector.

    // --- Variables Internas ---
    private float currentHealth;
    private bool andatti = false;
    private bool healthChangedThisFrame = false;
    private bool isGameOver = false;
    private AudioSource audioSource; // <<-- NUEVO: Referencia al componente AudioSource.

    void Start()
    {
        // Obtiene la referencia al componente AudioSource de este GameObject.
        audioSource = GetComponent<AudioSource>();
        // Configuración inicial opcional del AudioSource
        if(audioSource != null) {
            audioSource.playOnAwake = false; // No sonar al inicio
            // audioSource.spatialBlend = 0f; // Sonido 2D
        } else {
             Debug.LogError("HealthController no encontró el componente AudioSource requerido!", this);
        }


        currentHealth = maxHealth;
        andatti = false;
        isGameOver = false;

        if (screamerImageObject != null) {
            screamerImageObject.SetActive(false);
        } else {
            Debug.LogWarning("Referencia a 'screamerImageObject' no asignada.", this);
        }
        UpdateHealthBar();
    }

    void Update()
    {
        if (isGameOver) return;

        if (!andatti) {
            float previousHealth = currentHealth;
            currentHealth -= decreaseRate * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            if (currentHealth != previousHealth) {
                healthChangedThisFrame = true;
            }
        }

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

            if (currentHealth <= 0f) {
                TriggerGameOverSequence();
            }
        }
    }

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

    private void UpdateHealthBar() {
        if (healthBarFill != null && maxHealth > 0) {
            healthBarFill.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        }
    }

    private void TriggerGameOverSequence() {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("¡Vida a cero! Mostrando Screamer y reproduciendo sonido...");

        // Activa imagen
        if (screamerImageObject != null) {
            screamerImageObject.SetActive(true);
        }

        // --- REPRODUCIR SONIDO ---
        // Comprueba si tenemos AudioSource y un clip asignado.
        if (audioSource != null && screamerSoundClip != null) {
            // Reproduce el clip una vez. No interrumpe otros sonidos
            // que pudiera estar reproduciendo este AudioSource (si los hubiera).
            audioSource.PlayOneShot(screamerSoundClip);
        } else if (screamerSoundClip == null) {
             Debug.LogWarning("No se asignó 'screamerSoundClip' en el Inspector de HealthController.", this);
        }
        // --- FIN REPRODUCIR SONIDO ---


        // Pausar juego
        Time.timeScale = 0f;

        // Otras acciones opcionales...
    }


    // --- Lógica Andatti (sin cambios) ---
    public void ConsumeAndatti() {
        if (isGameOver || !andatti) {
             if(!isGameOver) andatti = true;
             if(andatti) Invoke(nameof(ResetAndatti), 5f);
        }
    }
    private void ResetAndatti() {
        andatti = false;
    }
    // --- Fin Lógica Andatti ---
}