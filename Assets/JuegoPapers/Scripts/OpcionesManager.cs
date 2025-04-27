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
    public IndicadoresManager indicadoresManager;

    private Dictionary<int, List<Impacto>> impactosPorBoton = new();

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
        string url = $"https://localhost:7058/Videojuego/opciones/{idCaso}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new ForceAcceptAll();
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
                Debug.Log("🟡 Botón clicado con id_opcion: " + idOpcion + "Instancia:" + indicadoresManager.idInstancia);

                StartCoroutine(AplicarImpactoEnBD(idOpcion)); // <--- esta debe estar
                uiManager.RegistrarDecision(idOpcion);
            });

            StartCoroutine(CargarImpactosDeAPI(idOpcion, botonIndex));
        }
    }

    private IEnumerator CargarImpactosDeAPI(int idOpcion, int botonIndex)
    {
        string url = $"https://localhost:7058/Videojuego/opcion/{idOpcion}/indicadores";
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

        impactosPorBoton[botonIndex] = wrapper.impactos;
    }

    private IEnumerator AplicarImpactoEnBD(int idOpcion)
    {
        string url = "https://localhost:7058/Videojuego/aplicar_impacto";

        var datos = new AplicarImpactoRequest
        {
            id_instancia = indicadoresManager.idInstancia,
            id_opcion = idOpcion
        };
        string body = JsonUtility.ToJson(datos);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);

        Debug.Log("📤 Enviando a API con cuerpo: " + body);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new ForceAcceptAll();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Error al aplicar impacto: " + request.error);
        }
        else
        {
            Debug.Log("✅ Impacto aplicado correctamente.");
            indicadoresManager.MostrarIndicadores(); // Refresca la hoja de indicadores
        }
        indicadoresManager.MostrarIndicadores();
        uiManager.tabletOpcionesContainer.SetActive(false);
    }



}
