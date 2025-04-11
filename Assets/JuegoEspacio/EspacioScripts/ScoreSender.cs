using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ScoreSender : MonoBehaviour
{
    public static ScoreSender Instance;

    private string apiUrl = "https://localhost:7058/Score/SaveGameResult"; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnviarScoreFinal(int puntuacion)
    {
        if (UserManager.Instance == null || !UserManager.Instance.CurrentUserId.HasValue)
        {
            Debug.LogError("❗ No se puede enviar score: UserManager o ID nulo.");
            return;
        }

        int idUsuario = UserManager.Instance.CurrentUserId.Value;
        int idJuego = 1; // ID fijo como mencionaste

        StartCoroutine(EnviarCoroutine(idUsuario, idJuego, puntuacion));
    }

    private IEnumerator EnviarCoroutine(int idUsuario, int idJuego, int puntuacion)
    {
        SaveScoreRequest requestBody = new SaveScoreRequest
        {
            idUsuario = idUsuario,
            idJuego = idJuego,
            puntuacion = puntuacion
        };

        string json = JsonUtility.ToJson(requestBody);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new APIManagerLogin.ForceAcceptAll(); // para certificados locales

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Puntaje enviado exitosamente.");
        }
        else
        {
            Debug.LogError("❌ Error al enviar puntaje: " + request.error);
        }
    }

    [System.Serializable]
    public class SaveScoreRequest
    {
        public int idUsuario;
        public int idJuego;
        public int puntuacion;
    }
}
