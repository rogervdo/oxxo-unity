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
    public class PuntajeRequestBody
    {
        public int idUsuario;
        public int idJuego;
        public int puntuacion;
    }

    private float tiempoActual;
    private bool tiempoActivado = false;
    public delegate void TiempoFinalizadoDelegate();
    public event TiempoFinalizadoDelegate OnTiempoFinalizado;

    public event System.Action OnTiempoFinalizado1;
    public event System.Action OnTiempoFinalizado2;
    public event System.Action OnTiempoFinalizado3;

    public int signoActivo = 1;

    private bool resultadoEnviado = false;

    private void Start()
    {
        APIManagerEspacial.Instance.IniciarJuegoEspacial();
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

        if (GameSessionManager.Instance.ObtenerVidas() <= 0 && !resultadoEnviado)
        {
            resultadoEnviado = true;
            StartCoroutine(GuardarResultadoFinal());
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

    public void GuardarResultadoFinalYTerminar2()
    {
        if (resultadoEnviado) return;
        resultadoEnviado = true;

        StartCoroutine(GuardarResultadoFinal());
    }

    private IEnumerator GuardarResultadoFinal()
    {
        int puntajeFinal = GameSessionManager.Instance.ObtenerPuntaje();
        int idUsuario = UserManager.Instance != null ? UserManager.Instance.GetCurrentUser2() : 0;

        Debug.Log($"✅ Guardando solo el puntaje. Puntaje: {puntajeFinal}, Usuario: {idUsuario}");

        // Mandar solo el puntaje directamente
        string urlGuardarPuntaje = APIManagerEspacial.Instance.apiBaseUrl + "/Score/SaveGameResult";

        PuntajeRequestBody data = new PuntajeRequestBody
        {
            idUsuario = idUsuario,
            idJuego = 1,
            puntuacion = puntajeFinal
        };

        string jsonData = JsonUtility.ToJson(data);
        Debug.Log($"🚀 Enviando JSON de Puntaje: {jsonData}");

        UnityWebRequest request = new UnityWebRequest(urlGuardarPuntaje, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new ForceAcceptAll();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Puntaje guardado correctamente.");
        }
        else
        {
            Debug.LogError($"❌ Error al guardar puntaje: {request.error}");
        }

        yield return new WaitForSeconds(0.5f); // Dar tiempo visual

        // Ahora cambiar de escena
        Debug.Log("✅ Cambiando a FinalPreguntas...");
        SceneManager.LoadScene("FinalPreguntas");
    }
}
