using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class APIManager : MonoBehaviour
{
    public static APIManager Instance;

    public string apiBaseUrl = "https://10.22.169.234:7058"; // <- reemplaza con tu URL real

    public int idInstancia { get; private set; }
    public List<Caso> casosCargados = new List<Caso>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // opcional si quieres conservar entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IniciarJuego(int idJuego = 2)
    {
        StartCoroutine(CrearInstanciaYObtenerCasos(idJuego));
    }

    private IEnumerator CrearInstanciaYObtenerCasos(int idJuego)
    {
        // 1. Crear instancia
        string urlInstancia = $"{apiBaseUrl}/videojuego/instancia/{idJuego}";

        UnityWebRequest request = UnityWebRequest.PostWwwForm(urlInstancia, "");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            InstanciaRespuesta respuesta = JsonUtility.FromJson<InstanciaRespuesta>(request.downloadHandler.text);
            idInstancia = respuesta.id_instancia;

            Debug.Log("Instancia creada: " + idInstancia);

            // 2. Obtener casos aleatorios
            string urlCasos = $"{apiBaseUrl}/videojuego";
            UnityWebRequest casosRequest = UnityWebRequest.Get(urlCasos);
            yield return casosRequest.SendWebRequest();

            if (casosRequest.result == UnityWebRequest.Result.Success)
            {
                string json = casosRequest.downloadHandler.text;
                Caso[] casos = JsonHelper.FromJson<Caso>(json);
                casosCargados = new List<Caso>(casos);

                Debug.Log("Casos recibidos: " + casosCargados.Count);
            }
            else
            {
                Debug.LogError("Error al obtener casos: " + casosRequest.error);
            }
        }
        else
        {
            Debug.LogError("Error al crear instancia: " + request.error);
        }
    }
}
