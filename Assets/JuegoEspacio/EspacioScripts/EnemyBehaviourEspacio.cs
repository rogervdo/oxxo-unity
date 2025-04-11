using UnityEngine;

public class EnemyBehaviourEspacio : MonoBehaviour
{
    private float velocity;

    void Start()
    {
        float xPos = transform.position.x;

        // 👇 Establece velocidad según el lado donde nació
        if (xPos > 0)
            velocity = -10f; // si aparece a la derecha, va hacia la izquierda
        else
            velocity = 10f;  // si aparece a la izquierda, va hacia la derecha
    }

    void Update()
    {
        transform.position += Vector3.right * Time.deltaTime * velocity;

        if (transform.position.x <= -50f || transform.position.x >= 70f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EspacioGameControll.Instance.SpendLives();
            Destroy(this.gameObject);
        }
    }
}
