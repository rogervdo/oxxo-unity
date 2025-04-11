using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBufferController : MonoBehaviour
{
    // Inicia juego al presionarse boton "Play"
    public void StartGame()
    {
        Time.timeScale = 1f; // Asegura que el tiempo corra normal al empezar
        SceneManager.LoadScene("HR_GameScene");
    }

    // Regresa al menú principal
    public void GetMenu()
    {
        Time.timeScale = 1f; // Asegura tiempo normal
        SceneManager.LoadScene("Menu_Main");
    }

    // Sale del juego/editor al presionarse "Exit".
    public void ExitGame()
    {
#if UNITY_EDITOR
        // Si está en el Editor de Unity, detiene la reproducción
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Si es una build compilada, cierra la aplicación
        Application.Quit();
#endif
    }
}