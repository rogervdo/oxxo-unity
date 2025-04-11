using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public class APIManagerLogin : MonoBehaviour
{
    public static APIManagerLogin Instance;
    public string apiBaseUrl = "https://localhost:7058";

    // Clase para deserializar la respuesta JSON del login API
    [System.Serializable]
    public class LoginResponse
    {
        public bool acceso;
        public int id_usuario; 
    }

    private void Awake()
    {
        // Singleton para APIManagerLogin
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

    // Método público para iniciar el proceso de login desde otros scripts (como LoginUI)
    public void IniciarLogin(string nickname, string password, System.Action<bool> callback)
    {
        // Inicia la coroutine que hace la llamada web real
        StartCoroutine(VerificarLoginCoroutine(nickname, password, callback));
    }

    // Coroutine que realiza la llamada a la API de login
    private IEnumerator VerificarLoginCoroutine(string nickname, string password, System.Action<bool> callback)
    {
        string url = $"{apiBaseUrl}/login/login?nickname={UnityWebRequest.EscapeURL(nickname)}&password={UnityWebRequest.EscapeURL(password)}";

        Debug.Log($"[APIManagerLogin] Intentando login en: {url}");

        // Crea la solicitud GET
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new ForceAcceptAll(); 

        // Envía la solicitud y espera la respuesta
        yield return request.SendWebRequest();

        // Verifica si hubo éxito en la conexión y protocolo
        if (request.result == UnityWebRequest.Result.Success)
        {
            // Intenta deserializar la respuesta JSON
            try
            {
                LoginResponse data = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

                // Comprueba si el acceso fue concedido y si tenemos un ID válido
                if (data != null && data.acceso && data.id_usuario > 0)
                {
                    Debug.Log($"[APIManagerLogin] Login exitoso. Usuario ID: {data.id_usuario}");

                    // --- GUARDAR ID DE USUARIO EN USERMANAGER ---
                    if (UserManager.Instance != null)
                    {
                        UserManager.Instance.SetCurrentUser(data.id_usuario);
                    }
                    else
                    {
                        Debug.LogError("[APIManagerLogin] ¡UserManager.Instance no encontrado! No se pudo guardar el ID.");
                        // Considera si el callback debe ser false en este caso crítico
                        callback?.Invoke(false); // Indica fallo si no hay UserManager
                        yield break; // Salir de la coroutine
                    }

                    // Llama al callback indicando éxito
                    callback?.Invoke(true);
                    yield break; // Salir porque ya procesamos el éxito
                }
                else
                {
                    // El JSON se recibió pero acceso=false o id_usuario inválido
                    Debug.LogWarning($"[APIManagerLogin] Login fallido: Acceso denegado o ID de usuario inválido en respuesta JSON.");
                }
            }
            catch (System.Exception ex)
            {
                // Error al intentar leer/deserializar el JSON
                Debug.LogError($"[APIManagerLogin] Error al procesar JSON de login: {ex.Message}. Respuesta recibida: {request.downloadHandler.text}");
            }
        }
        else // Hubo error de conexión o protocolo (4xx, 5xx)
        {
            Debug.LogError($"[APIManagerLogin] Error en solicitud de login: {request.error} | Código: {request.responseCode}");
        }

        callback?.Invoke(false);
    }

    public class ForceAcceptAll : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }
}