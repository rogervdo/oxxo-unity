using UnityEngine;
using UnityEngine.UI;


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





}
