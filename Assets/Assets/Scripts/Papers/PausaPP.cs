using UnityEngine;
using UnityEngine.SceneManagement;

public class PausaPP : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;

    public void Pausa()
    {
        Time.timeScale = 0f;
        botonPausa.SetActive(false); //para que se activen y desaparezcan
        menuPausa.SetActive(true);
    }

    public void Reanudar()
    {
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
    }

    //btn reiniciar escala de tiempo y recragar escena
    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //mandar a llamar a las escenas de los menus 
    public void Cerrar()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void Skip()
    {
        SceneManager.LoadScene("MainGame");
    }

}
