using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConsumableController : MonoBehaviour
{

    public Image[] consumables;
    public Sprite graySprite;
    public Sprite yellowSprite;
    private HealthController healthController;


    public float healthPerConsumable = 25f;

    void Start()
    {
        healthController = FindFirstObjectByType<HealthController>();
        if (healthController != null)
        {
            // --> AÑADE ESTA LÍNEA <--
            Debug.Log($"ConsumableController encontró HealthController en el GameObject: {healthController.gameObject.name}", this);
        }
        else
        {
            Debug.LogError("¡ConsumableController no pudo encontrar HealthController!", this);
        }
        // ... resto del Start ...
    }

    // Método AHORA PÚBLICO para llamarlo desde el Inspector.
    // Necesitamos saber qué índice se clickeó. Hay varias formas,
    // la más simple es crear un método por cada índice si son pocos.
    // O usar un script intermedio (más avanzado).
    // -> OPCIÓN SIMPLE: Un método por botón (si tienes pocos)
    public void UseConsumable0() { HandleConsumableClicked(0); }
    public void UseConsumable1() { HandleConsumableClicked(1); }
    public void UseConsumable2() { HandleConsumableClicked(2); }
    // Añade más si tienes más consumibles...

    // La lógica principal sigue siendo privada, llamada por los métodos públicos.
    private void HandleConsumableClicked(int index)
    {
        // --> MANTÉN ESTE DEBUG LOG PARA PRUEBAS <--
        Debug.Log($"CLICK DETECTADO en HandleConsumableClicked para el índice: {index}");
        // ------------------------------------------

        // Validaciones básicas
        if (index < 0 || index >= consumables.Length || consumables[index] == null)
        {
            Debug.LogError($"Índice inválido ({index}) pasado a HandleConsumableClicked.");
            return;
        }
        if (healthController == null)
        {
            Debug.LogError("No se puede procesar, falta HealthController.");
            return;
        }

        Image clickedImage = consumables[index];
        // Necesitamos obtener el botón correspondiente para desactivarlo
        Button clickedButton = consumables[index].GetComponent<Button>();
        if (clickedButton == null)
        {
            Debug.LogError($"No se encontró el componente Button en el índice {index}.");
            return;
        }


        // Verificar si ya se usó
        if (!clickedButton.interactable || clickedImage.sprite == graySprite)
        {
            Debug.Log($"Consumible {index} ya usado o no interactuable.");
            return;
        }

        // 1. Añadir vida
        healthController.AddHealth(healthPerConsumable);
        Debug.Log($"Añadidos {healthPerConsumable} de vida desde {index}.");

        // 2. Consumir visualmente
        clickedImage.sprite = graySprite;

        // 3. Desactivar botón
        clickedButton.interactable = false;
    }

    // GainConsumable y Update se mantienen igual por ahora.
    public void GainConsumable()
    {
        Debug.Log("GainConsumable() llamado.");
        Button buttonToReactivate = null; // Necesitamos encontrar el botón también
        for (int i = 0; i < consumables.Length; i++)
        {
            if (consumables[i] != null)
            {
                buttonToReactivate = consumables[i].GetComponent<Button>();
                if (buttonToReactivate != null && !buttonToReactivate.interactable)
                {
                    consumables[i].sprite = yellowSprite;
                    buttonToReactivate.interactable = true;
                    Debug.Log($"Consumible rellenado en índice {i}!");
                    return;
                }
            }
        }
        Debug.Log("GainConsumable: No slots vacíos.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) { GainConsumable(); }
    }
}