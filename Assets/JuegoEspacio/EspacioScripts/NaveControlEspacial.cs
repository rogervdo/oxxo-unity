using UnityEngine;

public class NaveControlEspacial : MonoBehaviour
{
   public float movespeed;
    public float jumpForce;
    public Rigidbody2D rig;
    public SpriteRenderer sr;
    Animator animatorController;

    void Start()
    {
        animatorController = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 direction = rig.linearVelocity;

        if (direction.magnitude > 0.1f) // Solo si se mueve
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Limita el ángulo a un rango razonable (por ejemplo, de -90° a +90°)
            angle = Mathf.Clamp(angle, -25f, 25f);

            // Suaviza la rotación hacia el ángulo deseado
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        else
        {
            // Si no se mueve, regresa lentamente a 0°
            Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }
    }




    private void FixedUpdate()
    {
        float xInput = Input.GetAxis("Horizontal");
    float yInput = Input.GetAxis("Vertical");
    rig.linearVelocity = new Vector2(xInput * movespeed, yInput * movespeed);


         if(xInput != 0 && rig.linearVelocity.y == 0)
        {
            UpdateAnimation(PlayerAnimation.ForwardEspacio);//Animacion para caminar
        }
        else
        {
            UpdateAnimation(PlayerAnimation.IdleEspacio);//Animacion quieto
        }
    }

        public enum PlayerAnimation
    {
        IdleEspacio, ForwardEspacio,
    }

    //Acutaliza la animacion de jugador segun sus condiciones
    void UpdateAnimation(PlayerAnimation nameAnimation)
    {
        switch(nameAnimation)
        {
            case PlayerAnimation.IdleEspacio:
                animatorController.SetBool("isMovingEspacio",false);
                break;
            case PlayerAnimation.ForwardEspacio:
                animatorController.SetBool("isMovingEspacio",true);
                animatorController.SetBool("isIdleEspacio",false);
                break;
        }
    }   
    
}
