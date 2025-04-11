using UnityEngine;
using UnityEngine.Networking; 
using System.Collections;       

public class APIScoreSender : MonoBehaviour
{
    // Clase auxiliar interna para la estructura JSON.
    [System.Serializable]
    private class ScoreData
    {
        public int IdUsuario;
        public int IdJuego;
        public int Puntuacion;
    }


    public class ForceAcceptAll : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }


    public static IEnumerator SendScore(string apiUrl, int userId, int gameId, int score)
    {
        // Valida la URL.
        if (string.IsNullOrEmpty(apiUrl))
        {
            Debug.LogError("[APIScoreSender] Error: URL de la API no proporcionada.");
            yield break; // Sale de la corutina si no hay URL.
        }

        // Prepara el JSON.
        ScoreData payload = new ScoreData { IdUsuario = userId, IdJuego = gameId, Puntuacion = score };
        string jsonData = JsonUtility.ToJson(payload);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // Crea y configura la solicitud POST.
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.certificateHandler = new ForceAcceptAll(); // Aplica el manejador de certificados.

            Debug.Log($"[APIScoreSender] Enviando a {apiUrl}: {jsonData}"); // <<< LOG API MANTENIDO
            yield return request.SendWebRequest(); // Envía la solicitud.

            // Procesa la respuesta.
            if (request.result != UnityWebRequest.Result.Success) {

                Debug.LogError($"[APIScoreSender] Error: {request.error} | Code: {request.responseCode} | Resp: {request.downloadHandler?.text}");
            } else {

                Debug.Log($"[APIScoreSender] Puntaje enviado (Code {request.responseCode})");
            }
        }
    }

} 