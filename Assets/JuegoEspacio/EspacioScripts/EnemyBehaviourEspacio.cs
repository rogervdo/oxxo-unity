using UnityEngine;

public class EnemyBehaviourEspacio : MonoBehaviour
{
    public float velocity; // Velocidad del enemigo

    // Start is called once before the first execution of Update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        // Mueve al enemigo a la derecha
        this.transform.position += Vector3.right * Time.deltaTime * velocity;

        // Si el enemigo sale del límite, se destruye
        if(transform.position.x >= 50)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    // Detecta la colisión con el jugador
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // Acción al colisionar con el jugador
            //EspacioGameControll.Instance.SFXManager.getCoin();
            EspacioGameControll.Instance.SpendLives();
            GameObject.Destroy(this.gameObject);
        }
    }
}
