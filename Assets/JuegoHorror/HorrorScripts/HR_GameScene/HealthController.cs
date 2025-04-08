using UnityEngine;
using UnityEngine.UI;
using System.Text; // Para StringBuilder

public class HealthController : MonoBehaviour
{
    [Header("Settings")] // Puedes quitar [Header] si prefieres
    public Image healthBarFill;
    public float maxHealth = 100f;
    public float decreaseRate = 3f;

    [Header("State (Read Only)")] // Puedes quitar [Header] si prefieres
    [SerializeField]
    private float currentHealth = 100f;
    [SerializeField]
    private bool andatti = false;

    // Bandera y Log Builder para depuración
    private bool healthChangedThisFrame = false;
    private StringBuilder debugLogBuilder = new StringBuilder();

    void Start()
    {
        // --- LOG INICIO ---
        debugLogBuilder.Clear(); // Limpia por si acaso
        debugLogBuilder.AppendLine($"--- HealthController Start Frame {Time.frameCount} ---");
        currentHealth = maxHealth;
        andatti = false;
        debugLogBuilder.AppendLine($"Start: Inicializando currentHealth={currentHealth}, maxHealth={maxHealth}");
        if (healthBarFill != null) {
            UpdateHealthBar(); // Configuración visual inicial
        } else {
            debugLogBuilder.AppendLine("Start ERROR: healthBarFill NO está asignado!");
            Debug.LogError("HealthController Start: healthBarFill is NOT assigned in the Inspector!", this); // Error directo también
        }
        healthChangedThisFrame = false;
        Debug.Log(debugLogBuilder.ToString()); // Imprime log de inicio
        // --- FIN LOG INICIO ---
    }

    void Update()
    {
        // --- LOG INICIO UPDATE ---
        debugLogBuilder.Clear(); // Limpia para el log de este fotograma
        debugLogBuilder.AppendLine($"--- HealthController Update Frame {Time.frameCount} (Time={Time.time:F3}, Delta={Time.deltaTime:F4}) ---");
        debugLogBuilder.AppendLine($"Update Start: currentHealth={currentHealth:F3}, andatti={andatti}, healthChangedThisFrame={healthChangedThisFrame}");
        // --- FIN LOG INICIO UPDATE ---

        // --- Lógica de Descenso de Vida ---
        if (!andatti) {
            float previousHealth = currentHealth;
            float decreaseAmount = decreaseRate * Time.deltaTime;
            if (currentHealth > 0) {
                currentHealth -= decreaseAmount;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                if (currentHealth != previousHealth) {
                    debugLogBuilder.AppendLine($"Update Decrease: Disminuyó en {decreaseAmount:F4}. Vida {previousHealth:F3} -> {currentHealth:F3}. Marcando cambio.");
                    healthChangedThisFrame = true;
                } else {
                     // debugLogBuilder.AppendLine($"Update Decrease: Calculado {decreaseAmount:F4}, pero vida no cambió tras clamp. Vida={currentHealth:F3}");
                }
            } else {
                 // debugLogBuilder.AppendLine($"Update Decrease: Saltado, vida ya en 0.");
            }
        } else {
             debugLogBuilder.AppendLine($"Update Decrease: Saltado (andatti={andatti})");
        }
        // --- Fin Lógica Descenso ---

        // --- Prueba con Barra Espaciadora ---
        if (Input.GetKeyDown(KeyCode.Space)) {
           debugLogBuilder.AppendLine($"Update: ¡Barra espaciadora presionada!");
           AddHealth(10f); // AddHealth añadirá su propio log
        }
        // --- Fin Prueba ---

        // No imprimir log aquí, esperar a LateUpdate
    }

    // Se llama después de todos los Updates del fotograma.
    void LateUpdate()
    {
        // --- LOG INICIO LATEUPDATE ---
        // Añade al log que se construyó durante Update
        debugLogBuilder.AppendLine($"--- HealthController LateUpdate Frame {Time.frameCount} ---");
        debugLogBuilder.AppendLine($"LateUpdate Start: healthChangedThisFrame={healthChangedThisFrame}, currentHealth={currentHealth:F3}");
        // --- FIN LOG INICIO LATEUPDATE ---

        if (healthChangedThisFrame) {
            debugLogBuilder.AppendLine($"LateUpdate: Cambio detectado, llamando a UpdateHealthBar.");
            UpdateHealthBar(); // UpdateHealthBar añadirá sus logs
            healthChangedThisFrame = false; // Resetea bandera DESPUÉS de actualizar
        } else {
             debugLogBuilder.AppendLine($"LateUpdate: No se detectó cambio, saltando actualización UI.");
        }

        // Imprime el log completo acumulado para este fotograma y limpia para el siguiente
        Debug.Log(debugLogBuilder.ToString());
        // debugLogBuilder.Clear(); // Es más seguro limpiar al inicio del Update
    }

    // Añade vida. Llamado por otros scripts.
    public void AddHealth(float amountToAdd)
    {
        // --- LOG ADDHEALTH ---
        // Añade al log del fotograma actual
        debugLogBuilder.AppendLine($"AddHealth({amountToAdd:F1}) llamado. Vida actual ANTES de añadir = {currentHealth:F3}");
        // --- FIN LOG ADDHEALTH ---

        if (amountToAdd <= 0) {
             debugLogBuilder.AppendLine($"AddHealth: Cantidad <= 0, abortando.");
             return;
        }

        float previousHealth = currentHealth;
        if (currentHealth < maxHealth) {
            currentHealth += amountToAdd;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (currentHealth != previousHealth) {
                debugLogBuilder.AppendLine($"AddHealth: Vida cambió {previousHealth:F3} -> {currentHealth:F3}. Marcando cambio.");
                healthChangedThisFrame = true; // Marca que hubo cambio
            } else {
                // debugLogBuilder.AppendLine($"AddHealth: Vida no cambió tras añadir y clampear. Prev={previousHealth:F3}, Curr={currentHealth:F3}");
            }
        } else {
             debugLogBuilder.AppendLine($"AddHealth: Saltado, vida ya al máximo ({currentHealth:F3}).");
        }
    }

    // Actualiza la barra de vida visual. Llamado por Start y LateUpdate.
    private void UpdateHealthBar()
    {
        // --- LOG UPDATEHEALTHBAR ---
        // Añade al log del fotograma actual
        debugLogBuilder.Append($"UpdateHealthBar: "); // Usa Append para continuar la línea
        // --- FIN LOG UPDATEHEALTHBAR ---

        if (healthBarFill == null) {
             debugLogBuilder.AppendLine($"FALLÓ - ¡healthBarFill es NULL!");
             return;
        }
        if (maxHealth <= 0) {
            debugLogBuilder.AppendLine($"FALLÓ - ¡maxHealth <= 0 ({maxHealth:F1})!");
            healthBarFill.fillAmount = 0f;
            return;
        }

        float fillValue = Mathf.Clamp01(currentHealth / maxHealth);
        debugLogBuilder.AppendLine($"Calculado fillValue={fillValue:F3}. Intentando asignar a '{healthBarFill.gameObject.name}' (VidaActual={currentHealth:F3})");

        // Asigna solo si el valor es diferente (micro-optimización)
        if (healthBarFill.fillAmount != fillValue) {
             healthBarFill.fillAmount = fillValue;
        } else {
             // debugLogBuilder.Append(" (Valor sin cambios, asignación saltada)");
        }
    }

    // --- Lógica Andatti (sin cambios, logs simplificados) ---
    public void ConsumeAndatti() {
        if (!andatti) {
             andatti = true;
             Debug.Log($"ConsumableController -> ConsumeAndatti llamado en Frame {Time.frameCount}. Pausando descenso.");
             Invoke(nameof(ResetAndatti), 5f);
        }
    }
    private void ResetAndatti() {
        andatti = false;
         Debug.Log($"HealthController -> ResetAndatti llamado en Frame {Time.frameCount}. Reanudando descenso.");
    }
    // --- Fin Lógica Andatti ---
}