using UnityEngine;

[System.Serializable]
public class SaveGameResultRequest
{
    public int idUsuario;
    public int idJuego;
    public int puntuacion;

    public SaveGameResultRequest(int idUsuario, int idJuego, int puntuacion)
    {
        this.idUsuario = idUsuario;
        this.idJuego = idJuego;
        this.puntuacion = puntuacion;
    }
}
