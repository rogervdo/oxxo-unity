using UnityEngine;
using UnityEngine.SceneManagement;

public class TestButtonController : MonoBehaviour
{
    // Variable para almacenar el resultado del juego (0 = derrota, 1 = victoria).
    public int isWin = 0;

    // Establece el resultado como victoria y carga la escena de Game Over.
    public void WinGame()
    {
        isWin = 1; // Marca como victoria.
        PlayerPrefs.SetInt("isWin", isWin); // Guarda el resultado para la siguiente escena.
        SceneManager.LoadScene("HR_GameOver"); // Carga la escena de fin de juego.
    }

    // Establece el resultado como derrota y carga la escena de Game Over.
    public void LoseGame()
    {
        isWin = 0; // Marca como derrota.
        PlayerPrefs.SetInt("isWin", isWin); // Guarda el resultado.
        SceneManager.LoadScene("HR_GameOver"); // Carga la escena de fin de juego.
    }

    // Carga la escena de inicio/buffer.
    public void GoHome()
    {
        SceneManager.LoadScene("HR_StartBuffer"); // Carga la escena especificada.
    }
} 