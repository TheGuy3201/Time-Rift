using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] Transform groundChecker;
    [SerializeField] float groundCheckDistance = 0.2f; // Adjust as needed
    [SerializeField] LayerMask groundLayer; // Set this in the Inspector

    [Header("Player Audio Sources")]
    [SerializeField] AudioSource landingSFX;

    //Shooting Stuff
    [Header("Shooting")]

    [SerializeField] Transform barrel;
    [SerializeField] Transform muzzle;
    [SerializeField] float bulletSpeed = 15f;
    [SerializeField] float bulletLife = 3f;
    [SerializeField] Rigidbody2D bullet;

    //float angleSpeed = 0.5f;

    private Rigidbody2D rb;

    private SpriteRenderer spriteRenderer;
    private bool isLookingRight = true;

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
        //------PLAYER-MOVEMENT-------//
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

        //------SHOOTING-------//

        //Get mousePosition
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        mousePosition.z = 0;

        //Calculate Direction vector
        Vector3 direction = mousePosition - barrel.position;

        //rotate barrel (using FromtoRotation)
        if (isLookingRight)
        {
            Quaternion rotation = Quaternion.FromToRotation(barrel.right, direction);
            barrel.rotation = rotation * barrel.rotation;
        }
        else if (!isLookingRight) 
        {
            Quaternion rotation = Quaternion.FromToRotation(-barrel.right, direction);
            barrel.rotation = rotation * barrel.rotation;
        }

        if (direction.x < 0f)
        {
            transform.localScale = new Vector3(-1f,2f,1f);
            isLookingRight = false;
        }

        else if (direction.x > 0f)
        {
            transform.localScale = new Vector3(1f, 2f, 1f);
            isLookingRight = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireCannon();
        }
    }

    private void FireCannon()
    {
        Rigidbody2D rb = Instantiate(bullet, muzzle.position, barrel.rotation);

        //AudioManager.Play("shot");
        if (isLookingRight)
        { 
            rb.linearVelocity = rb.transform.right * bulletSpeed; 
        }
        else if (!isLookingRight)
        {
            rb.linearVelocity = -rb.transform.right * bulletSpeed;
        }

        Destroy(rb.gameObject, bulletLife);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            landingSFX.Play();
        }
    }
}