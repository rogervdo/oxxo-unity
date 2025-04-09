using UnityEngine;
using UnityEngine.UI;

public class QuestionMark : MonoBehaviour
{
    [SerializeField]private ControladorJuegoEspacial ControladorJuegoEspacial;
    public GameObject TextUIPreguntas;     // Texto tipo "Presiona E para responder"
    public GameObject panelPreguntas;    // Panel donde va la pregunta
    public Text preguntaText;            // Componente de texto dentro del panel

    public GameObject timePanel;
    public GameObject signoPregunta; // arrástralo desde el inspector
    public int numeroSigno = 1; // Asigna 1, 2 o 3 desde Unity



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
            if (TextUIPreguntas != null)
                TextUIPreguntas.SetActive(false);

            if (panelPreguntas != null)
                panelPreguntas.SetActive(true);

            if (timePanel != null)
                timePanel.SetActive(true);

            if (ControladorJuegoEspacial != null)
            {
                ControladorJuegoEspacial.signoActivo = numeroSigno; // ⬅️ Indica qué signo fue activado
                ControladorJuegoEspacial.ActivarTemporizador();
            }

            if (preguntaText != null)
                preguntaText.text = "Pregunta: ¿Cuál es la capital de Francia?";
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            if (TextUIPreguntas != null)
                TextUIPreguntas.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            if (TextUIPreguntas != null)
                TextUIPreguntas.SetActive(false);
        }
    }
    private void CerrarPanelPregunta()
    {
        if (panelPreguntas != null)
            panelPreguntas.SetActive(false);

        if (timePanel != null)
            timePanel.SetActive(false);
        
        if (signoPregunta != null)
        signoPregunta.SetActive(false);
    }

}
