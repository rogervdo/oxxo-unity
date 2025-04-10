using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Opcion
{
    public int id_opcion;
    public string texto_opcion;
    public int sprite_resultado;
    public int? id_tipo_opcion;
    public List<ImpactoIndicador> impactos; // <-- NUEVO
}

