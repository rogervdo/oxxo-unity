using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public Text winLoseText;
    // Inicia juego al presionarse boton "Play"

    void Start()
    {
        if (PlayerPrefs.GetInt("isWin") == 1)
        {
            winLoseText.text = "Ganaste!";
        }
        else if (PlayerPrefs.GetInt("isWin") == 0)
        {
            winLoseText.text = "Perdiste!";
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
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
