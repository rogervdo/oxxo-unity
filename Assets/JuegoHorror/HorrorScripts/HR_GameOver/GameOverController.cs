using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{

    // Inicia juego al presionarse boton "reiniciar"

    public void StartGame()
    {
        SceneManager.LoadScene("HR_StartBuffer");
    }

    public void getMenu()
    {
        SceneManager.LoadScene("Menu_Main");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
