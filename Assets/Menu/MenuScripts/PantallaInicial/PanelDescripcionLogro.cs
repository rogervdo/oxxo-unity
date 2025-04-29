using UnityEngine;
using UnityEngine.UI;

public class PanelDescripcionLogro : MonoBehaviour
{
    public static PanelDescripcionLogro Instance;

    [Header("Referencias UI")]
    public GameObject panelDescripcion;  // El panel visual que aparece
    public Text nombreTexto;             // Texto para el nombre del logro
    public Text descripcionTexto;        // Texto para la descripción del logro

    private void Awake()
    {
        // Establecer el singleton
        Instance = this;

        // Asegurar que el panel de descripción esté oculto al iniciar
        if (panelDescripcion != null)
        {
            panelDescripcion.SetActive(false);
        }
        else
        {
            Debug.LogError("❌ PanelDescripcionLogro: El 'panelDescripcion' no está asignado en el inspector.");
        }
    }

    public void Mostrar(LogroDTO logro)
    {
        if (panelDescripcion == null || nombreTexto == null || descripcionTexto == null)
        {
            Debug.LogError("❌ PanelDescripcionLogro: Faltan referencias UI en Mostrar().");
            return;
        }

        nombreTexto.text = logro.nombre;
        descripcionTexto.text = logro.descripcion;
        panelDescripcion.SetActive(true);
    }

    public void Cerrar()
    {
        if (panelDescripcion != null)
            panelDescripcion.SetActive(false);
    }
}
