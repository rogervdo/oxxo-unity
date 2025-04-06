using UnityEngine;
using UnityEngine.SceneManagement;

public class MGameScene : MonoBehaviour
{
    public void Win()
    {
        SceneManager.LoadScene("PP_Win");
    }
}
