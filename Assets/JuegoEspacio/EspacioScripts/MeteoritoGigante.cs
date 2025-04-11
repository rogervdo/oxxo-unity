using UnityEngine;

public class MeteoritoGigante : MonoBehaviour
{
    private float velocidad;

    void Start()
    {
        float xPos = transform.position.x;

        if (xPos > 0)
        {
            velocidad = -10f; 
        }
        else
        {
            velocidad = 10f;  
        }
    }

    void Update()
    {
        transform.position += Vector3.right * velocidad * Time.deltaTime;

        if (Mathf.Abs(transform.position.x) > 60f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EspacioGameControll.Instance.FlipSpawnerPosition();
            Destroy(gameObject);
        }
    }
}
