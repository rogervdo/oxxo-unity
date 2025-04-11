using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour 
{
    public GameObject panelInstruccionesJuego; 

    void Start()
    {
        // Asegura que el panel de instrucciones esté oculto al iniciar la escena del juego.
        if (panelInstruccionesJuego != null)
        {
            panelInstruccionesJuego.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Referencia a 'panelInstruccionesJuego' no asignada en GameUIManager. El botón de ayuda no funcionará.", this);
        }


    }

    // Método para ser llamado por el botón de ABRIR (i) en la escena de juego
    public void AbrirPanelInstruccionesJuego()
    {
        if (panelInstruccionesJuego != null)
        {
            panelInstruccionesJuego.SetActive(true);
            Time.timeScale = 0f;
        }
    }


    public void CerrarPanelInstruccionesJuego()
    {
        if (panelInstruccionesJuego != null)
        {
            panelInstruccionesJuego.SetActive(false);

            Time.timeScale = 1f;
        }
    }

} 