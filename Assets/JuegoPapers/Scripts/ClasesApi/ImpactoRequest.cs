using UnityEngine;

[System.Serializable]
public class ImpactoRequest
{
    public int id_instancia;
    public int id_opcion;

    public ImpactoRequest(int idOpcion, int idInstancia)
    {
        this.id_opcion = idOpcion;
        this.id_instancia = idInstancia;
    }
}
