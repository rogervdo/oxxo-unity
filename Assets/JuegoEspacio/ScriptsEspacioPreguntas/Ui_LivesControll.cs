using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // ← Este es el que necesitas para usar IEnumerator


public class Ui_LivesControll : MonoBehaviour
{   
    // Texto para mostrar el tiempo restante
    public Sprite spendLives; // Sprite para mostrar la pérdida de vidas
    public Image[] livesImages; // Imágenes de las vidas restantes
    int lives = 3; // Número inicial de vidas
    int time;
  void Start()
{
    StartCoroutine(EsperarGameManager());
}

IEnumerator EsperarGameManager()
{
    while (GameSessionManager.Instance == null)
        yield return null;

    lives = GameSessionManager.Instance.ObtenerVidas();
    UpdateLives(); // <-- fuerza la actualización visual de los sprites
}



public void UpdateLives()
{
    lives = GameSessionManager.Instance.ObtenerVidas();

    // Primero, resetear todas las vidas a sprite normal (si usas otro sprite base)
    for (int i = 0; i < livesImages.Length; i++)
    {
        // Optional: poner sprite normal aquí si tienes uno como "vida llena"
        // livesImages[i].sprite = fullLifeSprite;
    }

    // Luego marcar las vidas perdidas con el sprite "gastado"
    for (int i = lives; i < livesImages.Length; i++)
    {
        livesImages[i].sprite = spendLives;
    }
}




}
