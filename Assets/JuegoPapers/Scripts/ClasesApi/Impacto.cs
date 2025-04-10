using UnityEngine;

[System.Serializable]
public class Impacto
{
    public int id_indicador;
    public string categoria;
    public int cambio_valor; // Este define si es tipo 1 (positivo), 2 (neutral), 3 (negativo)
    public string nombre;
    public int tipo; // 1: verde, 2: amarillo, 3: rojo
    public float cambio;
    public int id_tipo_opcion;
}

