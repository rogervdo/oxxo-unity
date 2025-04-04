using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIControllerEspacio : MonoBehaviour
{
    public Text timeText; // Texto para mostrar el tiempo restante
    public Sprite spendLives; // Sprite para mostrar la pérdida de vidas
    public Image[] livesImages; // Imágenes de las vidas restantes
    int lives = 3; // Número inicial de vidas
    int time;
    void Start()
    {
        time = EspacioGameControll.Instance.timeToWin; // Establece el tiempo de la partida
        lives = PlayerPrefs.GetInt("lives", 3); // Carga las vidas guardadas
        ActiveText(); // 
    }

    public void ActiveText()
    {
        timeText.text = "TIEMPO RESTANTE: "+time;
    }

    public void StartTimer()
    {
        StartCoroutine(MatchTime());
    }

     IEnumerator MatchTime()
    {
        yield return new WaitForSeconds(1); // Espera 1 segundo
        time -= 1; // Reduce el tiempo
        ActiveText(); // Actualiza el texto en la UI

        if (time == 0)
        {
            SceneManager.LoadScene("Escena_Ganar_S"); // Carga la escena de fin del juego cuando se acaba el tiempo
        }
        else
        {
            StartCoroutine(MatchTime()); // Vuelve a llamar la corutina si aún hay tiempo
        }
    }

    public void UpdateLives()
    {
        lives = EspacioGameControll.Instance.GetCurrentLives(); // Obtiene las vidas actuales
        if (lives > 0)
        {
            livesImages[lives].sprite = spendLives; // Actualiza la imagen de la vida perdida
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
