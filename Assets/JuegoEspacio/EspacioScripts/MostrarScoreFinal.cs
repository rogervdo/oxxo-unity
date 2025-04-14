using UnityEngine;
using UnityEngine.UI;

public class MostrarScoreFinal : MonoBehaviour
{
    public Text textoScoreFinal;

    void Start()
    {
        int ultimoScore = PlayerPrefs.GetInt("lastScoreEspacial", 0);
        textoScoreFinal.text = "PUNTAJE FINAL: " + ultimoScore.ToString("0000");
    }
}


