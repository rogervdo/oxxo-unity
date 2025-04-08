using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public Vector2 scrollDirection = Vector2.left; // Dirección del movimiento del fondo
    public float scrollSpeed = 0.5f; // Velocidad del parallax
    public float tileSize = 20f; // Tamaño del sprite para hacer el loop infinito

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Movimiento del fondo
        transform.position += (Vector3)(scrollDirection * scrollSpeed * Time.deltaTime);

        // Calcula cuánto se ha movido desde el inicio
        float distanceMoved = (transform.position - startPos).magnitude;

        // Si se movió más de una "tile", resetea
        if (distanceMoved >= tileSize)
        {
            startPos = transform.position;
        }
    }
}
