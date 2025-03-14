using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{
    public void PlayCut()
    {
        SceneManager.LoadScene("MainGame");
    }
}
