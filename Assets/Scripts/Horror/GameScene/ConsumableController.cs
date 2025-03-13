using UnityEngine;
using UnityEngine.UI;

public class ConsumableController : MonoBehaviour
{
    public Image[] consumables;
    public Sprite graySprite, yellowSprite;
    private int currentIndex = 0;

    public Button consumableButton;
    public HealthController healthController;

    void Start()
    {
        healthController = FindFirstObjectByType<HealthController>();

        if (consumableButton != null)
        {
            consumableButton.onClick.AddListener(OnConsumableClick); // Adds listener in code
            Debug.Log("OnClick Listener Added!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GainConsumable();
        }
    }
    public void UseConsumable()
    {
        Debug.Log("UseConsumable()");
        if (currentIndex < consumables.Length)
        {
            consumables[currentIndex].sprite = graySprite;
            Debug.Log("Consumable used! Remaining: " + (consumables.Length - currentIndex - 1));
            currentIndex++;
            healthController.ConsumeAndatti();
        }
    }

    public void GainConsumable()
    {
        Debug.Log("GainConsumable()");
        if (currentIndex > 0)
        {
            currentIndex--;
            consumables[currentIndex].sprite = yellowSprite;
            Debug.Log("Consumable gained! Remaining: " + (consumables.Length - currentIndex - 1));
        }
    }

    public void OnConsumableClick()
    {
        Debug.Log("OnConsumableClick()");
        UseConsumable();
    }

}
