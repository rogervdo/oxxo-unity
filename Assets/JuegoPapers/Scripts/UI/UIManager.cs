using UnityEngine;
using UnityEngine.UI;
using System.Collections;       // Necesario para Coroutines
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Networking;  // Necesario para el contexto de APIScoreSender

public class UIManager : MonoBehaviour
{
    [Header("Paneles")] // Encabezado para organizar en el Inspector
    public GameObject clipboardPanel; // Referencia al panel del portapapeles
    public GameObject libroPanel;     // Referencia al panel del libro
    private int idInstancia;          // ID de la instancia de juego actual

    [Header("API Configuration")] // Configuración de la API
    public string apiBaseUrl = "https://localhost:7058"; // URL base para llamadas API
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

    // --- Start y otros métodos permanecen en gran parte iguales ---
    private void Start()
    {
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
        StartCoroutine(InicializarJuegoDesdeAPI());
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
        // --- 1. Crear Instancia ---
        // Usa la variable apiBaseUrl definida a nivel de clase
        UnityWebRequest request = UnityWebRequest.PostWwwForm($"{apiBaseUrl}/Videojuego/instancia/2", "");
        request.certificateHandler = new ForceAcceptAll(); // Necesario si usas HTTPS con certificado local/inválido
        yield return request.SendWebRequest(); // Envía la solicitud y espera respuesta

        // Comprueba si hubo error en la solicitud
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("UIManager: Error creando instancia de juego: " + request.error);
            // Manejar el error: quizás mostrar un mensaje, deshabilitar el juego, volver al menú principal
            yield break; // Termina la corutina si falla
        }

        // Parsea la respuesta para obtener el ID de la instancia
        InstanciaRespuesta data = JsonUtility.FromJson<InstanciaRespuesta>(request.downloadHandler.text);
        idInstancia = data.id_instancia;
        Debug.Log($"UIManager: Instancia de juego creada con ID: {idInstancia}");


        // --- 2. Inicializar Valores de Indicadores ---
        UnityWebRequest initValores = UnityWebRequest.PostWwwForm($"{apiBaseUrl}/Videojuego/inicializar_valores/{idInstancia}", "");
        initValores.certificateHandler = new ForceAcceptAll();
        yield return initValores.SendWebRequest();

        if (initValores.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("UIManager: Error inicializando valores de indicadores: " + initValores.error);
            yield break;
        }
        Debug.Log($"UIManager: Valores de indicadores inicializados para instancia {idInstancia}");


        // --- 3. Mostrar Indicadores Iniciales ---
        indicadoresManager.idInstancia = idInstancia; // Pasa el ID de instancia al manager de indicadores
        indicadoresManager.MostrarIndicadores(); // Pide al manager que muestre los indicadores en la UI


        // --- 4. Obtener Todos los Casos ---
        UnityWebRequest requestCasos = UnityWebRequest.Get($"{apiBaseUrl}/Videojuego");
        requestCasos.certificateHandler = new ForceAcceptAll();
        yield return requestCasos.SendWebRequest();

        if (requestCasos.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("UIManager: Error obteniendo casos de juego: " + requestCasos.error);
            yield break;
        }

        // Intenta parsear la lista de casos desde el JSON recibido
        try {
            listaCasos = JsonHelper.FromJson<Caso>(requestCasos.downloadHandler.text).ToList();
            if (listaCasos == null || listaCasos.Count == 0) {
                 Debug.LogError("UIManager: Falló el parseo de casos o no se recibieron casos de la API.");
                 yield break;
            }
             // Crea una lista con los IDs de los casos y la desordena aleatoriamente
            ordenCasosAleatorios = listaCasos.Select(c => c.id_caso).OrderBy(x => Random.value).ToList();
            Debug.Log($"UIManager: Recibidos {listaCasos.Count} casos. Orden aleatorio generado.");

        } catch (System.Exception ex) {
             Debug.LogError($"UIManager: Error parseando JSON de casos: {ex.Message}. JSON: {requestCasos.downloadHandler.text}");
             yield break;
        }


        // --- 5. Ordenar Sprites Aleatoriamente ---
        // Asegúrate que el array spritesLideres esté asignado y tenga elementos en el Inspector
         if (spritesLideres == null || spritesLideres.Length == 0) {
             Debug.LogError("UIManager: ¡El array 'Sprites Lideres' no está asignado o está vacío en el Inspector!");
             // Decide cómo manejar esto - ¿usar uno por defecto? ¿detener?
             yield break;
         }
         // Crea una lista de índices (0, 1, 2, ...) hasta el número de sprites disponibles y la desordena
        ordenSpritesAleatorios = Enumerable.Range(0, spritesLideres.Length).OrderBy(x => Random.value).ToList();


        // --- Iniciar Juego ---
        indiceCaso = -1; // Empieza antes del primer caso (-1 para que el primer incremento sea 0)
        CargarSiguienteCaso(); // Carga el primer caso
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
    private IEnumerator FinalizeGameAndSendScore(int finalScore)
    {
        Debug.Log($"UIManager: Finalizando juego. Puntuación: {finalScore}. Intentando enviar puntuación vía API...");

        // --- Opcional: Guardar puntuación para mostrar en la escena final ---
        // Si tus escenas Win/Lose/Alt necesitan mostrar la puntuación, guárdala aquí.
        PlayerPrefs.SetInt("PuntajeFinal", finalScore);
        PlayerPrefs.Save(); // Asegura que se guarde inmediatamente si la necesita la siguiente escena

        // --- Intentar enviar puntuación a la API ---
        int? userId = UserManager.Instance?.CurrentUserId; // Obtiene el ID del usuario logueado (si existe)

        // Comprueba si hay un usuario logueado
        if (userId.HasValue)
        {
            // Construye la URL completa de la API para enviar puntuaciones
            string fullApiUrl = "";
            if (!string.IsNullOrEmpty(apiBaseUrl) && !string.IsNullOrEmpty(scoreApiEndpointPath))
            {
                fullApiUrl = apiBaseUrl.TrimEnd('/') + "/" + scoreApiEndpointPath.TrimStart('/'); // Construye URL segura
                Debug.Log($"UIManager: Enviando puntuación a {fullApiUrl}");

                // Inicia la corutina de envío de APIScoreSender Y ESPERA a que termine
                // APIScoreSender debe ser accesible (estático o una instancia)
                // Asegúrate que APIScoreSender.SendScore es una Corutina (IEnumerator)
                yield return StartCoroutine(APIScoreSender.SendScore(fullApiUrl, userId.Value, this.scoreGameId, finalScore));
                // La ejecución se reanuda aquí *después* de que APIScoreSender.SendScore termine su solicitud web

                Debug.Log("UIManager: Intento de APIScoreSender finalizado.");
            }
            else
            {
                Debug.LogError("UIManager: No se puede construir la URL de la API de puntuación - ¡apiBaseUrl o scoreApiEndpointPath faltan o están vacíos! Puntuación no enviada.");
                // Espera un breve momento incluso si la API falla, para evitar un cambio de escena abrupto
                yield return new WaitForSeconds(0.2f);
            }
        }
        else
        {
            Debug.LogWarning("UIManager: No hay usuario logueado (UserManager). Puntuación no enviada.");
            // Espera un breve momento incluso si no se envía, para evitar un cambio de escena abrupto
            yield return new WaitForSeconds(0.2f);
        }

        // --- Cargar la escena final apropiada BASADA EN LA PUNTUACIÓN ---
        string targetScene = ""; // Variable para almacenar el nombre de la escena a cargar
        // Comprueba la puntuación contra los umbrales definidos
        if (finalScore >= winScoreThreshold)
        {
            targetScene = winSceneName; // Escena de victoria
        }
        else if (finalScore <= loseScoreThreshold)
        {
            targetScene = loseSceneName; // Escena de derrota
        }
        else
        {
            targetScene = alternateEndSceneName; // Escena alternativa
        }

        // Valida y carga la escena objetivo
        if (!string.IsNullOrEmpty(targetScene))
        {
            Debug.Log($"UIManager: Cargando escena final '{targetScene}' basada en puntuación {finalScore}.");
            SceneManager.LoadScene(targetScene); // Carga la escena determinada
        }
        else
        {
            Debug.LogError($"UIManager: ¡No se pudo determinar la escena objetivo o el nombre de la escena está vacío! Revisa los umbrales y las variables de nombre de escena. Puntuación fue {finalScore}.");
            // Quizás cargar una escena de error por defecto o el menú principal aquí
        }
    }
    // --- FIN NUEVA CORUTINA ---
}