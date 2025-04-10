using UnityEngine;
using UnityEngine.UI;

public class QuestionMark : MonoBehaviour
{
    [SerializeField] private ControladorJuegoEspacial ControladorJuegoEspacial;
    public GameObject TextUIPreguntas;
    public GameObject panelPreguntas;
    public Text preguntaText;

    public GameObject timePanel;
    public GameObject signoPregunta;
    public int numeroSigno = 1;

    public Button[] botonesOpciones; // Asigna los 3 botones desde el inspector

    private bool playerIsNear = false;

    void Start()
    {
        if (TextUIPreguntas != null)
            TextUIPreguntas.SetActive(false);

        if (panelPreguntas != null)
            panelPreguntas.SetActive(false);

        if (ControladorJuegoEspacial != null)
        {
            switch (numeroSigno)
            {
                case 1:
                    ControladorJuegoEspacial.OnTiempoFinalizado1 += CerrarPanelPregunta;
                    break;
                case 2:
                    ControladorJuegoEspacial.OnTiempoFinalizado2 += CerrarPanelPregunta;
                    break;
                case 3:
                    ControladorJuegoEspacial.OnTiempoFinalizado3 += CerrarPanelPregunta;
                    break;
            }
        }
    }

    void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            TextUIPreguntas?.SetActive(false);
            panelPreguntas?.SetActive(true);
            timePanel?.SetActive(true);

            if (ControladorJuegoEspacial != null)
            {
                ControladorJuegoEspacial.signoActivo = numeroSigno;
                ControladorJuegoEspacial.ActivarTemporizador();
            }

            CargarPreguntaYOpciones(numeroSigno - 1); // índice del caso
        }
    }

    private void CargarPreguntaYOpciones(int indice)
    {
        var preguntas = APIManagerEspacial.Instance.preguntasEspaciales;

        if (indice < 0 || indice >= preguntas.Count)
        {
            Debug.LogWarning("Índice de pregunta inválido");
            return;
        }

        var pregunta = preguntas[indice];
        preguntaText.text = "Pregunta: " + pregunta.texto;

        StartCoroutine(APIManagerEspacial.Instance.ObtenerRespuestas(pregunta.id_pregunta, opciones =>
        {
            if (opciones == null || opciones.Count == 0)
            {
                Debug.LogWarning("No se encontraron opciones");
                return;
            }

            int cantidadOpciones = Mathf.Min(opciones.Count, botonesOpciones.Length);

            for (int i = 0; i < cantidadOpciones; i++)
            {
                int index = i;
                int idOpcion = opciones[index].id_respuesta;
                bool esCorrecta = opciones[index].es_correcta;

                botonesOpciones[index].GetComponentInChildren<Text>().text = opciones[index].texto;

                botonesOpciones[index].onClick.RemoveAllListeners();
                botonesOpciones[index].onClick.AddListener(() =>
                {
                    if (!esCorrecta)
                    {
                        var controlador = FindFirstObjectByType<ControladorJuegoEspacial>();
                        if (controlador != null)
                        {
                            controlador.SpendLives();
                        }
                    }
                    else
                    {
                        Debug.Log("✅ ¡Respuesta correcta!");
                    }

                    var ui = FindFirstObjectByType<UIControllerEspacio>();
                    if (ui != null)
                    {
                        ui.RegistrarDecision(idOpcion);
                    }

                    CerrarPanelPregunta();
                });
            }

            // Oculta botones sobrantes si hay menos de 3 opciones
            for (int i = cantidadOpciones; i < botonesOpciones.Length; i++)
            {
                botonesOpciones[i].gameObject.SetActive(false);
            }
        }));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            TextUIPreguntas?.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            TextUIPreguntas?.SetActive(false);
        }
    }

    private void CerrarPanelPregunta()
    {
        if (panelPreguntas != null) panelPreguntas.SetActive(false);
        if (timePanel != null) timePanel.SetActive(false);
        if (signoPregunta != null) signoPregunta.SetActive(false);

        // Vuelve a mostrar todos los botones para la próxima pregunta
        foreach (var boton in botonesOpciones)
        {
            if (boton != null)
                boton.gameObject.SetActive(true);
        }
    }
}
