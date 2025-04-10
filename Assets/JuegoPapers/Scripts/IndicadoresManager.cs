using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class IndicadoresManager : MonoBehaviour
{
    public GameObject prefabIndicadorUI;
    public Transform contenedorIndicadores;
    public int idInstancia; // Establece esto desde tu UIManager o controlador principal

    private Dictionary<int, Slider> slidersIndicadores = new Dictionary<int, Slider>();

    public void MostrarIndicadores()
    {
        Debug.Log("MostrarIndicadores() fue llamado");
        StartCoroutine(CargarIndicadores());
    }


    IEnumerator CargarIndicadores()
    {
        string url = $"https://10.22.169.234:7058/Videojuego/indicadores/{idInstancia}";
        UnityWebRequest req = UnityWebRequest.Get(url);
        req.certificateHandler = new ForceAcceptAll(); // Para SSL local
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al obtener los indicadores: " + req.error);
            yield break;
        }

        string json = "{\"indicadores\":" + req.downloadHandler.text + "}";
        IndicadoresListWrapper wrapper = JsonUtility.FromJson<IndicadoresListWrapper>(json);

        foreach (Transform child in contenedorIndicadores)
            Destroy(child.gameObject); // Limpia los indicadores anteriores

        foreach (Indicador indicador in wrapper.indicadores)
        {
            Debug.Log("Creando indicador: " + indicador.nombre);
            
            GameObject nuevo = Instantiate(prefabIndicadorUI, contenedorIndicadores);
            nuevo.transform.Find("NombreIndicador").GetComponent<Text>().text = indicador.nombre;

            Slider barra = nuevo.transform.Find("BarraIndicador").GetComponent<Slider>();
            barra.maxValue = 15;
            barra.value = 3 + indicador.impacto_total;

            slidersIndicadores[indicador.id_indicador] = barra;
        }

    }

    public void ActualizarIndicadores(List<Impacto> impactos)
    {
        foreach (Impacto impacto in impactos)
        {
            if (slidersIndicadores.TryGetValue(impacto.id_indicador, out Slider barra))
            {
                barra.value += impacto.cambio_valor;
                barra.value = Mathf.Clamp(barra.value, 0, 15);
            }
        }
    }
}