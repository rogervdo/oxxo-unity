using UnityEngine;

[System.Serializable]
public class Indicador
{
    public int id_indicador;
    public string nombre;
    public string unidad;
    public string categoria;
    public int impacto_total;

    public Indicador(string nombre, int valor)
    {
        this.nombre = nombre;
        this.impacto_total = valor;
    }
}

