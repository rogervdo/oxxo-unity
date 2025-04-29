using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ScoreSender : MonoBehaviour
{
    public static ScoreSender Instance;

    [Header("Configuración API")]
    public string apiUrl = "https://10.22.169.234:7058/Score/SaveGameResult"; // 🔥 Pon aquí tu URL real

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class SaveScoreRequest
    {
        public int idUsuario;
        public int idJuego;
        public int puntuacion;
    }

    public void EnviarScoreFinal(int puntuacion)
    {
        if (UserManager.Instance == null || !UserManager.Instance.CurrentUserId.HasValue)
        {
            Debug.LogError("❗ No se puede enviar score: UserManager o ID nulo.");
            return;
        }

        int idUsuario = UserManager.Instance.CurrentUserId.Value;
        int idJuego = 2; // 🔥 ID del juego de nave, pon el ID correcto aquí

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
        request.certificateHandler = new APIManagerLogin.ForceAcceptAll(); // 🔥 O pon null si no usas certificado especial

        Debug.Log($"🚀 Enviando JSON de Score: {json}");

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
}
