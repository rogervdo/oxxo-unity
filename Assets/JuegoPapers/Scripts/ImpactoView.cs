using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImpactoView : MonoBehaviour
{
    public GameObject[] botones; // ButtonOption1, 2, 3

    public void MostrarImpactos(List<Impacto> impactos, int botonIndex)
    {
        Transform contenedorIndicadores = botones[botonIndex].transform.Find("Indicadores");
        if (contenedorIndicadores == null)
        {
            Debug.LogWarning("Contenedor 'Indicadores' no encontrado");
            return;
        }

        // Mostrar solo los primeros 4 impactos
        for (int i = 0; i < 4; i++)
        {
            Text textoIndicador = contenedorIndicadores.GetChild(i).GetComponent<Text>();
            Impacto impacto = impactos[i];

            // Texto del impacto
            string valorTexto = impacto.cambio_valor > 0 ? "+" + impacto.cambio_valor.ToString() : impacto.cambio_valor.ToString();
            textoIndicador.text = impacto.nombre + ": " + valorTexto;

            // Color según tipo
            if (impacto.cambio_valor > 0)
                textoIndicador.color = new Color(0f, 0.556f, 0.121f); // Impacto positivo
            else if (impacto.cambio_valor < 0)
                textoIndicador.color = new Color(0.647f, 0.055f, 0f);// Impacto negativo
            else
                textoIndicador.color = new Color(0.682f, 0.557f, 0.000f);
        }
    }
}


