using UnityEngine;

[System.Serializable]
public class Indicador
{
    public int id_indicador;
    public string nombre;
    public string unidad;
    public string categoria;
    public int valor_actual ;

    public Indicador(string nombre, int valor)
    {
        this.nombre = nombre;
        this.valor_actual  = valor;
    }
}

