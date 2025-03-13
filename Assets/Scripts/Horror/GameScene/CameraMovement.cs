using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float movimiento = 0f;
    public float margenMovimiento = 4f;
    public float margenMouse = 2.5f;
    public float velocidadMovimiento = 1f;

    public float limiteMouse = 5f;

    public Vector2 mousePosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
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

    public void ResetMovimiento()
    {
        movimiento = 0f;
        // Debug.Log("Movimiento = 0;");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(margenMouse, -4f), new Vector2(margenMouse, 4f));
        Gizmos.DrawLine(new Vector2(-margenMouse, -4f), new Vector2(-margenMouse, 4f));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(limiteMouse, -4f), new Vector2(limiteMouse, 4f));
        Gizmos.DrawLine(new Vector2(-limiteMouse, -4f), new Vector2(-limiteMouse, 4f));
    }
}
