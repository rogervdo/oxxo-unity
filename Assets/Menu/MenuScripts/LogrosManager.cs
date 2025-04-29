using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class LogrosManager : MonoBehaviour
{
    public static LogrosManager Instance;

    [Header("Configuración")]
    public string apiBaseUrl = "https://localhost:7058/Logros";  // <-- pon tu URL real
    public int idUsuario;  // Id del usuario logueado

    [Header("Logros")]
    public List<LogroAsset> logrosAssets;  // Lista de logros para asignar en el Inspector

    private Dictionary<int, GameObject> logrosDict = new Dictionary<int, GameObject>();

    void Awake()
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

        // Llenamos el diccionario para acceso rápido
        foreach (var logro in logrosAssets)
        {
            logrosDict.Add(logro.id_logro, logro.asset);
        }
    }

    void Start()
    {
        if (UserManager.Instance != null && UserManager.Instance.CurrentUserId.HasValue)
        {
            idUsuario = UserManager.Instance.CurrentUserId.Value;
            Debug.Log($"LogrosManager: idUsuario obtenido del login: {idUsuario}");
            StartCoroutine(ObtenerLogros());
        }
        else
        {
            Debug.LogError("LogrosManager: No se encontró usuario logueado.");
        }
    }

    // 👇 GET: Obtener los logros del usuario
    IEnumerator ObtenerLogros()
    {
        string url = $"{apiBaseUrl}/{idUsuario}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"❌ Error obteniendo logros: {request.error}");
            yield break;
        }

        LogroObtenidoListWrapper wrapper = JsonUtility.FromJson<LogroObtenidoListWrapper>("{\"logros\":" + request.downloadHandler.text + "}");

        foreach (var logro in wrapper.logros)
        {
            if (logrosDict.ContainsKey(logro.id_logro))
            {
                logrosDict[logro.id_logro].SetActive(true);  // Activa el asset correspondiente
            }
        }
    }

    // 👇 POST: Registrar nuevo logro
    public void RegistrarLogro(int idLogro)
    {
        StartCoroutine(RegistrarLogroCoroutine(idLogro));
    }

    IEnumerator RegistrarLogroCoroutine(int idLogro)
    {
        string url = $"{apiBaseUrl}/obtener";

        LogroObtenerRequest requestBody = new LogroObtenerRequest
        {
            id_usuario = idUsuario,
            id_logro = idLogro
        };

        string jsonData = JsonUtility.ToJson(requestBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"❌ Error registrando logro: {request.error}");
        }
        else
        {
            Debug.Log($"✅ Logro {idLogro} registrado exitosamente.");

            // Opcional: activar inmediatamente en pantalla
            if (logrosDict.ContainsKey(idLogro))
            {
                logrosDict[idLogro].SetActive(true);
            }
        }
    }
}


