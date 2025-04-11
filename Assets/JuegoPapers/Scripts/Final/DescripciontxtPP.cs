using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DescripciontxtPP : MonoBehaviour
{
    public Text textoDestino;         // El Text donde aparecerá el efecto
    [TextArea]
    public string mensajeCompleto;    // El texto completo que quieres mostrar
    public float velocidad = 0.05f;   // Velocidad de escritura (menos es más rápido)

    void Start()
    {
        StartCoroutine(MostrarTextoLetraPorLetra());
    }

    IEnumerator MostrarTextoLetraPorLetra()
    {
        textoDestino.text = "";
        foreach (char letra in mensajeCompleto)
        {
            textoDestino.text += letra;
            yield return new WaitForSeconds(velocidad);
        }
    }
}
