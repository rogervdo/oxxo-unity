using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConsumableController : MonoBehaviour
{

    public Image[] consumables;
    // Sprites para los estados 'usado' y 'disponible'.
    public Sprite graySprite;
    public Sprite yellowSprite;
    // Referencia al controlador de salud del jugador.
    private HealthController healthController;
    // Cantidad de salud restaurada por un consumible.
    public float healthPerConsumable = 25f;

    void Start()
    {

        healthController = FindFirstObjectByType<HealthController>();
        if (healthController == null)
        {

        }

    }


    public void UseConsumable0() { HandleConsumableClicked(0); }
    public void UseConsumable1() { HandleConsumableClicked(1); }
    public void UseConsumable2() { HandleConsumableClicked(2); }


    // Maneja la lógica cuando se hace clic/usa una ranura de consumible.
    private void HandleConsumableClicked(int index)
    {
        // Valida el índice proporcionado.
        if (index < 0 || index >= consumables.Length || consumables[index] == null)
        {

            return;
        }
        // Asegura que HealthController esté disponible.
        if (healthController == null)
        {

            return;
        }

        Image clickedImage = consumables[index];
        // Obtiene el componente Button asociado con la Image clicada.
        Button clickedButton = clickedImage.GetComponent<Button>();
        if (clickedButton == null)
        {
            return;
        }

        // Comprueba si el consumible ya está usado o no es interactuable.
        if (!clickedButton.interactable || clickedImage.sprite == graySprite)
        {
            return;
        }

        // 1. Aplica la restauración de salud a través de HealthController.
        healthController.AddHealth(healthPerConsumable);


        // 2. Consume visualmente: Cambia el sprite al estado 'usado'.
        clickedImage.sprite = graySprite;

        // 3. Desactiva la interacción con el Button.
        clickedButton.interactable = false;
    }

    // Añade una carga de consumible, encontrando la primera ranura disponible.
    public void GainConsumable()
    {

        Button buttonToReactivate = null;
        // Itera a través de las ranuras de consumibles.
        for (int i = 0; i < consumables.Length; i++)
        {
            if (consumables[i] != null)
            {
                // Encuentra el componente Button
                buttonToReactivate = consumables[i].GetComponent<Button>();
                // Comprueba si esta el slot está actualmente usada
                if (buttonToReactivate != null && !buttonToReactivate.interactable)
                {
                    // Reactiva el slot
                    consumables[i].sprite = yellowSprite; // Establece sprite a 'disponible'.
                    buttonToReactivate.interactable = true; // Habilita interacción del botón.

                    return; 
                }
            }
        }

    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.A)) { GainConsumable(); }
    }
} 