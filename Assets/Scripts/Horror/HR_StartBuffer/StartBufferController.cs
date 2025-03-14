using UnityEngine;
using UnityEngine.SceneManagement;
public class StartBufferController : MonoBehaviour
{
    // Inicia juego al presionarse boton "Play"
    public void StartGame()
    {
        SceneManager.LoadScene("HR_GameScene");
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
