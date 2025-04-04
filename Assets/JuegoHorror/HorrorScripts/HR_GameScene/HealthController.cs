using UnityEngine;
using UnityEngine.UI;
using System.Text; // For StringBuilder

public class HealthController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The UI Image component used for the health bar fill.")]
    public Image healthBarFill;
    [Tooltip("Maximum health value.")]
    public float maxHealth = 100f;
    [Tooltip("Rate at which health decreases per second.")]
    public float decreaseRate = 3f;

    [Header("State (Read Only)")]
    [SerializeField] // Show private variable in Inspector (read-only)
    private float currentHealth = 100f;
    [SerializeField]
    private bool andatti = false;

    // Flag to track if health changed this frame, triggering LateUpdate UI refresh
    private bool healthChangedThisFrame = false;
    private StringBuilder debugLogBuilder = new StringBuilder(); // To reduce log spam

    void Start()
    {
        debugLogBuilder.AppendLine($"--- HealthController Start Frame {Time.frameCount} ---");
        currentHealth = maxHealth;
        andatti = false;
        debugLogBuilder.AppendLine($"Start: Initializing currentHealth={currentHealth}, maxHealth={maxHealth}");
        // Ensure the image reference is valid before updating
        if (healthBarFill != null)
        {
            UpdateHealthBar(); // Initial visual setup only if reference is valid
        }
        else
        {
            Debug.LogError("HealthController Start: healthBarFill is NOT assigned in the Inspector!", this);
        }
        healthChangedThisFrame = false; // Start with no change flagged
        Debug.Log(debugLogBuilder.ToString()); // Print accumulated start log
        debugLogBuilder.Clear();
    }

    void Update()
    {
        debugLogBuilder.Clear(); // Clear at start of Update for this frame's log
        debugLogBuilder.AppendLine($"--- HealthController Update Frame {Time.frameCount} (Time={Time.time:F3}, Delta={Time.deltaTime:F4}) ---");
        debugLogBuilder.AppendLine($"Update Start: currentHealth={currentHealth:F3}, andatti={andatti}, healthChangedThisFrame={healthChangedThisFrame}");

        // --- Health Decrease Logic ---
        if (!andatti)
        {
            float previousHealth = currentHealth;
            float decreaseAmount = decreaseRate * Time.deltaTime;
            // Only decrease if currentHealth is above 0
            if (currentHealth > 0)
            {
                currentHealth -= decreaseAmount;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Clamp *after* decrease

                // Check if health actually changed
                if (currentHealth != previousHealth)
                {
                    debugLogBuilder.AppendLine($"Update Decrease: Decreased by {decreaseAmount:F4}. Health {previousHealth:F3} -> {currentHealth:F3}. Flagging change.");
                    healthChangedThisFrame = true;
                }
                else if (previousHealth == 0 && currentHealth == 0) {
                     // Optional: Log only if it was already 0 and tried to decrease
                     // debugLogBuilder.AppendLine($"Update Decrease: Skipped decrease amount {decreaseAmount:F4}, health already at 0.");
                }
                 else {
                    // This case might happen if decreaseAmount is extremely small due to low deltaTime
                    debugLogBuilder.AppendLine($"Update Decrease: Decrease calculated ({decreaseAmount:F4}), but health value didn't change after clamp. Health={currentHealth:F3}");
                }
            }
            else {
                 // Already at 0, no need to decrease further
                 // debugLogBuilder.AppendLine($"Update Decrease: Skipped, health already at 0.");
            }
        } else {
             debugLogBuilder.AppendLine($"Update Decrease: Skipped (andatti={andatti})");
        }

        // --- Spacebar Test (for debugging) ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
           debugLogBuilder.AppendLine($"Update: Spacebar pressed!");
           // Temporarily make spacebar heal HUGE amount
           AddHealth(50f); // Use a large value like 50 for testing
        }

        // DO NOT print log here, wait until LateUpdate finishes for the frame
    }

    // --- LateUpdate for UI Synchronization ---
    // Runs after all Update functions are complete for the frame.
    void LateUpdate()
    {
        // Append LateUpdate start info to the log built during Update
        debugLogBuilder.AppendLine($"--- HealthController LateUpdate Frame {Time.frameCount} ---");
        debugLogBuilder.AppendLine($"LateUpdate Start: healthChangedThisFrame={healthChangedThisFrame}, currentHealth={currentHealth:F3}");
        if (healthChangedThisFrame)
        {
            debugLogBuilder.AppendLine($"LateUpdate: Change detected, calling UpdateHealthBar.");
            UpdateHealthBar(); // This will append its own log messages
            healthChangedThisFrame = false; // Reset flag *after* potentially updating
        } else {
             debugLogBuilder.AppendLine($"LateUpdate: No change detected, skipping UI update.");
        }

        // Print accumulated log for this entire frame (Update + LateUpdate) and clear
        Debug.Log(debugLogBuilder.ToString());
        // Ensure it's clear for the *next* frame's Update
        // debugLogBuilder.Clear(); // Clearing here might be too early if other LateUpdates log? Let's clear at start of Update instead.
    }

    /// <summary>
    /// Adds a specific amount to the current health/time.
    /// Clamps the value and flags that a change occurred for LateUpdate.
    /// Called externally (e.g., by ClickEffectSpawner).
    /// </summary>
    public void AddHealth(float amountToAdd)
    {
        // Log happens *within* Update or wherever AddHealth is called
        // Append to the current frame's log string
        debugLogBuilder.AppendLine($"AddHealth({amountToAdd:F1}) called. Current health before add = {currentHealth:F3}");
        if (amountToAdd <= 0) {
             debugLogBuilder.AppendLine($"AddHealth: Amount was <= 0, aborting.");
             return;
        }

        float previousHealth = currentHealth;
        // Only add health if not already at max
        if (currentHealth < maxHealth)
        {
            currentHealth += amountToAdd;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Clamp after adding

             // Check if the health value actually changed after clamping
            if (currentHealth != previousHealth)
            {
                debugLogBuilder.AppendLine($"AddHealth: Health changed {previousHealth:F3} -> {currentHealth:F3}. Flagging change.");
                healthChangedThisFrame = true; // Flag that a change happened
            }
            else
            {
                // This could happen if amountToAdd is very small or due to float precision near max
                debugLogBuilder.AppendLine($"AddHealth: Health value unchanged after adding & clamping. Previous={previousHealth:F3}, Current={currentHealth:F3}");
            }
        } else {
             debugLogBuilder.AppendLine($"AddHealth: Skipped adding health, already at max ({currentHealth:F3}).");
        }
    }

    /// <summary>
    /// Updates the fill amount of the health bar Image component.
    /// Called by Start and LateUpdate. Appends logs to the frame's StringBuilder.
    /// </summary>
    private void UpdateHealthBar()
    {
        // Log happens *within* LateUpdate context usually
        debugLogBuilder.Append($"UpdateHealthBar: "); // Append to existing log for the frame
        if (healthBarFill == null) {
             debugLogBuilder.AppendLine($"FAILED - healthBarFill is NULL!");
             // Optional: Try to find it dynamically as a last resort? Not recommended for performance.
             // healthBarFill = GetComponentInChildren<Image>(); // Example, adjust if needed
             // if(healthBarFill == null) return; // Still couldn't find it
             return; // Critical error, stop processing
        }
        if (maxHealth <= 0) {
            debugLogBuilder.AppendLine($"FAILED - maxHealth <= 0 ({maxHealth:F1})!");
            healthBarFill.fillAmount = 0f;
            return;
        }

        // Calculate fill amount (0.0 to 1.0) and set it
        float fillValue = Mathf.Clamp01(currentHealth / maxHealth);
        debugLogBuilder.AppendLine($"Setting fillAmount to {fillValue:F3} (current={currentHealth:F3}, max={maxHealth:F1}) on Image '{healthBarFill.gameObject.name}'");

        // Check if the value is actually different before assigning, minor optimization
        if (healthBarFill.fillAmount != fillValue) {
             healthBarFill.fillAmount = fillValue;
        } else {
             // Optional: Log if the value didn't need changing
             // debugLogBuilder.Append(" (Value unchanged, skipping assignment)");
        }
    }


    // --- Andatti Logic (unchanged, but added frame count to logs) ---
    public void ConsumeAndatti() {
        if (!andatti) {
             andatti = true;
             Debug.Log($"ConsumeAndatti called at Frame {Time.frameCount}. Pausing health decrease."); // Use Debug.Log directly for events
             Invoke(nameof(ResetAndatti), 5f);
        }
    }
    private void ResetAndatti() {
        andatti = false;
         Debug.Log($"ResetAndatti called at Frame {Time.frameCount}. Resuming health decrease."); // Use Debug.Log directly for events
    }
    // --- End Andatti Logic ---
}