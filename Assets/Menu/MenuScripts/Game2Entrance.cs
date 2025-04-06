using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game2Entrance : MonoBehaviour
{
    public GameObject interactionTextUI;
    private bool playerIsNear = false;
    void Start()
    {
        if (interactionTextUI != null)
            interactionTextUI.SetActive(false);
    }

    
    void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("HR_StartBuffer");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            if (interactionTextUI != null)
                interactionTextUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            if (interactionTextUI != null)
                interactionTextUI.SetActive(false);
        }
    }
}
