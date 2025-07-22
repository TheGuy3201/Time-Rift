using TMPro;
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

    [Header("Shooting")]
    public Transform barrel;
    [SerializeField] Transform muzzle;
    public float bulletSpeed = 15f;
    public float bulletLife = 3f;
    public float damage = 25f;
    public int ammoCapacity;
    public int currentAmmo;
    public string audioName;

    public string reloadAudio;
    [SerializeField] Rigidbody2D bullet;

    [Header("Canvas UI")]
    [SerializeField] Transform uiCanvas;
    [SerializeField] GameObject healthBar;
    public GameObject keyCountDisplay;

    [SerializeField] GameObject EDisplay;
    [SerializeField] TextMeshProUGUI ammoCountDisplay;

    // Private variables
    private Rigidbody2D rb;
    private Sprite ogSprite;
    private SpriteRenderer spriteRenderer;
    private bool isLookingRight = true;
    private Vector3 originalCanvasScale;
    private float healthAmount = 100f;
    public int keyCount;
    public float reloadTime;
    public bool IsFullAuto;
    public float fireRate = 0.1f; // Time between shots (0.1f = 10 bullets per second)
    private bool isReloading = false;
    private float lastFireTime = 0f;

    public Sprite bulletSprite;

    public bool IsGrounded
    {
        get => Physics2D.OverlapCircle(groundChecker.position, groundCheckDistance, groundLayer);
    }

    private void UpdateAmmoDisplay()
    {
        ammoCountDisplay.text = $"{currentAmmo} / {ammoCapacity}";
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalCanvasScale = uiCanvas.localScale;
        
        // Get the original sprite from the serialized EDisplay field
        if (EDisplay != null)
        {
            SpriteRenderer eDisplaySprite = EDisplay.GetComponent<SpriteRenderer>();
            if (eDisplaySprite != null)
            {
                ogSprite = eDisplaySprite.sprite;
            }
        }
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

        // Handle ammo display
        UpdateAmmoDisplay();

        // Handle shooting
        if (Input.GetKeyDown(KeyCode.Mouse0) && !IsFullAuto)
        {
            FireCannon();
        }
        else if(Input.GetKey(KeyCode.Mouse0) && IsFullAuto && Time.time >= lastFireTime + fireRate)
        {
            FireCannon();
            lastFireTime = Time.time;
        }

        // Reload only when R is pressed, ammo is not full, and not already reloading
            if (Input.GetKeyDown(KeyCode.R) && currentAmmo < ammoCapacity && !isReloading)
            {
                StartCoroutine(ReloadCoroutine());
            }
    }

    private System.Collections.IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        // Optionally play reload SFX here
        AudioManager.Play(reloadAudio);
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = ammoCapacity;
        isReloading = false;
    }

    private void FireCannon()
    {
        if (currentAmmo > 0)
        {
            Rigidbody2D bulletRb = Instantiate(bullet, muzzle.position, barrel.rotation);
            bulletRb.gameObject.GetComponent<BulletController>().DamageAmount = damage;
            bulletRb.gameObject.GetComponent<BulletController>().audioName = audioName;
            bulletRb.gameObject.GetComponent<SpriteRenderer>().sprite = bulletSprite;
            currentAmmo--;

            if (isLookingRight)
            {
                bulletRb.linearVelocity = bulletRb.transform.right * bulletSpeed;
            }
            else
            {
                bulletRb.linearVelocity = -bulletRb.transform.right * bulletSpeed;
            }

            Destroy(bulletRb.gameObject, bulletLife);
        }
        else
        {
            AudioManager.Play("OutOfAmmo");
        }
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
        
        // Clamp health to maximum
        if (healthAmount > 100)
        {
            healthAmount = 100;
        }
        
        healthBar.GetComponent<Slider>().value = healthAmount;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            AudioManager.Play("PlayerLanding");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Key"))
        {
            keyCount++;
            if (keyCountDisplay != null)
            {
                TextMeshProUGUI keyText = keyCountDisplay.GetComponent<TextMeshProUGUI>();
                if (keyText != null)
                {
                    keyText.text = keyCount.ToString();
                }
            }
            //AudioManager.Play("KeyPickup");
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("HealthPickup"))
        {
            Heal(20);
            Destroy(other.gameObject);
        }
    }
}