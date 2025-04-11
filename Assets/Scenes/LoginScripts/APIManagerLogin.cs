using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class APIManagerLogin : MonoBehaviour
{
    public static APIManagerLogin Instance; // Instancia Singleton.
    public string apiBaseUrl = "https://localhost:7058"; // URL base para la API.


    [System.Serializable]
    public class LoginResponse
    {
        public bool acceso;
        public int id_usuario;
    }


    public class ForceAcceptAll : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }

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

    // Método público para iniciar el proceso de login desde otros scripts.
    public void IniciarLogin(string nickname, string password, System.Action<bool> callback)
    {
        // Inicia la corutina que realiza la solicitud web real.
        StartCoroutine(VerificarLoginCoroutine(nickname, password, callback));
    }

    // Corutina que realiza la llamada a la API de login.
    private IEnumerator VerificarLoginCoroutine(string nickname, string password, System.Action<bool> callback)
    {
        // Construye la URL completa con parámetros de consulta.
        string url = $"{apiBaseUrl}/login/login?nickname={UnityWebRequest.EscapeURL(nickname)}&password={UnityWebRequest.EscapeURL(password)}";

        Debug.Log($"[APIManagerLogin] Intentando login en: {url}"); // <<< LOG API MANTENIDO

        // Crea la solicitud GET.
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new ForceAcceptAll(); // Aplica el manejador de certificados.

        // Envía la solicitud y espera la respuesta.
        yield return request.SendWebRequest();

        // Verifica el éxito de la conexión y el protocolo.
        if (request.result == UnityWebRequest.Result.Success)
        {
            // Intenta deserializar la respuesta JSON.
            try
            {
                LoginResponse data = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

                // Comprueba si se concedió acceso y se recibió un ID válido.
                if (data != null && data.acceso && data.id_usuario > 0)
                {
                    Debug.Log($"[APIManagerLogin] Login exitoso. Usuario ID: {data.id_usuario}"); // <<< LOG API MANTENIDO

                    // --- GUARDA ID DE USUARIO EN USERMANAGER ---
                    if (UserManager.Instance != null)
                    {
                        UserManager.Instance.SetCurrentUser(data.id_usuario); // Almacena el ID.
                    }
                    else
                    {

                        Debug.LogError("[APIManagerLogin] ¡UserManager.Instance no encontrado! No se pudo guardar el ID.");
                        callback?.Invoke(false); // Indica fallo si falta UserManager.
                        yield break; // Sale de la corutina.
                    }

                    // Llama al callback indicando éxito.
                    callback?.Invoke(true);
                    yield break; // Sale ya que se procesó el éxito.
                }
                else
                {
  
                    Debug.LogWarning($"[APIManagerLogin] Login fallido: Acceso denegado o ID de usuario inválido en respuesta JSON.");
                }
            }
            catch (System.Exception ex)
            {

                Debug.LogError($"[APIManagerLogin] Error al procesar JSON de login: {ex.Message}. Respuesta recibida: {request.downloadHandler.text}");
            }
        }
        else 
        {

            Debug.LogError($"[APIManagerLogin] Error en solicitud de login: {request.error} | Código: {request.responseCode}");
        }

        // Si la ejecución llega aquí, el login falló. Invoca el callback con false.
        callback?.Invoke(false);
    }

} 