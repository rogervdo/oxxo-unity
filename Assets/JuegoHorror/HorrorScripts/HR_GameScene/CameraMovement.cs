using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Desplazamiento horizontal actual de la cámara.
    public float movimiento = 0f;
    // Desplazamiento horizontal máximo permitido desde el centro.
    public float margenMovimiento = 4f;
    // Distancia desde el borde de la pantalla donde comienza el movimiento.
    public float margenMouse = 1.5f;
    // Multiplicador de velocidad base para el movimiento de la cámara.
    public float velocidadMovimiento = 5f;
    // Coordenada X del mundo que define el borde usado para los cálculos de margen.
    public float limiteMouse = 3f;
    // Posición actual del ratón en coordenadas del mundo.
    public Vector2 mousePosition;

    void Update()
    {
        // Actualiza la posición de la cámara basada en la posición del ratón cada frame.
        MoverCamara();
    }

    // Calcula y aplica el movimiento horizontal de la cámara basado en la posición del ratón.
    private void MoverCamara()
    {
        // Obtiene la posición del ratón en coordenadas del mundo.
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Calcula la zona efectiva para la velocidad proporcional.
        float max = limiteMouse - margenMouse;
        // Calcula cuánto se ha movido el ratón dentro de la zona activa.
        float speed = Mathf.Abs(mousePosition.x) - margenMouse;
        // Normaliza la velocidad (0 a 1) dentro de la zona activa.
        speed = Mathf.Clamp01(speed / max); // Asegura que speed esté entre 0 y 1.

        // Mueve a la derecha si el ratón está más allá del margen derecho.
        if (mousePosition.x > margenMouse)
        {
            movimiento = movimiento + velocidadMovimiento * speed * Time.deltaTime;
        }
        // Mueve a la izquierda si el ratón está más allá del margen izquierdo.
        else if (mousePosition.x < -margenMouse)
        {
            movimiento = movimiento - velocidadMovimiento * speed * Time.deltaTime;
        }

        // Limita el movimiento horizontal total dentro de los márgenes definidos.
        movimiento = Mathf.Clamp(movimiento, -margenMovimiento, margenMovimiento);
        // Aplica la nueva posición X a la cámara, manteniendo Y y Z.
        transform.position = new Vector3(movimiento, transform.position.y, -10f);
    }
} 