using UnityEngine;
using UnityEngine.UI;          


public class GameOverUI : MonoBehaviour
{

    public Text finalScoreText; 

    void Start()
    {
        int score = PlayerPrefs.GetInt("LastScore", 0);
        finalScoreText.text = score.ToString();

        // Opcional: Limpiar la clave para que no persista innecesariamente.
        // PlayerPrefs.DeleteKey("LastScore");
        // PlayerPrefs.Save(); // Guardar el borrado si lo haces
    }
}