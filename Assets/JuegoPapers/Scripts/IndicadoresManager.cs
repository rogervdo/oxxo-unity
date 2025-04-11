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
    public int idInstancia;

    private Dictionary<int, Slider> slidersIndicadores = new Dictionary<int, Slider>();

    public void MostrarIndicadores()
    {
        Debug.Log("MostrarIndicadores() fue llamado");
        StartCoroutine(CargarIndicadores());
    }

    IEnumerator CargarIndicadores()
    {
        string url = $"https://10.22.169.234:7058/Videojuego/valores_actuales/{idInstancia}";
        UnityWebRequest req = UnityWebRequest.Get(url);
        req.certificateHandler = new ForceAcceptAll(); // Certificado local
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al obtener los indicadores: " + req.error);
            yield break;
        }

        string json = "{\"indicadores\":" + req.downloadHandler.text + "}";
        IndicadoresListWrapper wrapper = JsonUtility.FromJson<IndicadoresListWrapper>(json);

        foreach (Transform child in contenedorIndicadores)
            Destroy(child.gameObject);

        foreach (Indicador indicador in wrapper.indicadores)
        
{
    Debug.Log("Creando indicador: " + indicador.nombre);

    GameObject nuevo = Instantiate(prefabIndicadorUI, contenedorIndicadores);
    var nombreTxt = nuevo.transform.Find("NombreIndicador");
    var barraObj = nuevo.transform.Find("BarraIndicador");
    var valorTexto = nuevo.transform.Find("ValorTexto"); // 👈 Asegúrate que coincida el nombre

    if (nombreTxt == null) Debug.LogError("No se encontró NombreIndicador");
    if (barraObj == null) Debug.LogError("No se encontró BarraIndicador");
    if (valorTexto == null) Debug.LogError("No se encontró ValorTexto"); // Validación opcional

    nombreTxt.GetComponent<Text>().text = indicador.nombre;

    Slider barra = barraObj.GetComponent<Slider>();
    barra.maxValue = 15;
    barra.value = indicador.valor_actual;

    // 👇 Asigna el valor al texto
    Text textoValor = valorTexto.GetComponent<Text>();
    textoValor.text = barra.value.ToString("0"); // sin decimales

    // 👇 Actualiza el texto en tiempo real cuando el valor cambie
    barra.onValueChanged.AddListener(val =>
    {
        textoValor.text = val.ToString("0");
    });

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

    public int SumarValoresFinales()
    {
        int suma = 0;
        foreach (var slider in slidersIndicadores.Values)
            suma += Mathf.RoundToInt(slider.value); // Asegura que sea int

        return suma;
    }

}
