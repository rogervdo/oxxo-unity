using UnityEngine;
using UnityEngine.UI;
using System.Collections;       // Necesario para Coroutines
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Networking;  // Necesario para el contexto de APIScoreSender

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singletone Esto lo agregas arriba en tu clase

    [Header("Paneles")] // Encabezado para organizar en el Inspector
    public GameObject clipboardPanel; // Referencia al panel del portapapeles
    public GameObject libroPanel;     // Referencia al panel del libro
    private int idInstancia;          // ID de la instancia de juego actual

    [Header("API Configuration")] // Configuración de la API
    public string apiBaseUrl = "https://10.22.169.234:7058"; // URL base para llamadas API
    public string scoreApiEndpointPath = "/api/Puntuaciones"; // <<< ESTABLECE TU RUTA REAL PARA PUNTUACIONES AQUÍ
    public int scoreGameId = 2; // <<< ESTABLECE EL ID CORRECTO PARA ESTE JUEGO AQUÍ

    [Header("Scene Names")] // Nombres de las Escenas
    // Nombres de tus posibles escenas finales
    public string winSceneName = "WinScenePP";
    public string loseSceneName = "LoseScenePP";
    public string alternateEndSceneName = "AlternEnd";
    // Umbrales de puntuación para decidir qué escena cargar
    public int winScoreThreshold = 60;  // Puntuación >= esto carga la escena de victoria
    public int loseScoreThreshold = 45; // Puntuación <= esto carga la escena de derrota

    [Header("Tablet y sus opciones")] // Configuración de la Tablet
    public GameObject tabletOpcionesContainer; // Contenedor de las opciones en la tablet
    private bool opcionesYaMostradas = false; // Bandera para controlar si las opciones ya se mostraron para el caso actual
    public LiderMovimiento liderMovimiento; // Referencia al script que controla el movimiento del líder
    public Image imagenLider;               // Referencia a la Imagen UI del líder
    public Sprite[] spritesLideres;         // Array de sprites para los diferentes líderes
    private int indiceCaso = 0;             // Índice del caso actual que se está jugando
    private Coroutine temporizadorCoroutine; // Referencia a la corutina del temporizador
    private float tiempoLimite = 60f;       // Tiempo límite en segundos para tomar una decisión
    public Button botonOpcion1;             // Referencia al botón de la primera opción (para selección por defecto)
    public Text textoTemporizador;          // Referencia al texto UI que muestra el temporizador
    private List<int> ordenCasosAleatorios = new List<int>(); // Lista para almacenar el orden aleatorio de los IDs de los casos
    private List<int> ordenSpritesAleatorios = new List<int>();// Lista para almacenar el orden aleatorio de los índices de los sprites
    private List<Caso> listaCasos = new List<Caso>();       // Lista para almacenar los objetos Caso recibidos de la API
    public CasoViewer casoViewer;           // Referencia al script que muestra la información del caso
    public OpcionesManager opcionesManager; // Referencia al script que maneja las opciones
    public IndicadoresManager indicadoresManager; // Referencia al script que maneja los indicadores (¡Asegúrate que esté asignado!)
    private bool juegoYaInicializado = false;
    private int idUsuario; // 🆕 Agregado


    // --- Start y otros métodos permanecen en gran parte iguales ---
    private void Start()
    {
        idUsuario = UserManager.Instance.GetCurrentUser2();
        // Asegura que los componentes requeridos estén asignados en el Inspector
        if (indicadoresManager == null || opcionesManager == null || casoViewer == null || liderMovimiento == null)
        {
            Debug.LogError("UIManager: ¡Faltan referencias a componentes requeridos en el Inspector!");
            return; // Previene errores posteriores
        }
         if (string.IsNullOrEmpty(winSceneName) || string.IsNullOrEmpty(loseSceneName) || string.IsNullOrEmpty(alternateEndSceneName))
         {
             Debug.LogError("UIManager: ¡Uno o más nombres de escenas finales no están establecidos en el Inspector!");
         }

        // Inicia la corutina que prepara el juego obteniendo datos de la API
        if (!juegoYaInicializado)
        {
            juegoYaInicializado = true;
            StartCoroutine(InicializarJuegoDesdeAPI());
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: hace que UIManager no se destruya entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Muestra el panel del portapapeles
    public void MostrarClipboard()
    {
        clipboardPanel.SetActive(true); // Activa el panel
        clipboardPanel.transform.SetAsLastSibling(); // Asegura que se muestre por encima de otros elementos UI

        // Muestra las opciones si no se han mostrado ya para este caso y el índice es válido
        if (!opcionesYaMostradas && indiceCaso >= 0 && indiceCaso < listaCasos.Count)
        {
            opcionesYaMostradas = true; // Marca las opciones como mostradas
            tabletOpcionesContainer.SetActive(true); // Activa el contenedor de opciones

            // Carga las opciones correspondientes al caso actual
            var caso = listaCasos[indiceCaso];
            opcionesManager.CargarOpcionesParaCaso(caso.id_caso);
        }
        else if (indiceCaso < 0 || indiceCaso >= listaCasos.Count) {
             Debug.LogWarning("Se intentó mostrar las opciones del clipboard, pero el índice del caso actual es inválido.");
        }
    }

    // Cierra el panel del portapapeles
    public void CerrarClipboard()
    {
        clipboardPanel.SetActive(false);
    }

    // Muestra el panel del libro
    public void MostrarLibro()
    {
        libroPanel.SetActive(true);
        libroPanel.transform.SetAsLastSibling();
    }

    // Cierra el panel del libro
    public void CerrarLibro()
    {
        libroPanel.SetActive(false);
    }

    // Llamado cuando se elige una opción de la tablet (probablemente invocado desde OpcionesManager al hacer clic en un botón)
    public void OpcionSeleccionada()
    {
        opcionesYaMostradas = false; // Resetea la bandera para el siguiente caso

        // Cierra el clipboard si está abierto
        if (clipboardPanel.activeSelf)
            clipboardPanel.SetActive(false);

        // Indica al líder que inicie su animación de salida
        if (liderMovimiento != null)
            liderMovimiento.SalirLider(); // Esto eventualmente llamará a CargarSiguienteCaso

        // Detiene la corutina del temporizador si está activa
        if (temporizadorCoroutine != null)
        {
            StopCoroutine(temporizadorCoroutine);
            temporizadorCoroutine = null;
            if (textoTemporizador != null) textoTemporizador.text = ""; // Limpia el texto del temporizador
        }
    }

    // Llamado por LiderMovimiento después de que termina su animación de salida
    public void CargarSiguienteCaso()
    {
        indiceCaso++; // Incrementa el índice para pasar al siguiente caso
         Debug.Log($"UIManager: Intentando cargar el siguiente caso. Nuevo índice: {indiceCaso}");

        // --- CONDICIÓN DE FIN DE JUEGO ---
        // Comprueba si el índice ha alcanzado o superado el número de casos a jugar
        if (listaCasos == null || indiceCaso >= listaCasos.Count) // Usa listaCasos.Count como límite
        {
            Debug.Log("UIManager: Todos los casos completados. Terminando juego.");

            // Calcula la puntuación final
            int puntajeFinal = 0;
             if (indicadoresManager != null) {
                 puntajeFinal = indicadoresManager.SumarValoresFinales(); // Suma los valores de los indicadores
                 Debug.Log($"UIManager: Puntuación final calculada: {puntajeFinal}");
             } else {
                 Debug.LogError("UIManager: ¡IndicadoresManager no encontrado, no se puede calcular la puntuación final!");
                 // Decide cómo proceder - ¿cargar una escena final por defecto?
             }

            // --- INICIA LA CORUTINA DE FINALIZACIÓN ---
            // Esta corutina se encargará de enviar la puntuación Y cargar la escena correcta
            StartCoroutine(FinalizeGameAndSendScore(puntajeFinal));
            // --- FIN INICIO CORUTINA DE FINALIZACIÓN ---

            // El método retorna aquí; la corutina se encargará de cargar la escena más tarde
            return;
        }
        // --- FIN CONDICIÓN DE FIN DE JUEGO ---


        // --- Cargar Siguiente Caso (La lógica permanece igual) ---
         // Añadir comprobaciones de límites de listas como antes...
         if (listaCasos == null || indiceCaso >= listaCasos.Count) { /* ... manejar error ... */ return; }
         if (ordenSpritesAleatorios == null || indiceCaso >= ordenSpritesAleatorios.Count) { /* ... manejar error ... */ return; }
         var caso = listaCasos[indiceCaso]; // Obtiene el objeto Caso actual
         int idSprite = ordenSpritesAleatorios[indiceCaso]; // Obtiene el índice del sprite para este caso
         if (idSprite < 0 || idSprite >= spritesLideres.Length) { /* ... manejar error ... */ return; }

        imagenLider.sprite = spritesLideres[idSprite]; // Asigna el sprite correcto a la imagen del líder
        liderMovimiento.IniciarEntrada();              // Inicia la animación de entrada del líder
        casoViewer.MostrarCaso(caso);                  // Muestra la información del caso en la UI

        // Reinicia el temporizador para el nuevo caso
        if (temporizadorCoroutine != null)
            StopCoroutine(temporizadorCoroutine);
        temporizadorCoroutine = StartCoroutine(TemporizadorDecision());
    }


    // Llamado por OpcionesManager cuando se hace clic en un botón de opción específico
    public void RegistrarDecision(int idOpcion)
    {
        // Este método actualmente solo registra en consola.
        // La progresión real del juego sucede en OpcionSeleccionada -> liderMovimiento.SalirLider -> CargarSiguienteCaso
        Debug.Log("UIManager: Decisión registrada para ID de Opción: " + idOpcion + " en Índice de Caso: " + indiceCaso);
        // Si necesitaras enviar *cada* decisión al backend inmediatamente, lo harías aquí.
        // Ejemplo: StartCoroutine(SendDecisionToAPI(idInstancia, idOpcion, indiceCaso));
    }


    // Corutina que maneja el temporizador para tomar una decisión
    private IEnumerator TemporizadorDecision()
    {
        float tiempoRestante = tiempoLimite; // Inicializa el tiempo

        // Bucle mientras quede tiempo
        while (tiempoRestante > 0f)
        {
            tiempoRestante -= Time.deltaTime; // Reduce el tiempo

            // Actualiza el texto del temporizador en la UI
            if (textoTemporizador != null)
            {
                int segundos = Mathf.Max(0, Mathf.CeilToInt(tiempoRestante)); // Asegura que no sea negativo
                int segs = segundos % 60; // Calcula los segundos para mostrar
                textoTemporizador.text = $"Tiempo restante: {segs:00}"; // Formato 00
            }

            yield return null; // Espera al siguiente frame
        }

        // El tiempo se acabó
        if (textoTemporizador != null) textoTemporizador.text = "Tiempo agotado";
        Debug.Log("UIManager: El temporizador se agotó. Ejecutando opción por defecto...");

        // Ejecuta automáticamente el clic en el primer botón si está disponible y es interactuable
        if (botonOpcion1 != null && botonOpcion1.interactable)
        {
            botonOpcion1.onClick.Invoke(); // Esto disparará la secuencia: AplicarImpacto -> RegistrarDecision -> OpcionSeleccionada
        } else {
            Debug.LogWarning("UIManager: El temporizador se agotó, pero el botón por defecto (Opción 1) no estaba disponible o no era interactuable.");
            // Opcionalmente, fuerza la progresión del juego sin aplicar impacto si falta el botón
             OpcionSeleccionada(); // Fuerza la progresión incluso sin hacer clic en un botón
        }
    }

    // Corutina para inicializar el juego obteniendo datos de la API
    private IEnumerator InicializarJuegoDesdeAPI()
    {
        Debug.Log("UIManager: Inicializando juego desde API.");

        // 1. Crear la nueva instancia de partida
        UnityWebRequest requestInstancia = UnityWebRequest.PostWwwForm($"{apiBaseUrl}/Videojuego/instancia/2", "");
        requestInstancia.certificateHandler = new ForceAcceptAll();
        yield return requestInstancia.SendWebRequest();

        if (requestInstancia.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("UIManager: ❌ Error creando instancia: " + requestInstancia.error);
            yield break;
        }

        // Parsear el ID de la nueva instancia
        InstanciaRespuesta data = JsonUtility.FromJson<InstanciaRespuesta>(requestInstancia.downloadHandler.text);
        idInstancia = data.id_instancia; // Guardamos el nuevo ID
        indicadoresManager.idInstancia = idInstancia; 

        Debug.Log($"UIManager: ✅ Instancia creada con ID {idInstancia}");

        // 2. Inicializar valores de indicadores usando la instancia
        UnityWebRequest initValores = UnityWebRequest.PostWwwForm($"{apiBaseUrl}/Videojuego/inicializar_valores/{idInstancia}", "");
        initValores.certificateHandler = new ForceAcceptAll();
        yield return initValores.SendWebRequest();

        if (initValores.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("UIManager: ❌ Error inicializando valores de indicadores: " + initValores.error);
            yield break;
        }
        Debug.Log("UIManager: ✅ Valores de indicadores inicializados para instancia " + idInstancia);

        // 3. Obtener todos los casos
        UnityWebRequest requestCasos = UnityWebRequest.Get($"{apiBaseUrl}/Videojuego");
        requestCasos.certificateHandler = new ForceAcceptAll();
        yield return requestCasos.SendWebRequest();

        if (requestCasos.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("UIManager: ❌ Error obteniendo casos: " + requestCasos.error);
            yield break;
        }

        try
        {
            listaCasos = JsonHelper.FromJson<Caso>(requestCasos.downloadHandler.text).ToList();
            ordenCasosAleatorios = listaCasos.Select(c => c.id_caso).OrderBy(x => Random.value).ToList();
            Debug.Log($"UIManager: ✅ Casos recibidos: {listaCasos.Count}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"UIManager: ❌ Error parseando casos: {ex.Message}");
            yield break;
        }

        // 4. Ordenar sprites de líderes
        if (spritesLideres == null || spritesLideres.Length == 0)
        {
            Debug.LogError("UIManager: ❌ Sprites de líderes no asignados.");
            yield break;
        }
        ordenSpritesAleatorios = Enumerable.Range(0, spritesLideres.Length).OrderBy(x => Random.value).ToList();

        // 5. Preparar primer caso
        indiceCaso = -1; 
        CargarSiguienteCaso();
    }



    // Métodos de ayuda si se necesitan en otros scripts
    public int GetOrdenDelCasoActual()
    {
        // Nota: indiceCaso es el índice 0-based del caso *cargado* actualmente
        return indiceCaso;
    }

    public int GetIdInstancia()
    {
        // Retorna el ID de la instancia actual del juego
        return idInstancia;
    }


    // --- NUEVA CORUTINA PARA MANEJAR ENVÍO DE PUNTUACIÓN Y CARGA DE ESCENA ---
    public IEnumerator FinalizeGameAndSendScore(int finalScore)
    {
        Debug.Log($"UIManager: Finalizando juego. Puntuación final: {finalScore}");

        if (idInstancia <= 0)
        {
            Debug.LogError("UIManager: ❌ ID de instancia inválido. No se puede actualizar puntaje.");
            yield break;
        }

        // Crear objeto JSON para actualizar el puntaje
        UpdateGameResultRequest data = new UpdateGameResultRequest(idInstancia, finalScore, idUsuario);

        string jsonData = JsonUtility.ToJson(data);

        Debug.Log($"📦 Datos que se enviarán a la API: idInstancia = {idInstancia}, puntuacion = {finalScore}, idUsuario = {idUsuario}");
        Debug.Log($"📜 JSON final: {jsonData}");


        // Enviar la actualización
        string urlUpdate = $"{apiBaseUrl}/Videojuego/UpdateGameResult";

        UnityWebRequest requestUpdate = new UnityWebRequest(urlUpdate, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        requestUpdate.uploadHandler = new UploadHandlerRaw(bodyRaw);
        requestUpdate.downloadHandler = new DownloadHandlerBuffer();
        requestUpdate.SetRequestHeader("Content-Type", "application/json");
        requestUpdate.certificateHandler = new ForceAcceptAll();

        yield return requestUpdate.SendWebRequest();

        if (requestUpdate.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("UIManager: ✅ Puntaje actualizado correctamente.");

            // Guardar puntaje final para mostrarlo en la escena de resultados
            PlayerPrefs.SetInt("PuntajeFinal", finalScore);
            PlayerPrefs.Save();
        }

        // Ahora mostrar la escena final como siempre
        string targetScene = "";

        if (finalScore >= winScoreThreshold)
        {
            targetScene = winSceneName;
        }
        else if (finalScore <= loseScoreThreshold)
        {
            targetScene = loseSceneName;
        }
        else
        {
            targetScene = alternateEndSceneName;
        }

        if (!string.IsNullOrEmpty(targetScene))
        {
            Debug.Log($"UIManager: Cargando escena final '{targetScene}'.");
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogError("UIManager: ❌ No se pudo determinar la escena final.");
        }
    }


    private IEnumerator ConsultarYMostrarMaximoPuntaje(int idUsuario, int idJuego, int nuevoPuntaje)
    {
        string url = $"{apiBaseUrl}/Score/MaximoPuntaje/{idUsuario}/{idJuego}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new ForceAcceptAll();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"UIManager: ❌ Error al consultar máximo puntaje: {request.error}");
            yield break;
        }

        try
        {
            MaximoPuntajeResponse response = JsonUtility.FromJson<MaximoPuntajeResponse>(request.downloadHandler.text);
            PlayerPrefs.SetInt("PuntajeMaximoHistorial", response.puntuacionMaxima);

            Debug.Log($"UIManager: 🏆 Puntaje máximo histórico: {response.puntuacionMaxima}");

            if (nuevoPuntaje > response.puntuacionMaxima)
            {
                Debug.Log("🎉 ¡Nuevo Mejor Puntaje!");
                PlayerPrefs.SetInt("NuevoRecord", 1); // Opcional, para usarlo en la escena final
            }
            else
            {
                Debug.Log($"👑 Mejor puntaje sigue siendo: {response.puntuacionMaxima} pts.");
                PlayerPrefs.SetInt("NuevoRecord", 0);
            }

            // 3. Después de mostrar el mensaje, cargar la escena final
            string targetScene = "";

            if (nuevoPuntaje >= winScoreThreshold)
            {
                targetScene = winSceneName;
            }
            else if (nuevoPuntaje <= loseScoreThreshold)
            {
                targetScene = loseSceneName;
            }
            else
            {
                targetScene = alternateEndSceneName;
            }

            if (!string.IsNullOrEmpty(targetScene))
            {
                Debug.Log($"UIManager: Cargando escena final '{targetScene}'.");
                SceneManager.LoadScene(targetScene);
            }
            else
            {
                Debug.LogError("UIManager: ❌ No se pudo determinar la escena final.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"UIManager: ❌ Error procesando respuesta de máximo puntaje: {ex.Message}");
        }
    }

}