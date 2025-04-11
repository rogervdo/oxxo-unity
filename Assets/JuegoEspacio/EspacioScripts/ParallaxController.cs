using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private int direccion = 1; // 1 = hacia la derecha, -1 = hacia la izquierda

    private Vector3 startPos;
    private float length;

    void Start()
    {
        startPos = transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Mueve el fondo horizontalmente
        transform.position += Vector3.right * scrollSpeed * direccion * Time.deltaTime;

        float distancia = Mathf.Abs(transform.position.x - startPos.x);
        if (distancia >= length)
        {
            startPos = transform.position;
        }
    }

    public void CambiarDireccion(bool aLaDerecha)
    {
        direccion = aLaDerecha ? 1 : -1;
    }
}
