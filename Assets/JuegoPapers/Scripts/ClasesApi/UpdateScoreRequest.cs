using UnityEngine;

[System.Serializable]
public class UpdateGameResultRequest
{
    public int idInstancia;
    public int puntuacion;
    public int idUsuario;

    public UpdateGameResultRequest(int idInstancia, int puntuacion, int idUsuario)
    {
        this.idInstancia = idInstancia;
        this.puntuacion = puntuacion;
        this.idUsuario = idUsuario;
    }
}



