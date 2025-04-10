using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScene : MonoBehaviour
{

    public void StartToPlay()
    {
        SceneManager.LoadScene("CutScene");
    }

    public void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        //Application.Quit();
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu_Main");
    }

}
