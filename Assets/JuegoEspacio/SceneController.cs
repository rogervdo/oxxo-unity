using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void Menu()
    {
        SceneManager.LoadScene("SpaceSelection");
    }

    public void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }


    //Primer juego
    public void SpaceShipGame()
    {
        SceneManager.LoadScene("VJ_Nave");

    }

    public void Ganar_S()
    {
        SceneManager.LoadScene("Escena_Ganar_S");
    }

    public void Perder_S()
    {
        SceneManager.LoadScene("Escena_Perder_S");
    }



    //Segundo juego
    public void QuestionsGame()
    {
        SceneManager.LoadScene("ControlRoom_Q");

    }

    public void ControlRoom()
    {
        SceneManager.LoadScene("ControlRoom_Q");
    }

    public void EngineRoom()
    {
        SceneManager.LoadScene("EngineRoom_Q");
    }

    public void Perder_Q()
    {
        SceneManager.LoadScene("Escena_Perder_Q");
    }

    public void Ganar_Q()
    {
        SceneManager.LoadScene("Escena_Ganar_Q");
    }

    public void Regresar_M()
    {
        SceneManager.LoadScene("Menu_Main");
    }





}