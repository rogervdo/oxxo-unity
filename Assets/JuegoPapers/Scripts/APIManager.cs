using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public static APIManager Instance; // Singleton para fácil acceso
    public string apiBaseUrl = "https://10.22.169.234:7058"; // URL base de tu API
    private List<Caso> casosCargados = new List<Caso>(); // Lista local de casos

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: mantener entre escenas si quieres
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método para obtener casos desde la API
    public void ObtenerCasos()
    {
        StartCoroutine(ObtenerCasosDesdeAPI());
    }

    private IEnumerator ObtenerCasosDesdeAPI()
    {
        string urlCasos = $"{apiBaseUrl}/videojuego";
        UnityWebRequest casosRequest = UnityWebRequest.Get(urlCasos);
        casosRequest.certificateHandler = new ForceAcceptAll(); // Solo si tienes problemas con HTTPS local
        yield return casosRequest.SendWebRequest();

        if (casosRequest.result == UnityWebRequest.Result.Success)
        {
            try
            {
                string json = casosRequest.downloadHandler.text;
                Caso[] casos = JsonHelper.FromJson<Caso>(json);
                casosCargados = new List<Caso>(casos);
                Debug.Log("APIManager: Casos recibidos correctamente. Total: " + casosCargados.Count);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("APIManager: Error parseando casos: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("APIManager: Error al obtener casos: " + casosRequest.error);
        }
    }

    // Método para acceder a los casos cargados
    public List<Caso> GetCasos()
    {
        return casosCargados;
    }
}
