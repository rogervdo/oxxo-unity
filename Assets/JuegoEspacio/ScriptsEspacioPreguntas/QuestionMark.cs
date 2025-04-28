using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // 🔥 Necesario para IEnumerator

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

            CargarPreguntaYOpciones(numeroSigno - 1);
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
                    var controlador = FindFirstObjectByType<ControladorJuegoEspacial>();

                    GameSessionManager.Instance?.AumentarPreguntasRespondidas();

                    if (!esCorrecta)
                    {
                        if (controlador != null)
                        {
                            controlador.SpendLives();

                            if (GameSessionManager.Instance.ObtenerVidas() <= 0)
                            {
                                if (controlador != null)
                                {
                                    controlador.GuardarResultadoFinalYTerminar2();
                                }
                                StartCoroutine(CambiarEscenaFinalDespuesDeGuardar());
                                return;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("✅ ¡Respuesta correcta!");
                        int puntos = pregunta.puntaje;
                        GameSessionManager.Instance?.AgregarPuntos(puntos);
                        GameSessionManager.Instance?.AgregarPuntosPreguntas(puntos);
                        GameSessionManager.Instance?.AumentarRespuestasCorrectas();
                    }

                    if (GameSessionManager.Instance != null &&
                        GameSessionManager.Instance.preguntasRespondidas >= 6)
                    {
                        if (controlador != null)
                        {
                            controlador.GuardarResultadoFinalYTerminar2();
                        }
                        StartCoroutine(CambiarEscenaFinalDespuesDeGuardar());
                        return;
                    }

                    var ui = FindFirstObjectByType<UIControllerEspacio>();
                    if (ui != null)
                    {
                        ui.RegistrarDecision(idOpcion);
                    }

                    CerrarPanelPregunta();
                });
            }

            for (int i = cantidadOpciones; i < botonesOpciones.Length; i++)
            {
                botonesOpciones[i].gameObject.SetActive(false);
            }
        }));
    }

    private IEnumerator CambiarEscenaFinalDespuesDeGuardar()
    {
        yield return new WaitForSeconds(1.0f); // 🔥 Tiempo de seguridad para guardar
        Debug.Log("✅ Cambiando a escena FinalPreguntas...");
        SceneManager.LoadScene("FinalPreguntas");
    }

    private void DesactivarBotones()
    {
        foreach (var boton in botonesOpciones)
        {
            if (boton != null)
                boton.interactable = false;
        }
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

        foreach (var boton in botonesOpciones)
        {
            if (boton != null)
                boton.gameObject.SetActive(true);
        }
    }
}
