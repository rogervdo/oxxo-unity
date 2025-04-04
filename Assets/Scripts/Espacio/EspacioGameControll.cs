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
        //uiConroller.UpdateLives(); 
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
        SceneManager.LoadScene("EndScene");
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
}
