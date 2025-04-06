using UnityEngine;

// Controla la reproducción de efectos de sonido
public class SFXManagerEspacio : MonoBehaviour
{
    public AudioClip coins; // Sonido de monedas
    public AudioClip win; // Sonido de victoria
    public AudioClip lose; // Sonido de derrota

    // Reproduce el sonido de una moneda recogida
    public void getCoin()
    {
        AudioSource.PlayClipAtPoint(coins, Camera.main.transform.position, 0.5f);
    }

    // Reproduce el sonido de victoria
    public void Winning()
    {
        AudioSource.PlayClipAtPoint(win, Camera.main.transform.position, 0.5f);
    }

    // Reproduce el sonido de derrota
    public void Losing()
    {
        AudioSource.PlayClipAtPoint(lose, Camera.main.transform.position, 0.5f);
    }


    void Start() { }

 
    void Update() { }
}
