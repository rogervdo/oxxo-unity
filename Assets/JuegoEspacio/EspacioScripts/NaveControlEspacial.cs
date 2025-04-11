using UnityEngine;

public class NaveControlEspacial : MonoBehaviour
{
    public float movespeed;
    public float jumpForce;
    public Rigidbody2D rig;
    public SpriteRenderer sr;
    Animator animatorController;

    public bool mirandoDerecha = true; // hacia dónde está viendo visualmente la nave
    public bool mirandoDerechaSpawner = true; // hacia qué lado está el spawner

    void Start()
    {
        animatorController = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 direction = rig.linearVelocity;

        if (direction.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 🔁 Invertimos la rotación si el spawner está a la izquierda
            if (!mirandoDerechaSpawner)
            {
                angle *= -1f;
            }

            angle = Mathf.Clamp(angle, -25f, 25f);
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        else
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }
    }

    private void FixedUpdate()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        // Solo moverse libremente en ambas direcciones
        rig.linearVelocity = new Vector2(xInput * movespeed, yInput * movespeed);

        // 🔒 Limitar hacia dónde puede mirar según el lado del spawner
        if (mirandoDerechaSpawner)
        {
            mirandoDerecha = true;
            sr.flipX = false;
        }
        else
        {
            mirandoDerecha = false;
            sr.flipX = true;
        }

        if (xInput != 0 && rig.linearVelocity.y == 0)
        {
            UpdateAnimation(PlayerAnimation.ForwardEspacio);
        }
        else
        {
            UpdateAnimation(PlayerAnimation.IdleEspacio);
        }
    }

    public enum PlayerAnimation
    {
        IdleEspacio, ForwardEspacio,
    }

    void UpdateAnimation(PlayerAnimation nameAnimation)
    {
        switch (nameAnimation)
        {
            case PlayerAnimation.IdleEspacio:
                animatorController.SetBool("isMovingEspacio", false);
                break;
            case PlayerAnimation.ForwardEspacio:
                animatorController.SetBool("isMovingEspacio", true);
                animatorController.SetBool("isIdleEspacio", false);
                break;
        }
    }

    // 👇 Llama este método desde FlipSpawnerPosition()
    public void ActualizarDireccionVisual(bool nuevaDireccionDerecha)
    {
        mirandoDerechaSpawner = nuevaDireccionDerecha;
    }
}
