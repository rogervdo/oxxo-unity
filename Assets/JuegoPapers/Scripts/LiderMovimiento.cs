using UnityEngine;
using UnityEngine.UI;

public class LiderMovimiento : MonoBehaviour
{
    public RectTransform liderTransform;
    public float velocidad = 800f;
    public Vector2 posicionInicio = new Vector2(-1200, 0); // Fuera de pantalla izquierda
    public Vector2 posicionCentro = new Vector2(0, 0);     // Centro visible
    public Vector2 posicionSalida = new Vector2(-1200, 0); // Vuelve a salir por izquierda
    private bool esperandoDecision = false;
    private bool enMovimiento = false;
    public UIManager uiManager;



    private System.Collections.IEnumerator EntrarLider()
    {
        enMovimiento = true;
        while (Vector2.Distance(liderTransform.anchoredPosition, posicionCentro) > 0.1f)
        {
            liderTransform.anchoredPosition = Vector2.MoveTowards(
                liderTransform.anchoredPosition,
                posicionCentro,
                velocidad * Time.deltaTime
            );
            yield return null;
        }

        liderTransform.anchoredPosition = posicionCentro;
        enMovimiento = false;
        esperandoDecision = true;
    }

    public void SalirLider()
    {
        if (!esperandoDecision || enMovimiento) return;

        StartCoroutine(SalirLiderAnimado());
    }

    private System.Collections.IEnumerator SalirLiderAnimado()
    {
        enMovimiento = true;
        esperandoDecision = false;

        // Voltear horizontalmente (flip)
        Vector3 escala = liderTransform.localScale;
        escala.x = -1;
        liderTransform.localScale = escala;

        while (Vector2.Distance(liderTransform.anchoredPosition, posicionSalida) > 0.1f)
        {
            liderTransform.anchoredPosition = Vector2.MoveTowards(
                liderTransform.anchoredPosition,
                posicionSalida,
                velocidad * Time.deltaTime
            );
            yield return null;
        }

        liderTransform.anchoredPosition = posicionSalida;
        enMovimiento = false;

        // Aquí puedes llamar al siguiente paso o cargar el siguiente líder
        yield return new WaitForSeconds(1f);
        uiManager.CargarSiguienteCaso(); // Llama al siguiente caso

    }

    public void IniciarEntrada()
    {
        StartCoroutine(EntrarLider());
    }

}
