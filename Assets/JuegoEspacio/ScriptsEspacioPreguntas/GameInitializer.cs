using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameObject canvasInstrucciones;
    public GameObject uiPanel;

    void Start()
    {
        // Pausa el juego al iniciar
        Time.timeScale = 0f;

        // Muestra solo el canvas de instrucciones
        if (canvasInstrucciones != null)
            canvasInstrucciones.SetActive(true);
        
        if (uiPanel != null)
            uiPanel.SetActive(false);
    }

    public void IniciarJuego()
    {
        // Reanuda el juego
        Time.timeScale = 1f;

        // Oculta instrucciones y muestra el juego
        if (canvasInstrucciones != null)
            canvasInstrucciones.SetActive(false);
        
        if (uiPanel != null)
            uiPanel.SetActive(true);
    }
}
