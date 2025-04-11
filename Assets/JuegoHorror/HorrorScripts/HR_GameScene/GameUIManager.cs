using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{

    public GameObject panelInstruccionesJuego;

    void Start()
    {
        // Asegura que el panel de instrucciones esté oculto cuando comienza la escena del juego.
        if (panelInstruccionesJuego != null)
        {
            panelInstruccionesJuego.SetActive(false);
        }
    }

    // Método llamado por el botón 'Abrir Instrucciones'.
    public void AbrirPanelInstruccionesJuego()
    {
        if (panelInstruccionesJuego != null)
        {
            panelInstruccionesJuego.SetActive(true); // Muestra el panel.
            Time.timeScale = 0f; // Pausa el tiempo del juego.
        }
    }

    // Método llamado por el botón 'Cerrar Instrucciones'.
    public void CerrarPanelInstruccionesJuego()
    {
        if (panelInstruccionesJuego != null)
        {
            panelInstruccionesJuego.SetActive(false); // Oculta el panel.
            Time.timeScale = 1f; // Reanuda el tiempo del juego.
        }
    }

} 