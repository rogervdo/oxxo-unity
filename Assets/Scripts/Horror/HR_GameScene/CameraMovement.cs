using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float movimiento = 0f;
    public float margenMovimiento = 4f;
    public float margenMouse = 2.5f;
    public float velocidadMovimiento = 1f;

    public float limiteMouse = 5f;

    public Vector2 mousePosition;

    void Update()
    {
        MoverCamara();
    }

    private void MoverCamara()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float max = limiteMouse - margenMouse; // 5 - 2.5
        float speed = Mathf.Abs(mousePosition.x) - margenMouse; // 3 - 2.5 = 0.5
        speed = speed / max;

        if (mousePosition.x > margenMouse)
        {
            movimiento = movimiento + velocidadMovimiento * speed * Time.deltaTime;
        }
        else if (mousePosition.x < -margenMouse)
        {
            movimiento = movimiento - velocidadMovimiento * speed * Time.deltaTime;
        }

        movimiento = Mathf.Clamp(movimiento, -margenMovimiento, margenMovimiento);
        transform.position = new Vector3(movimiento, transform.position.y, -10f);
    }
}
