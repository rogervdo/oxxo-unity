using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class APIManagerLogin : MonoBehaviour
{
    public static APIManagerLogin Instance;

    public string apiBaseUrl = "https://10.22.169.234:7058"; // Tu URL real

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persistente entre escenas
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

    public void IniciarLogin(string nickname, string password, System.Action<bool> callback)
    {
        StartCoroutine(VerificarLoginCoroutine(nickname, password, callback));
    }

    private IEnumerator VerificarLoginCoroutine(string nickname, string password, System.Action<bool> callback)
    {
        string url = $"{apiBaseUrl}/login/login?nickname={nickname}&password={password}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new ForceAcceptAll();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LoginResponse data = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

            if (data.acceso)
            {
                PlayerPrefs.SetInt("id_usuario", data.id_usuario);
                PlayerPrefs.Save();

                callback?.Invoke(true);
                yield break;
            }
        }

        callback?.Invoke(false);
    }
}