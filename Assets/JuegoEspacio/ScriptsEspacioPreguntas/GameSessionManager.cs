using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance;

    public int vidasActuales = 3;
    public int vidasMaximas = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persistente entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🔴 VIDAS
    public void ReiniciarVidas()
    {
        vidasActuales = vidasMaximas;
    }

    public void QuitarVida()
    {
        vidasActuales--;
    }

    public int ObtenerVidas()
    {
        return vidasActuales;
    }

    // 🟢 PUNTOS
    public int puntosTotales = 0;

    public void AgregarPuntos(int puntos)
    {
        puntosTotales += puntos;
    }

    public int ObtenerPuntaje()
    {
        return puntosTotales;
    }

    public void ReiniciarPuntaje()
    {
        puntosTotales = 0;
    }

    // 🟡 PREGUNTAS RESPONDIDAS (correctas o no)
    public int preguntasRespondidas = 0;

    public void AumentarPreguntasRespondidas()
    {
        preguntasRespondidas++;
    }

    public void ReiniciarPreguntas()
    {
        preguntasRespondidas = 0;
    }

    public bool HaGanado()
    {
        return preguntasRespondidas >= 6;
    }

    // ✅ NUEVO: RESPUESTAS CORRECTAS
    public int respuestasCorrectas = 0;

    public int puntosPreguntas = 0;

public void AgregarPuntosPreguntas(int puntos)
{
    puntosPreguntas += puntos;
}

public void ReiniciarPuntosPreguntas()
{
    puntosPreguntas = 0;
}

public void AumentarRespuestasCorrectas()
{
    respuestasCorrectas++;
}


}
