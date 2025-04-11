using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]public float moveSpeed;
    public Rigidbody2D playerRb;
    public SpriteRenderer sr;
    private Vector2 moveInput;
    private Animator playerAnimator;
    [SerializeField] private GameObject gameSessionPrefab; // Asigna el prefab desde Unity


    void Start()
    {
        if (GameSessionManager.Instance == null)
        {
            Instantiate(gameSessionPrefab);
        }

        playerRb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }


    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        playerAnimator.SetFloat("Horizontal",moveX);
        playerAnimator.SetFloat("Vertical", moveY);
        playerAnimator.SetFloat("Speed", moveInput.sqrMagnitude);

    }

    private void FixedUpdate()
    {
        playerRb.MovePosition(playerRb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BloqueCambio"))  // Asegúrate de que el bloque tenga este tag
        {
            SceneManager.LoadScene("EngineRoom_Q");
        }
    }
}
