using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CasoViewer : MonoBehaviour
{
    public Text tituloCasoText;
    public Text descripcionCasoText;

    // En tu script CasoViewer.cs
    public void MostrarCaso(Caso caso)
    {
        // Mostrar en los elementos de UI:
        tituloCasoText.text = caso.titulo;
        descripcionCasoText.text = caso.descripcion;
    }


    private IEnumerator CargarCasoDesdeAPI(int idCaso)
    {
        string url = $"https://10.22.169.234:7058/Videojuego/caso/{idCaso}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new ForceAcceptAll();
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error obteniendo caso: " + request.error);
            yield break;
        }

        Caso caso = JsonUtility.FromJson<Caso>(request.downloadHandler.text);
        tituloCasoText.text = caso.titulo;
        descripcionCasoText.text = caso.descripcion;
    }
}

