using UnityEngine;
using UnityEngine.SceneManagement;


public class MMenuScene : MonoBehaviour
{
    public void StartToPlay()
    {
        SceneManager.LoadScene("Menu_Main");
    }

    public void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        //Application.Quit();
    }
}
