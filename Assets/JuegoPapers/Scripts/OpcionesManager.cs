using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class OpcionesManager : MonoBehaviour
{
    public GameObject[] botonesOpciones; // Asigna ButtonOption1, 2 y 3 desde el inspector
    private Button[] botones;

    public UIManager uiManager; // Arrastra el UIManager aquí desde Unity
    public ImpactoView impactoViewer;

    private void Awake()
    {
        botones = botonesOpciones.Select(b => b.GetComponent<Button>()).ToArray();
    }

    public void CargarOpcionesParaCaso(int idCaso)
    {
        StartCoroutine(ObtenerOpciones(idCaso));
    }

    private IEnumerator ObtenerOpciones(int idCaso)
    {
        string url = $"https://10.22.169.234:7058/Videojuego/opciones/{idCaso}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new ForceAcceptAll(); // Para certificados locales
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al obtener opciones: " + request.error);
            yield break;
        }

        Opcion[] opcionesTotales = JsonHelper.FromJson<Opcion>(request.downloadHandler.text);
        List<Opcion> opcionesAleatorias = opcionesTotales.OrderBy(x => Random.value).Take(3).ToList();

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            var btn = botonesOpciones[i].GetComponent<Button>();
            var textoBtn = botonesOpciones[i].transform.GetChild(0).GetComponent<Text>();

            textoBtn.text = opcionesAleatorias[i].texto_opcion;

            int idOpcion = opcionesAleatorias[i].id_opcion;
            int botonIndex = i;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                uiManager.RegistrarDecision(idOpcion);
            });

            // Aquí está bien colocada
            StartCoroutine(CargarImpactosDeAPI(idOpcion, botonIndex));
        }

    }

    public IEnumerator CargarImpactosDeAPI(int idOpcion, int botonIndex)
    {
        string url = $"https://10.22.169.234:7058/Videojuego/opcion/{idOpcion}/indicadores";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new ForceAcceptAll();
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al obtener impactos: " + request.error);
            yield break;
        }

        string json = "{\"impactos\":" + request.downloadHandler.text + "}";
        ImpactoListWrapper wrapper = JsonUtility.FromJson<ImpactoListWrapper>(json);

        if (impactoViewer != null)
        {
            impactoViewer.MostrarImpactos(wrapper.impactos, botonIndex);
        }
    }
}
