using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class UIControllerEspacio : MonoBehaviour
{
    public Text timeText;
    public Sprite spendLives;
    public Image[] livesImages;
    int lives = 3;
    int time;

    private void Start()
    {
        time = EspacioGameControll.Instance.timeToWin;
        lives = PlayerPrefs.GetInt("lives", 3);
        ActiveText();
    }

    public void ActiveText()
    {
        timeText.text = "TIEMPO RESTANTE: " + time;
    }

    public void StartTimer()
    {
        StartCoroutine(MatchTime());
    }

    IEnumerator MatchTime()
    {
        yield return new WaitForSecondsRealtime(1);
        time -= 1;
        ActiveText();

        if (time == 0)
        {
            int scoreFinal = ScoreManager.Instance.score;

            PlayerPrefs.SetInt("lastScoreEspacial", scoreFinal);
            PlayerPrefs.Save();

            yield return StartCoroutine(EnviarScoreFinal(scoreFinal));

            Debug.Log("✅ Score enviado, cambiando a Escena_Ganar_S...");
            SceneManager.LoadScene("Escena_Ganar_S");
        }
        else
        {
            StartCoroutine(MatchTime());
        }
    }

    public void UpdateLives()
    {
        lives = EspacioGameControll.Instance.GetCurrentLives();

        if (lives > 0)
        {
            livesImages[lives].sprite = spendLives;
        }
        else
        {
            int scoreFinal = ScoreManager.Instance.score;

            PlayerPrefs.SetInt("lastScoreEspacial", scoreFinal);
            PlayerPrefs.Save();

            StartCoroutine(EnviarScoreFinalYPerder(scoreFinal));
        }
    }

    public void RegistrarDecision(int idOpcion)
    {
        StartCoroutine(EnviarDecisionAPI(idOpcion));
    }

    private IEnumerator EnviarDecisionAPI(int idOpcion)
    {
        int idInstancia = PlayerPrefs.GetInt("id_instancia", 0);

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
        request.certificateHandler = new ForceAcceptAll();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Respuesta enviada correctamente.");
        }
        else
        {
            Debug.LogError("❌ Error al enviar respuesta: " + request.error);
        }
    }

    private IEnumerator EnviarScoreFinal(int puntuacion)
    {
        int idUsuario = UserManager.Instance != null ? UserManager.Instance.GetCurrentUser2() : 0;
        int idJuego = 1; // 🔥 Siempre juego ID 1 como me dijiste

        if (idUsuario == 0)
        {
            Debug.LogError("❌ No hay ID de usuario válido. Score no enviado.");
            yield break;
        }

        string apiUrl = "https://localhost:7058/Score/SaveGameResult";

        SaveScoreRequest requestBody = new SaveScoreRequest
        {
            idUsuario = idUsuario,
            idJuego = idJuego,
            puntuacion = puntuacion
        };

        string jsonData = JsonUtility.ToJson(requestBody);

        Debug.Log($"🚀 Enviando JSON de Score: {jsonData}");

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new ForceAcceptAll();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Score enviado exitosamente.");
        }
        else
        {
            Debug.LogError("❌ Error al enviar score: " + request.error);
        }
    }

    private IEnumerator EnviarScoreFinalYPerder(int scoreFinal)
    {
        yield return StartCoroutine(EnviarScoreFinal(scoreFinal));

        Debug.Log("✅ Score enviado, cambiando a Escena_Perder_S...");
        SceneManager.LoadScene("Escena_Perder_S");
    }
}

[System.Serializable]
public class SaveScoreRequest
{
    public int idUsuario;
    public int idJuego;
    public int puntuacion;
}
