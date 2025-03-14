using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public Image healthBarFill;
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float decreaseRate = 3f;
    public float healAmount = 10f;
    public bool andatti;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        andatti = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!andatti)
        {
            currentHealth -= decreaseRate * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Heal();
        }
    }

    private void Heal()
    {
        float healAmount = maxHealth * 0.1f; // 10% of max health
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBarFill.fillAmount = currentHealth / maxHealth;
        Debug.Log("Healed! Current Health: " + currentHealth);
    }

    public void ConsumeAndatti()
    {
        andatti = true;
        Invoke(nameof(ResetAndatti), 5f);
    }

    private void ResetAndatti()
    {
        andatti = false;
    }
}
