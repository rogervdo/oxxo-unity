using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{

    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;

    // Pausa el juego y muestra el menú de pausa.
    public void Pausa()
    {
        Time.timeScale = 0f; // Detiene el tiempo del juego.
        botonPausa.SetActive(false); // Oculta el botón de pausa.
        menuPausa.SetActive(true); // Muestra el panel del menú de pausa.
    }

    // Reanuda el juego y oculta el menú de pausa.
    public void Reanudar()
    {
        Time.timeScale = 1f; // Reanuda el tiempo del juego.
        botonPausa.SetActive(true); // Muestra el botón de pausa.
        menuPausa.SetActive(false); // Oculta el panel del menú de pausa.
    }

    // Reinicia la escena actual.
    public void Reiniciar()
    {
        Time.timeScale = 1f; // Asegura que el tiempo esté corriendo antes de cargar.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recarga la escena activa.
    }

    // Vuelve al menú principal.
    public void Cerrar()
    {
        Time.timeScale = 1f; // Asegura que el tiempo esté corriendo.
        SceneManager.LoadScene("Menu_Main"); // Carga la escena del menú principal.
    }
} 