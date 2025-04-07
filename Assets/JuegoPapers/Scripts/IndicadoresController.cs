using UnityEngine;
using UnityEngine.UI;

public class IndicadoresController : MonoBehaviour
{
    public RectTransform hojaTransform;        // El panel que se moverá (la hoja)
    public float velocidad = 500f;              // Velocidad del movimiento
    public Vector2 posicionOculta;             // Posición cuando está escondida
    public Vector2 posicionVisible;            // Posición cuando está desplegada
    public Image flechaIcono;                  // Imagen de la flecha
    public Sprite flechaArriba;
    public Sprite flechaAbajo;

    private bool desplegado = false;
    private bool enTransicion = false;

    public void ToggleIndicadores()
    {
        if (enTransicion) return;
        StartCoroutine(MoverHoja());
    }

    private System.Collections.IEnumerator MoverHoja()
    {
        enTransicion = true;

        Vector2 objetivo = desplegado ? posicionOculta : posicionVisible;
        Sprite nuevaFlecha = desplegado ? flechaArriba : flechaAbajo;

        while (Vector2.Distance(hojaTransform.anchoredPosition, objetivo) > 0.1f)
        {
            hojaTransform.anchoredPosition = Vector2.MoveTowards(
                hojaTransform.anchoredPosition,
                objetivo,
                velocidad * Time.deltaTime
            );
            yield return null;
        }

        hojaTransform.anchoredPosition = objetivo;
        flechaIcono.sprite = nuevaFlecha;
        desplegado = !desplegado;
        enTransicion = false;
    }
}
