using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

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
        yield return new WaitForSecondsRealtime(1); // Espera 1 segundo
        time -= 1; // Reduce el tiempo
        ActiveText(); // Actualiza el texto en la UI

        if (time == 0)
        {
            int scoreFinal = ScoreManager.Instance.score;

            PlayerPrefs.SetInt("lastScoreEspacial", scoreFinal);
            PlayerPrefs.Save();

            if (ScoreSender.Instance != null)
                ScoreSender.Instance.EnviarScoreFinal(scoreFinal);

            SceneManager.LoadScene("Escena_Ganar_S");
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

    public void RegistrarDecision(int idOpcion)
{
    StartCoroutine(EnviarDecisionAPI(idOpcion));
}

private IEnumerator EnviarDecisionAPI(int idOpcion)
{
    int idInstancia = PlayerPrefs.GetInt("id_instancia", 0); // o como lo manejes

    if (idInstancia == 0)
    {
        Debug.LogWarning("No hay id_instancia guardado en PlayerPrefs.");
        yield break;
    }

    string url = "https://localhost:7058/videojuego/respuesta";

    WWWForm form = new WWWForm();
    form.AddField("id_instancia", idInstancia);
    form.AddField("id_opcion", idOpcion);

    UnityWebRequest request = UnityWebRequest.Post(url, form);
    request.certificateHandler = new ForceAcceptAll(); // si usas HTTPS con certificado local

    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
        Debug.Log("Respuesta enviada correctamente.");
    }
    else
    {
        Debug.LogError("Error al enviar la respuesta: " + request.error);
    }
}

}
