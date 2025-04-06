using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.GetComponent<Menu_PlayerMovement>()){
            SceneManager.LoadScene("Menu");
        }
    }
}
