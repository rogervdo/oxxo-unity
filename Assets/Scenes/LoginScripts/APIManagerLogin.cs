using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class APIManagerLogin : MonoBehaviour
{
    public static APIManagerLogin Instance;

    [Header("Configuración API")]
    public string apiBaseUrl = "https://10.22.197.131:7058"; // Asegúrate de usar tu URL real

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persiste entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class LoginResponse
    {
        public bool acceso;
        public int id_usuario;
    }

    public void IniciarLogin(string nickname, string password)
    {
        StartCoroutine(VerificarLoginCoroutine(nickname, password));
    }

    private IEnumerator VerificarLoginCoroutine(string nickname, string password)
    {
        string url = $"{apiBaseUrl}/login/login?nickname={nickname}&password={password}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new ForceAcceptAll(); // por si usas HTTPS con certificado propio

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Respuesta de login: " + request.downloadHandler.text);
            LoginResponse data = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

            if (data.acceso)
            {
                Debug.Log($"✅ Login exitoso. ID: {data.id_usuario}");

                PlayerPrefs.SetInt("id_usuario", data.id_usuario);
                PlayerPrefs.Save();

                // Cargar siguiente escena si quieres
                // SceneManager.LoadScene("MenuPrincipal");
            }
            else
            {
                Debug.LogWarning("❌ Login fallido. Usuario o contraseña incorrectos.");
            }
        }
        else
        {
            Debug.LogError("Error de conexión con la API: " + request.error);
        }
    }
}
