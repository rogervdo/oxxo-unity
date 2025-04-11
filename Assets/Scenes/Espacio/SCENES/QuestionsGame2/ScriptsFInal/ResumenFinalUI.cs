using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResumenFinalUI : MonoBehaviour
{
    public Text textoPuntos;
    public Text textoPreguntas;
    public Button botonReintentar;
    public string nombreEscenaInicio = "Escena1"; // Cambia esto al nombre de tu primera escena

    void Start()
    {
        if (GameSessionManager.Instance != null)
        {
            int puntos = GameSessionManager.Instance.ObtenerPuntaje();
            int preguntas = GameSessionManager.Instance.preguntasRespondidas;

            textoPuntos.text = "Puntaje final: " + GameSessionManager.Instance.puntosPreguntas;
            textoPreguntas.text = "Preguntas correctas: " + GameSessionManager.Instance.respuestasCorrectas + " de 6";


        }
        else
        {
            textoPuntos.text = "Puntaje final: 0";
            textoPreguntas.text = "Preguntas correctas: 0";
        }

        if (botonReintentar != null)
        {
            botonReintentar.onClick.AddListener(ReiniciarJuego);
        }
    }

    public void ReiniciarJuego()
    {
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.ReiniciarPuntaje();
            GameSessionManager.Instance.ReiniciarPreguntas();
            GameSessionManager.Instance.ReiniciarVidas(); // solo si estás usando vidas
        }

        SceneManager.LoadScene(nombreEscenaInicio);
    }
}
