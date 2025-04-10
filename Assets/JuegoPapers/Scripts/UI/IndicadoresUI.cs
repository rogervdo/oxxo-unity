using UnityEngine;
using UnityEngine.UI;

public class IndicadorUI : MonoBehaviour
{
    public Text nombreTexto;
    public Slider barraProgreso;

    // Este método te permite configurar el indicador al instanciarlo
    public void Configurar(string nombre, int valor)
    {
        nombreTexto.text = nombre;
        barraProgreso.maxValue = 15;
        barraProgreso.value = valor;
    }
}
