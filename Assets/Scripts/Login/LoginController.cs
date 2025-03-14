using UnityEngine;
using UnityEngine.SceneManagement;
public class LoginController : MonoBehaviour
{
    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
