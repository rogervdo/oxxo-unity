using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;


public class UIManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject clipboardPanel;
    public GameObject libroPanel;
    private int idInstancia;

    [Header("Tablet y sus opciones")]
    public GameObject tabletOpcionesContainer;
    private bool opcionesYaMostradas = false;
    public LiderMovimiento liderMovimiento;
    public Image imagenLider; // Asignas la imagen dentro de LiderContainer
    public Sprite[] spritesLideres; // Arrastras los 6 sprites en el orden correcto
    private int indiceCaso = 0;
    private int totalCasos = 6;
    private Coroutine temporizadorCoroutine;
    private float tiempoLimite = 60f;
    // Asignar el botón 1 de la tablet para auto-ejecución
    public Button botonOpcion1; // arrástralo desde Unity
    public Text textoTemporizador;
    private List<int> ordenCasosAleatorios = new List<int>();
    private List<int> ordenSpritesAleatorios = new List<int>();
    private List<Caso> listaCasos = new List<Caso>(); 
    public CasoViewer casoViewer;
    public OpcionesManager opcionesManager;


    private void Start()
    {
        StartCoroutine(InicializarJuegoDesdeAPI());
    }



    // Mostrar clipboard
    public void MostrarClipboard()
    {
        clipboardPanel.SetActive(true);
        clipboardPanel.transform.SetAsLastSibling();

        if (!opcionesYaMostradas)
        {
            opcionesYaMostradas = true;
            tabletOpcionesContainer.SetActive(true);

            // Aquí cargamos las opciones, pero solo si hay un caso actual
            var caso = listaCasos[indiceCaso];
            opcionesManager.CargarOpcionesParaCaso(caso.id_caso);
        }
    }

    public void CerrarClipboard()
    {
        clipboardPanel.SetActive(false);
        // NO ocultamos opciones aquí
    }

    public void MostrarLibro()
    {
        libroPanel.SetActive(true);
        libroPanel.transform.SetAsLastSibling();
    }

    public void CerrarLibro()
    {
        libroPanel.SetActive(false);
    }

    // Llamado cuando se elige una opción de la tablet
    public void OpcionSeleccionada()
    {
        // 1. Ocultar opciones de la tablet
        tabletOpcionesContainer.SetActive(false);
        opcionesYaMostradas = false;

        // 2. Cerrar el clipboard si está abierto
        if (clipboardPanel.activeSelf)
            clipboardPanel.SetActive(false);

        // 3. Hacer que el líder salga
        if (liderMovimiento != null)
            liderMovimiento.SalirLider();

        // 4. Aquí más adelante podrías preparar el siguiente caso
        if (temporizadorCoroutine != null)
        {
            StopCoroutine(temporizadorCoroutine);
            temporizadorCoroutine = null;
        } 
    }

    // UIManager.cs

    public void CargarSiguienteCaso()
    {
        indiceCaso++;

        if (indiceCaso >= ordenCasosAleatorios.Count)
        {
            Debug.Log("Juego terminado");
            return;
        }

        var caso = listaCasos[indiceCaso];
        int idSprite = ordenSpritesAleatorios[indiceCaso];

        imagenLider.sprite = spritesLideres[idSprite];
        imagenLider.rectTransform.localScale = Vector3.one;
        imagenLider.rectTransform.anchoredPosition = liderMovimiento.posicionInicio;

        liderMovimiento.IniciarEntrada();

        casoViewer.MostrarCaso(caso); // Muestra el texto en UI

        if (temporizadorCoroutine != null)
            StopCoroutine(temporizadorCoroutine);

        temporizadorCoroutine = StartCoroutine(TemporizadorDecision());
    }

    public void RegistrarDecision(int idOpcion)
    {
        Debug.Log("Opción seleccionada con ID: " + idOpcion);

        // Aquí puedes hacer un POST a la API con ese idOpcion y la idInstancia
        // También podrías preparar el siguiente caso si lo deseas aquí
    }


    private IEnumerator TemporizadorDecision()
    {
        float tiempoRestante = tiempoLimite;

        while (tiempoRestante > 0f)
        {
            tiempoRestante -= Time.deltaTime;

            // Mostrar tiempo en formato mm:ss
            if (textoTemporizador != null)
            {
                int segundos = Mathf.CeilToInt(tiempoRestante);
                int segs = segundos % 60;
                textoTemporizador.text = $"Tiempo restante: {segs:00}";
            }

            yield return null;
        }

        // Tiempo agotado
        textoTemporizador.text = "Tiempo agotado";

        Debug.Log("Tiempo agotado. Ejecutando opción por defecto...");
        
        if (botonOpcion1 != null && botonOpcion1.interactable)
        {
            botonOpcion1.onClick.Invoke();
        }
    }

    private IEnumerator InicializarJuegoDesdeAPI()
    {
        // 1. Crear nueva instancia
        UnityWebRequest request = UnityWebRequest.PostWwwForm("https://10.22.169.234:7058/Videojuego/instancia/2", "");
        request.certificateHandler = new ForceAcceptAll();
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error creando instancia: " + request.error);
            yield break;
        }

        InstanciaRespuesta data = JsonUtility.FromJson<InstanciaRespuesta>(request.downloadHandler.text);
        idInstancia = data.id_instancia;

        // 2. Obtener todos los casos
        UnityWebRequest requestCasos = UnityWebRequest.Get("https://10.22.169.234:7058/Videojuego");
        requestCasos.certificateHandler = new ForceAcceptAll();
        yield return requestCasos.SendWebRequest();

        if (requestCasos.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al obtener casos: " + requestCasos.error);
            yield break;
        }

        listaCasos = JsonHelper.FromJson<Caso>(requestCasos.downloadHandler.text).ToList();
        ordenCasosAleatorios = listaCasos.Select(c => c.id_caso).ToList();

        // 3. Orden aleatorio de sprites
        ordenSpritesAleatorios = Enumerable.Range(0, 6).OrderBy(x => Random.value).ToList();

        indiceCaso = -1;
        CargarSiguienteCaso();
    }



}
