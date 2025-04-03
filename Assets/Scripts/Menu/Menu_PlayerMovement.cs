using UnityEngine;

public class Menu_PlayerMovement : MonoBehaviour
{
    public float speed = 3;
    public Rigidbody2D rig;
    private Vector2 moveInput;
    private Animator playerAnimator;
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        playerAnimator.SetFloat("Horizontal", moveX);
        playerAnimator.SetFloat("Vertical", moveY);
        playerAnimator.SetFloat("Speed", moveInput.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        rig.MovePosition(rig.position + moveInput * speed * Time.fixedDeltaTime);
    }
}
