using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class APIManagerEspacial : MonoBehaviour
{
    public static APIManagerEspacial Instance;

    public string apiBaseUrl = "https://10.22.197.131:7058";
    public List<PreguntaEspacio> preguntasEspaciales = new List<PreguntaEspacio>();

    private bool preguntasCargadas = false; // ✅ Nuevo flag

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

    public void IniciarJuegoEspacial(int juegoId = 1)
    {
        if (!preguntasCargadas)
        {
            StartCoroutine(ObtenerPreguntasEspaciales(juegoId));
            preguntasCargadas = true;
        }
        else
        {
            Debug.Log("✅ Las preguntas espaciales ya estaban cargadas. No se vuelve a pedir.");
        }
    }

    private IEnumerator ObtenerPreguntasEspaciales(int juegoId)
    {
        string url = $"{apiBaseUrl}/Espacio/Espacio_Pregunta?juegoId={juegoId}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new ForceAcceptAll();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            preguntasEspaciales = new List<PreguntaEspacio>(JsonHelper.FromJson<PreguntaEspacio>(json));
            Debug.Log($"✅ Preguntas espaciales recibidas: {preguntasEspaciales.Count}");
        }
        else
        {
            Debug.LogError("❌ Error al obtener preguntas espaciales: " + request.error);
        }
    }

    public IEnumerator ObtenerRespuestas(int idPregunta, System.Action<List<OpcionEspacio>> callback)
    {
        string url = $"{apiBaseUrl}/Espacio/respuestas/{idPregunta}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new ForceAcceptAll();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            OpcionEspacio[] opciones = JsonHelper.FromJson<OpcionEspacio>(json);
            callback(new List<OpcionEspacio>(opciones));
        }
        else
        {
            Debug.LogError("❌ Error al obtener respuestas: " + request.error);
            callback(null);
        }
    }
}
