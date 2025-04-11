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

    private float tiempoActual;
    private bool tiempoActivado = false;
    public delegate void TiempoFinalizadoDelegate();
    public event TiempoFinalizadoDelegate OnTiempoFinalizado;

    public event System.Action OnTiempoFinalizado1;
    public event System.Action OnTiempoFinalizado2;
    public event System.Action OnTiempoFinalizado3;

    public int signoActivo = 1; // valor que defines según el signo activo


   
   private void CambiarContador()
   {
    tiempoActual -= Time.deltaTime;
    if (tiempoActual >= 0)
    {
        slider.value = tiempoActual;
    }
    if(tiempoActual <= 0)
    {
        CambiarTemporizador(false);
        OnTiempoFinalizado?.Invoke(); // Notifica a quien esté suscrito
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

    private void Start()
    {
        APIManagerEspacial.Instance.IniciarJuegoEspacial();

    }

    private void Update()
    {
        if(tiempoActivado)
        {
            CambiarContador();
        }
    }

    public void SpendLives()
{
    GameSessionManager.Instance.QuitarVida();
    vidasUI.UpdateLives();

    // 💥 Si ya no tiene vidas, termina el juego
    if (GameSessionManager.Instance.ObtenerVidas() <= 0)
    {
        SceneManager.LoadScene("FinalPreguntas");
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

    public void GuardarResultadoFinalYTerminar()
    {
        SceneManager.LoadScene("FinalPreguntas");
    }

    public void GuardarResultadoFinalYTerminar2()
{
    int puntajeFinal = GameSessionManager.Instance.ObtenerPuntaje();
    int idInstancia = APIManager.Instance.idInstancia;
    int idUsuario = UserManager.Instance.GetCurrentUser2(); // 👈 Agregado

    StartCoroutine(EnviarResultado(puntajeFinal, idInstancia, idUsuario));
}


private IEnumerator EnviarResultado(int puntaje, int idInstancia, int idUsuario)
{
    string url = "https://localhost:7058/Score/SaveGameResult";

    WWWForm form = new WWWForm();
    form.AddField("puntaje", puntaje);
    form.AddField("idInstancia", idInstancia);
    form.AddField("id_usuario", idUsuario); // 👈 Enviar también el usuario

    UnityWebRequest request = UnityWebRequest.Post(url, form);
    request.certificateHandler = new ForceAcceptAll();
    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
        Debug.Log("✅ Puntaje guardado correctamente");
        SceneManager.LoadScene("Escena_Ganar_Q");
    }
    else
    {
        Debug.LogError("❌ Error al guardar puntaje: " + request.error);
    }
}
}





