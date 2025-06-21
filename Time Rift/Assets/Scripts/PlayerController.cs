using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] Transform groundChecker;
    [SerializeField] float groundCheckDistance = 0.2f; // Adjust as needed
    [SerializeField] LayerMask groundLayer; // Set this in the Inspector

    [SerializeField] AudioSource landingSFX;

    private Rigidbody2D rb;

    private SpriteRenderer spriteRenderer;
    public bool IsGrounded
    {
        //get => true;
       
        get => Physics2D.OverlapCircle(groundChecker.position, groundCheckDistance, groundLayer);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Get Horizontal Input
        float movementDirection = Input.GetAxis("Horizontal");

        // Apply Movement
        rb.linearVelocity = new Vector2(movementDirection * movementSpeed, rb.linearVelocity.y);
        spriteRenderer.flipX = rb.linearVelocity.x < 0;

        // Jump
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            landingSFX.Play();
        }
    }
}