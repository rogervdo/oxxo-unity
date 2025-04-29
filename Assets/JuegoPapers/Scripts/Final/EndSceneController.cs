using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndSceneController : MonoBehaviour
{
    public Text puntajeTexto;             // Texto para mostrar el puntaje final obtenido
    public Text mejorPuntajeTexto;         // Texto para mostrar el mejor puntaje histórico
    public Text nuevoRecordTexto;          // Texto opcional para "¡Nuevo Mejor Puntaje!"
    public string escenaInicial = "StartScreen"; // Nombre del menú principal

    void Start()
    {
        int puntaje = PlayerPrefs.GetInt("PuntajeFinal", 0);
        int mejorPuntaje = PlayerPrefs.GetInt("PuntajeMaximoHistorial", 0);
        int nuevoRecord = PlayerPrefs.GetInt("NuevoRecord", 0);

        puntajeTexto.text = "Puntaje Obtenido: " + puntaje.ToString() + " pts";
        mejorPuntajeTexto.text = "Tu mejor puntaje: " + mejorPuntaje.ToString() + " pts";

        if (nuevoRecord == 1)
        {
            nuevoRecordTexto.gameObject.SetActive(true);
            nuevoRecordTexto.text = "🎉 ¡Nuevo Mejor Puntaje! 🎉";
        }
        else
        {
            nuevoRecordTexto.gameObject.SetActive(false);
        }
    }

    public void RegresarAlInicio()
    {
        SceneManager.LoadScene(escenaInicial);
    }
}
