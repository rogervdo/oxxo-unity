using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject clipboardPanel;
    public GameObject libroPanel;

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


    private void Start()
    {
        indiceCaso = -1; // Para que al llamar CargarSiguienteCaso suba a 0
        CargarSiguienteCaso(); // Esto inicia el primer caso y el temporizador
    }

    // Mostrar clipboard
    public void MostrarClipboard()
    {
        clipboardPanel.SetActive(true);
        clipboardPanel.transform.SetAsLastSibling();

        // Mostrar opciones solo la primera vez
        if (!opcionesYaMostradas)
        {
            tabletOpcionesContainer.SetActive(true);
            opcionesYaMostradas = true;
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

        // Validar límite del array ANTES de usar el índice
        if (indiceCaso >= spritesLideres.Length)
        {
            Debug.Log("Juego terminado");
            // Mostrar final, pantalla de resumen, etc.
            return;
        }

        // Reset del líder
        Vector3 escala = imagenLider.rectTransform.localScale;
        escala.x = 1;
        imagenLider.rectTransform.localScale = escala;

        // Cargar sprite válido
        imagenLider.sprite = spritesLideres[indiceCaso];
        imagenLider.rectTransform.anchoredPosition = liderMovimiento.posicionInicio;

        liderMovimiento.IniciarEntrada();

        // Reiniciar temporizador
        if (temporizadorCoroutine != null)
            StopCoroutine(temporizadorCoroutine);

        temporizadorCoroutine = StartCoroutine(TemporizadorDecision());
    }



    private System.Collections.IEnumerator TemporizadorDecision()
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

}
