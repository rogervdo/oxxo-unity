using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class LogrosUIManager : MonoBehaviour
{
    [Header("Configuración API")]
    public string apiBaseUrl = "https://localhost:7058/Logros";  // tu endpoint base
    public int idUsuario;

    [Header("UI Elementos")]
    public GameObject panelLogros;  // Panel principal que entra/desplaza
    public Transform contenedorLogros;  // GridLayout donde instanciarás los prefabs
    public GameObject prefabLogro;  // Prefab base de un logro

    [Header("Animación")]
    public float velocidadAnimacion = 1000f;
    public Vector2 posicionFueraPantalla = new Vector2(0, -1100);
    public Vector2 posicionCentro = new Vector2(0, 0);

    private Dictionary<int, PrefabLogroController> logrosInstanciados = new Dictionary<int, PrefabLogroController>();

    void Start()
    {
        if (UserManager.Instance != null && UserManager.Instance.CurrentUserId.HasValue)
        {
            idUsuario = UserManager.Instance.CurrentUserId.Value;
            Debug.Log($"🔵 LogrosUIManager: idUsuario cargado = {idUsuario}");
        }
        else
        {
            Debug.LogError("❌ LogrosUIManager: No se encontró usuario logueado.");
        }

        panelLogros.GetComponent<RectTransform>().anchoredPosition = posicionFueraPantalla;
    }


    public void MostrarPanelLogros()
    {
        StartCoroutine(AnimarPanelEntrada());
        StartCoroutine(CargarLogros());
    }

    IEnumerator AnimarPanelEntrada()
    {
        RectTransform rt = panelLogros.GetComponent<RectTransform>();

        while (Vector2.Distance(rt.anchoredPosition, posicionCentro) > 1f)
        {
            rt.anchoredPosition = Vector2.MoveTowards(rt.anchoredPosition, posicionCentro, velocidadAnimacion * Time.deltaTime);
            yield return null;
        }
        rt.anchoredPosition = posicionCentro;
    }

    IEnumerator CargarLogros()
    {
        string url = $"{apiBaseUrl}/{idUsuario}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error obteniendo logros: {request.error}");
            yield break;
        }

        string rawJson = request.downloadHandler.text;
        Debug.Log($"📦 JSON crudo recibido: {rawJson}");

        if (string.IsNullOrWhiteSpace(rawJson))
        {
            Debug.LogError("⚠️ Respuesta vacía del servidor.");
            yield break;
        }

        string wrappedJson = "{\"logros\":" + rawJson + "}";
        LogroObtenidoListWrapper wrapper = JsonUtility.FromJson<LogroObtenidoListWrapper>(wrappedJson);

        if (wrapper == null || wrapper.logros == null)
        {
            Debug.LogError("❌ No se pudo parsear el JSON a LogroObtenidoListWrapper.");
            yield break;
        }


        // Limpiar primero si ya había
        foreach (Transform hijo in contenedorLogros)
        {
            Destroy(hijo.gameObject);
        }

        // Marcar qué logros tiene el usuario
        HashSet<int> logrosObtenidos = new HashSet<int>();
        foreach (var l in wrapper.logros)
        {
            logrosObtenidos.Add(l.id_logro);
        }

        // Instanciar todos los logros disponibles
        foreach (var logroConfig in LogrosData.todosLosLogros)  // <-- Te explicaré esta parte abajo
        {
            GameObject obj = Instantiate(prefabLogro, contenedorLogros);
            PrefabLogroController plc = obj.GetComponent<PrefabLogroController>();

            bool obtenido = logrosObtenidos.Contains(logroConfig.id_logro);
            Debug.Log($"📦 JSON recibido: {request.downloadHandler.text}");
            plc.Configurar(logroConfig, obtenido);
        }
    }

    public void CerrarPanelLogros()
    {
        StartCoroutine(AnimarPanelSalida());
    }

    IEnumerator AnimarPanelSalida()
    {
        RectTransform rt = panelLogros.GetComponent<RectTransform>();

        while (Vector2.Distance(rt.anchoredPosition, posicionFueraPantalla) > 1f)
        {
            rt.anchoredPosition = Vector2.MoveTowards(rt.anchoredPosition, posicionFueraPantalla, velocidadAnimacion * Time.deltaTime);
            yield return null;
        }

        rt.anchoredPosition = posicionFueraPantalla;
    }

}
