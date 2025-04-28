using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class ControladorJuegoEspacial : MonoBehaviour
{
    [SerializeField] private float tiempoMaximo;
    [SerializeField] private Slider slider;
    [SerializeField] private Ui_LivesControll vidasUI;

    [System.Serializable]
    public class InstanciaRespuesta
    {
        public int id_instancia;
    }

    private float tiempoActual;
    private bool tiempoActivado = false;
    public delegate void TiempoFinalizadoDelegate();
    public event TiempoFinalizadoDelegate OnTiempoFinalizado;

    public event System.Action OnTiempoFinalizado1;
    public event System.Action OnTiempoFinalizado2;
    public event System.Action OnTiempoFinalizado3;

    public int signoActivo = 1; // Valor que defines según el signo activo

    private void Start()
    {
        APIManagerEspacial.Instance.IniciarJuegoEspacial(); // ✅ Solo carga las preguntas espaciales
    }

    private void Update()
    {
        if (tiempoActivado)
        {
            CambiarContador();
        }
    }

    private void CambiarContador()
    {
        tiempoActual -= Time.deltaTime;
        if (tiempoActual >= 0)
        {
            slider.value = tiempoActual;
        }
        if (tiempoActual <= 0)
        {
            CambiarTemporizador(false);
            OnTiempoFinalizado?.Invoke();
            SpendLives();
        }
    }

    private void CambiarTemporizador(bool estado)
    {
        tiempoActivado = estado;
    }

    public void ActivarTemporizador()
    {
        tiempoActual = tiempoMaximo;
        slider.maxValue = tiempoMaximo;
        CambiarTemporizador(true);
    }

    public void DesactivarTemporizador()
    {
        CambiarTemporizador(false);
    }

    public void SpendLives()
    {
        GameSessionManager.Instance.QuitarVida();
        vidasUI.UpdateLives();

        // Si ya no tiene vidas, termina el juego
        if (GameSessionManager.Instance.ObtenerVidas() <= 0)
        {
            GuardarResultadoFinalYTerminar2();
            return;
        }

        switch (signoActivo)
        {
            case 1:
                OnTiempoFinalizado1?.Invoke();
                break;
            case 2:
                OnTiempoFinalizado2?.Invoke();
                break;
            case 3:
                OnTiempoFinalizado3?.Invoke();
                break;
        }
    }

    // Si quieres terminar SIN guardar puntaje
    public void GuardarResultadoFinalYTerminar()
    {
        SceneManager.LoadScene("FinalPreguntas");
    }

    // ✅ Esta es la versión correcta: guarda puntaje e instancia
    public void GuardarResultadoFinalYTerminar2()
    {
        int puntajeFinal = GameSessionManager.Instance.ObtenerPuntaje();
        int idUsuario = UserManager.Instance.GetCurrentUser2();

        StartCoroutine(CrearInstanciaYGuardarResultado(puntajeFinal, idUsuario));
    }

    // ✅ Crear instancia de juego espacial Y guardar puntaje al mismo tiempo
    private IEnumerator CrearInstanciaYGuardarResultado(int puntajeFinal, int idUsuario)
    {
        // 1. Crear la instancia
        string urlCrearInstancia = "https://localhost:7058/Videojuego/instancia/1"; // ID 1 = Juego espacial
        UnityWebRequest requestInstancia = UnityWebRequest.PostWwwForm(urlCrearInstancia, "");
        requestInstancia.certificateHandler = new ForceAcceptAll();
        yield return requestInstancia.SendWebRequest();

        if (requestInstancia.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Error al crear instancia de juego: " + requestInstancia.error);
            yield break;
        }

        // 2. Extraer el id_instancia creado
        InstanciaRespuesta respuesta = JsonUtility.FromJson<InstanciaRespuesta>(requestInstancia.downloadHandler.text);
        int idInstancia = respuesta.id_instancia;

        Debug.Log($"✅ Instancia creada correctamente. ID: {idInstancia}");

        // 3. Guardar el puntaje
        string urlGuardarPuntaje = "https://localhost:7058/Score/SaveGameResult";

        WWWForm form = new WWWForm();
        form.AddField("puntaje", puntajeFinal);
        form.AddField("idInstancia", idInstancia);
        form.AddField("id_usuario", idUsuario);

        UnityWebRequest requestPuntaje = UnityWebRequest.Post(urlGuardarPuntaje, form);
        requestPuntaje.certificateHandler = new ForceAcceptAll();
        yield return requestPuntaje.SendWebRequest();

        if (requestPuntaje.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Puntaje guardado correctamente.");
            SceneManager.LoadScene("Escena_Ganar_Q"); // 🎯 Cargar la escena de final de juego
        }
        else
        {
            Debug.LogError("❌ Error al guardar puntaje: " + requestPuntaje.error);
        }
    }
}
