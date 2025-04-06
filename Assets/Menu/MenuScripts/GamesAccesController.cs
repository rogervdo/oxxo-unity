using UnityEngine;
using UnityEngine.SceneManagement;

public class GamesAccesController : MonoBehaviour
{
    public void StartHorror()
    {
        SceneManager.LoadScene("--");
    }

    public void StartToPlay()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void StartSpace()
    {
        SceneManager.LoadScene("--");
    }

}
