using UnityEngine;
using UnityEngine.SceneManagement;

public class TestButtonController : MonoBehaviour
{
    public int isWin = 0;

    public void WinGame()
    {
        isWin = 1;
        PlayerPrefs.SetInt("isWin", isWin);
        SceneManager.LoadScene("HR_GameOver");
    }

    public void LoseGame()
    {
        isWin = 0;
        PlayerPrefs.SetInt("isWin", isWin);
        SceneManager.LoadScene("HR_GameOver");
    }

    public void GoHome()
    {
        SceneManager.LoadScene("HR_StartBuffer");
    }
}
