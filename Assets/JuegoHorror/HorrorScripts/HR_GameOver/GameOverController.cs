using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{

    // Inicia juego al presionarse boton "Play"

    public void StartGame()
    {
        SceneManager.LoadScene("HR_StartBuffer");
    }

    public void getMenu()
    {
        SceneManager.LoadScene("Menu_Main");
    }

    // Sale del juego al presionarse "Exit". Preprocesador para diferenciar editor unity de juego
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
