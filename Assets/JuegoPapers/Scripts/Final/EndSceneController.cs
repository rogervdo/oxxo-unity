using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndSceneController : MonoBehaviour
{
    public Text puntajeTexto;
    public string escenaInicial = "StartScreen"; // Cambia al nombre real de tu escena inicial

    void Start()
    {
        int puntaje = PlayerPrefs.GetInt("PuntajeFinal", 0);
        puntajeTexto.text = "Puntaje Obtenido: " + puntaje.ToString() + " pts.";
    }

    public void RegresarAlInicio()
    {
        SceneManager.LoadScene(escenaInicial);
    }
}