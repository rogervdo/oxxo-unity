using UnityEngine;
using UnityEngine.UI;

public class PrefabLogroController : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image imagenLogro;       // Imagen del logro (el sprite en color)
    public Text tituloLogro;        // Título del logro
    public Button botonDetalle;     // Botón que muestra la descripción

    public void Configurar(LogroDTO logro, bool tieneLogro)
    {
        // Validar referencias
        if (imagenLogro == null || tituloLogro == null || botonDetalle == null)
        {
            Debug.LogError("❌ PrefabLogroController: Faltan referencias en el Inspector.");
            return;
        }

        // Asignar sprite original
        imagenLogro.sprite = tieneLogro ? logro.spriteOriginal : logro.spriteBloqueado;
        
        imagenLogro.color = Color.white;


        // Asignar título
        tituloLogro.text = logro.nombre;

        // Asignar evento de descripción
        botonDetalle.onClick.RemoveAllListeners();
        botonDetalle.onClick.AddListener(() =>
        {
            if (PanelDescripcionLogro.Instance != null)
                PanelDescripcionLogro.Instance.Mostrar(logro);
            else
                Debug.LogError("❌ No se encontró PanelDescripcionLogro.Instance al hacer click.");
        });
    }
}
