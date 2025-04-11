using UnityEngine;
using UnityEngine.SceneManagement;

public class EspacioGameControll : MonoBehaviour
{
  
    public int timeToWin = 15; 
    static public EspacioGameControll Instance; 
    public UIControllerEspacio uiConroller;
     public SFXManagerEspacio SFXManager;

    // Se llama al iniciar el juego
    public void Awake()
    {
        StopAllCoroutines();
        PlayerPrefs.SetInt("lives", 3); 
        PlayerPrefs.SetInt("timeToWin", PlayerPrefs.GetInt("timeToWin", timeToWin));
        Instance = this;
        Instance.SetReferences(); 
        DontDestroyOnLoad(this.gameObject); 
    }

    // Configura las referencias de los objetos
  void SetReferences()
    {
        if(uiConroller == null)
        {
            uiConroller = FindAnyObjectByType<UIControllerEspacio>();
        }
        timeToWin = PlayerPrefs.GetInt("timeToWin", 15);
        init(); // Inicializa el temporizador
    }

    // Inicia el temporizador en el UI
   void init()
    {
        if(uiConroller != null)
        {
            uiConroller.StartTimer();
        }
    }
    // Obtiene las vidas actuales
    public int GetCurrentLives()
    {
        return PlayerPrefs.GetInt("lives", 3);
    }

    // Resta una vida
    public void SpendLives()
    {
        int newLives = GetCurrentLives() - 1;
        PlayerPrefs.SetInt("lives", newLives);
        uiConroller.UpdateLives(); 
        checkGameOver();
    }

    // Verifica si el juego ha terminado
    public void checkGameOver()
    {
        if (PlayerPrefs.GetInt("lives") == 0)
        {
            ActiveEndScene(); 
        }
    }

    // Activa la escena de fin de juego
    public void ActiveEndScene()
    {
        SceneManager.LoadScene("Escena_Perder_S");
    }

    // Configura referencias de objetos adicionales

void SetReference()
    {
        if(uiConroller == null)
        {
            uiConroller = FindFirstObjectByType<UIControllerEspacio>();
        }
        if(SFXManager == null)
        {
            SFXManager = FindFirstObjectByType<SFXManagerEspacio>();
        }
        timeToWin = PlayerPrefs.GetInt("timeToWin", 15);
        init();
    }

    // Carga la escena del menú
    public void GotoMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

public void FlipSpawnerPosition()
{
    // 🔁 Obtener todos los spawners en la escena
    var spawners = FindObjectsByType<EnemySpawnerEspacial>(FindObjectsSortMode.None);

    foreach (var spawner in spawners)
    {
        spawner.CambiarLadoSpawner();
    }

    // ✅ Detectar si el primer spawner está a la derecha
    bool spawnerADerecha = spawners.Length > 0 && spawners[0].transform.position.x > 0;

    // 🔁 Actualizar la dirección visual de la nave
    var nave = FindAnyObjectByType<NaveControlEspacial>();
    if (nave != null)
    {
        nave.ActualizarDireccionVisual(spawnerADerecha);
    }

    // 🔁 Cambiar la dirección del parallax si lo hay
    var fondo = FindAnyObjectByType<ParallaxController>();
    if (fondo != null)
    {
        fondo.CambiarDireccion(spawnerADerecha);
    }
}


// Detecta si ahora está a la derecha
private bool spawnerADerecha => Object.FindObjectsOfType<EnemySpawnerEspacial>()[0].transform.position.x < 0 ? false : true;

    

}
