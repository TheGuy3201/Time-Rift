using Mono.Cecil;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] Transform groundChecker;
    [SerializeField] float groundCheckDistance = 0.2f; 
    [SerializeField] LayerMask groundLayer;

    //Shooting Stuff
    [Header("Shooting")]

    [SerializeField] Transform barrel;
    [SerializeField] Transform muzzle;
    [SerializeField] float bulletSpeed = 15f;
    [SerializeField] float bulletLife = 3f;
    [SerializeField] Rigidbody2D bullet;

    [Header("Canvas UI")]
    [SerializeField] Transform uiCanvas;
    [SerializeField] GameObject healthBar;

    //float angleSpeed = 0.5f;

    private Rigidbody2D rb;

    private SpriteRenderer spriteRenderer;
    private bool isLookingRight = true;

    private Vector3 originalCanvasScale;
    private float healthAmount = 100f;
    

    public bool IsGrounded
    {
        //get => true;
       
        get => Physics2D.OverlapCircle(groundChecker.position, groundCheckDistance, groundLayer);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalCanvasScale = uiCanvas.localScale;
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
            transform.localScale = new Vector3(-1f, 2f, 1f);
            isLookingRight = false;
        }
        else if (direction.x > 0f)
        {
            transform.localScale = new Vector3(1f, 2f, 1f);
            isLookingRight = true;
        }

        // Prevent canvas flip
        uiCanvas.localScale = originalCanvasScale;


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireCannon();
        }
    }

    private void FireCannon()
    {
        Rigidbody2D rb = Instantiate(bullet, muzzle.position, barrel.rotation);

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

    public void TakeDamage(float damage)
    {
        AudioManager.Play("PlayerDamage");
        healthAmount -= damage;
        healthBar.GetComponent<Slider>().value = healthAmount;

        if (healthAmount <= 0)
        {
            Debug.Log("You have died oof");

            SceneManager.LoadScene("MainMenu");
        }
    }

    public void Heal(float healingAmount)
    {
        AudioManager.Play("PlayerHeal");
        healthAmount += healingAmount;
        healthBar.GetComponent<Slider>().value = healthAmount;

        if (healthAmount > 100)
        {
            healthAmount = 100;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            AudioManager.Play("PlayerLanding");
        }
    }
}