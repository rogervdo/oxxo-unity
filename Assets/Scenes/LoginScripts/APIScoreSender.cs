using UnityEngine;
using UnityEngine.Networking; // Necesario para UnityWebRequest
using System.Collections;       // Necesario para IEnumerator

public class APIScoreSender : MonoBehaviour
{
    // Clase auxiliar interna para el JSON
    [System.Serializable]
    private class ScoreData
    {
        public int IdUsuario;
        public int IdJuego;
        public int Puntuacion;
    }

    // Handler para certificados (SOLO DESARROLLO LOCAL HTTPS)
    public class ForceAcceptAll : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }

    // --- MÉTODO ESTÁTICO para enviar el puntaje ---
    // Ahora recibe la URL como parámetro para mayor flexibilidad
    public static IEnumerator SendScore(string apiUrl, int userId, int gameId, int score)
    {
        // Verifica si la URL es válida
        if (string.IsNullOrEmpty(apiUrl))
        {
            Debug.LogError("[APIScoreSender] Error: URL de la API no proporcionada.");
            yield break; // Sale de la coroutine si no hay URL
        }

        ScoreData payload = new ScoreData { IdUsuario = userId, IdJuego = gameId, Puntuacion = score };
        string jsonData = JsonUtility.ToJson(payload);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.certificateHandler = new ForceAcceptAll(); // Aplica el handler

            Debug.Log($"[APIScoreSender] Enviando a {apiUrl}: {jsonData}");
            yield return request.SendWebRequest();

            // Procesa respuesta
            if (request.result != UnityWebRequest.Result.Success) {
                Debug.LogError($"[APIScoreSender] Error: {request.error} | Code: {request.responseCode} | Resp: {request.downloadHandler?.text}");
            } else {
                Debug.Log($"[APIScoreSender] Puntaje enviado (Code {request.responseCode})");
            }
        }
    }

} // Fin de la clase APIScoreSender