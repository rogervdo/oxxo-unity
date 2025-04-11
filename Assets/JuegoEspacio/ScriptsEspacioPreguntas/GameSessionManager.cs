using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance;

    public int vidasActuales = 3;
    public int vidasMaximas = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persistente entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReiniciarVidas()
    {
        vidasActuales = vidasMaximas;
    }

    public void QuitarVida()
    {
        vidasActuales--;
    }

    public int ObtenerVidas()
    {
        return vidasActuales;
    }
}
