using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody2D rig;
    public SpriteRenderer sr;
    private Animator animatorController;

    void Start()
    {
        animatorController = GetComponent<Animator>();
    }

    void Update()
     {
        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        rig.linearVelocity = movement * moveSpeed;

        if (movement != Vector2.zero)
        {
            // Detectar dirección
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                UpdateAnimation(PlayerAnimation.Lateral);

                // Flip horizontal
                if (movement.x < 0) sr.flipX = true;
                else if (movement.x > 0) sr.flipX = false;
            }
            else if (movement.y > 0)
            {
                UpdateAnimation(PlayerAnimation.Delante); // Movimiento hacia arriba
            }
            else if (movement.y < 0)
            {
                UpdateAnimation(PlayerAnimation.Atras); // Movimiento hacia abajo
            }
        }
        else
        {
            UpdateAnimation(PlayerAnimation.Idle); // Sin movimiento
        }
    }

    public enum PlayerAnimation
    {
        Idle, Delante, Atras, Lateral
    }

    void UpdateAnimation(PlayerAnimation animation)
    {
        switch (animation)
        {
            case PlayerAnimation.Idle:
                animatorController.SetBool("Idle", true);
                animatorController.SetBool("Forward", false);
                animatorController.SetBool("Back", false);
                animatorController.SetBool("Lateral", false);
                break;
            case PlayerAnimation.Delante:
                animatorController.SetBool("Forward", true);
                animatorController.SetBool("Idle", false);
                animatorController.SetBool("Back", false);
                animatorController.SetBool("Lateral", false);
                break;
            case PlayerAnimation.Atras:
                animatorController.SetBool("Back", true);
                animatorController.SetBool("Idle", false);
                animatorController.SetBool("Forward", false);
                animatorController.SetBool("Lateral", false);
                break;
            case PlayerAnimation.Lateral:
                animatorController.SetBool("Lateral", true);
                animatorController.SetBool("Idle", false);
                animatorController.SetBool("Back", false);
                animatorController.SetBool("Forward", false);
                break;
        }
    }
}
