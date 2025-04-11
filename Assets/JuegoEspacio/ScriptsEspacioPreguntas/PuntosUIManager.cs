using UnityEngine;
using UnityEngine.UI;

public class PuntosUIManager : MonoBehaviour
{
    public Text textoPuntos;

    void Update()
    {
        if (GameSessionManager.Instance != null)
        {
            textoPuntos.text = "Puntos: " + GameSessionManager.Instance.ObtenerPuntaje();
        }
    }
}

