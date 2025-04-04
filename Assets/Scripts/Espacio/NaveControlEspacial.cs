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
        if(rig.linearVelocity.x >0)
        {
            sr.flipX = false; //Mover el sprite del personaje a la derecha
        }
        else if (rig.linearVelocity.x <0)
        {
            sr.flipX = true;//Mover el sprite del personaje a la izquierda
        }
    }

    private void FixedUpdate()
    {
        float xInput = Input.GetAxis("Horizontal");
        rig.linearVelocity = new Vector2(xInput*movespeed, rig.linearVelocity.y);
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
