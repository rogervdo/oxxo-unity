using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Ui_LivesControll : MonoBehaviour
{   
    // Texto para mostrar el tiempo restante
    public Sprite spendLives; // Sprite para mostrar la pérdida de vidas
    public Image[] livesImages; // Imágenes de las vidas restantes
    int lives = 3; // Número inicial de vidas
    int time;
    void Start()
    {
        time = EspacioGameControll.Instance.timeToWin; // Establece el tiempo de la partida
        lives = PlayerPrefs.GetInt("lives", 3); // Carga las vidas guardadas 
    }

    public void UpdateLives()
    {
        lives = EspacioGameControll.Instance.GetCurrentLives(); // Obtiene las vidas actuales
        if (lives > 0 && lives - 1 < livesImages.Length)
        {
            livesImages[lives - 1].sprite = spendLives; // Actualiza la imagen de la vida perdida
        }
    }


}
