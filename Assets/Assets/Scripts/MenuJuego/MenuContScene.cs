using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuContScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartToPlay()
    {
        SceneManager.LoadScene("Menu");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void startGame2()
    {
        SceneManager.LoadScene("HR_StartBuffer");
    }

    public void startGame3()
    {
        SceneManager.LoadScene("SpaceMenu");
    }
}
