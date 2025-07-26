using Pathfinding;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyNavigation : MonoBehaviour
{
    [Header("Main Navigation System")]
    [SerializeField] Transform target;
    [SerializeField] float nextPointDistance = 3f;

    [Header("Jumping System")]
    [SerializeField] float jumpForce = 5f;
    [SerializeField] Transform groundChecker;
    [SerializeField] float groundCheckDistance = 0.2f;
    [SerializeField] LayerMask groundLayer;

    [Header("Shooting System")]
    [SerializeField] EnemyType enemyType;
    [SerializeField] EnemyTier enemyTier;

    [SerializeField] Rigidbody2D bullet;
    [SerializeField] Transform barrel;
    [SerializeField] Transform muzzle;

    [Header("Health System")]
    [SerializeField] GameObject healthBar;

    private bool isLookingRight = true;

    Seeker seeker;
    Rigidbody2D rb;
    Path path;
    int currentWaypoint = 0;
    bool reachedEnd = false;
    bool hasValidPath = false;

    private bool IsPlayerInsideAStarGrid()
    {
        // Get the AstarPath instance (the main pathfinding component)
        if (AstarPath.active == null || AstarPath.active.data.graphs == null || AstarPath.active.data.graphs.Length == 0)
            return false; // If no graphs, player is not inside

        // Get the closest node to the player position
        NNInfo info = AstarPath.active.GetNearest(target.position, NNConstraint.Default);
        
        if (info.node != null && info.node.Walkable)
        {
            // Calculate distance between player and closest walkable node
            float distanceToNode = Vector3.Distance(target.position, (Vector3)info.node.position);
            
            // If player is close to a walkable node, they're inside the grid
            return distanceToNode < 2f; // Adjust this threshold as needed
        }
        
        return false; // Player is outside the grid
    }

    private float minChaseDistance = 7f;
    private float maxChaseDistance = 30f;
    private string shootAudioName;
    private float attackSpeed;
    private float bulletLife = 3f;
    private float bulletSpeed = 15f;
    private float speed = 200f;
    private float health = 100f;
    public float dmg;

    public bool isGrounded
    {
        get => Physics2D.OverlapCircle(groundChecker.position, groundCheckDistance, groundLayer);
    }

    private void Awake()
    {
        SetEnemyType();
    }

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    private void SetEnemyType()
    {
        Animator childAnimator = gameObject.GetComponentInChildren<Animator>();
        SpriteRenderer spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        gameObject.name = enemyType.name;

        switch (enemyType.type)
        {
            case EnemyType.TypeOfEnemy.Archer:
                bulletSpeed = 20f;
                bulletLife = 20f;
                speed = 300f;
                attackSpeed = 6f;
                health = 75f;
                minChaseDistance = 15f;
                maxChaseDistance = 40f;
                dmg = 18f;
                break;
            case EnemyType.TypeOfEnemy.Rifter:
                bulletSpeed = 30f;
                bulletLife = 15f;
                speed = 420f;
                attackSpeed = 1f;
                health = 145f;
                minChaseDistance = 12f;
                maxChaseDistance = 30f;
                dmg = 32;
                break;
            case EnemyType.TypeOfEnemy.StormSoldier:
                bulletSpeed = 30f;
                bulletLife = 15f;
                speed = 420f;
                attackSpeed = 2f;
                health = 200f;
                minChaseDistance = 14f;
                maxChaseDistance = 20f;
                dmg = 27;
                break;
        }
        spriteRenderer.sprite = enemyType.characterSprite;
        childAnimator.runtimeAnimatorController = enemyType.AnimController;
        barrel.GetComponent<SpriteRenderer>().sprite = enemyType.weaponSprite;
        shootAudioName = enemyType.shootAudioName;

        Invoke("SetEnemyTier", 2f);
        InvokeRepeating("AIShoot", 1f, attackSpeed);
    }

    private void SetEnemyTier()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        gameObject.name += " - " + enemyTier.name;

        switch (enemyTier.tier)
        {
            case EnemyTier.TierOfEnemy.Light:
                break;
            case EnemyTier.TierOfEnemy.Medium:
                bulletSpeed += 3f;
                bulletLife += 2f;
                speed -= 20f;
                health += 10f;
                minChaseDistance -= 3f;
                maxChaseDistance += 5f;
                dmg += 2f;
                spriteRenderer.color = new Color(1f, 1f, 0.5f); // Light yellow
                break;
            case EnemyTier.TierOfEnemy.Heavy:
                bulletSpeed -= 3f;
                speed -= 50f;
                attackSpeed += 0.5f;
                health += 35f;
                minChaseDistance -= 4f;
                maxChaseDistance -= 5f;
                dmg += 6f;
                spriteRenderer.color = new Color(1f, 0.5f, 0.5f); // Light red
                break;
            case EnemyTier.TierOfEnemy.SUPERHEAVY:
                bulletSpeed -= 5f;
                speed -= 50f;
                attackSpeed += 1f;
                health += 100f;
                minChaseDistance += 6f;
                maxChaseDistance += 15f;
                dmg += 9f;
                spriteRenderer.color = new Color(0.5f, 0f, 0f); // Dark red
                //gameObject.transform.localScale = new Vector3(4f, 6f, 1f);
                break;
        }

        
        healthBar.GetComponent<Slider>().maxValue = health;
        healthBar.GetComponent<Slider>().value = health;
    }

    private void LookDirection()
    {
        Vector3 direction = target.position - barrel.position;

        Quaternion rotation = isLookingRight
            ? Quaternion.FromToRotation(barrel.right, direction)
            : Quaternion.FromToRotation(-barrel.right, direction);

        barrel.rotation = rotation * barrel.rotation;

        if (direction.x < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            isLookingRight = false;
        }
        else if (direction.x > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            isLookingRight = true;
        }
    }

    private void AIShoot()
    {
        // Only shoot if player is inside the A* grid
        if (Vector2.Distance(rb.position, target.position) >= maxChaseDistance || !hasValidPath || !IsPlayerInsideAStarGrid())
            return;
        Rigidbody2D rbBullet = Instantiate(bullet, muzzle.position, barrel.rotation);
        rbBullet.gameObject.GetComponent<BulletController>().DamageAmount = dmg;
        rbBullet.gameObject.GetComponent<BulletController>().audioName = shootAudioName;
        rbBullet.gameObject.GetComponent<SpriteRenderer>().sprite = enemyType.bulletSprite;

        if (isLookingRight)
        {
            rbBullet.linearVelocity = barrel.right * bulletSpeed;
        }
        else
        {
            rbBullet.linearVelocity = -barrel.right * bulletSpeed;
        }

        Destroy(rbBullet.gameObject, bulletLife);
    }

    public void TakeDamage(float damage)
    {
        AudioManager.Play("EnemyDamage");
        health -= damage;
        healthBar.GetComponent<Slider>().value = health;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void UpdatePath()
    {
        float distanceToPlayer = Vector2.Distance(rb.position, target.position);

        // Only update path if player is inside the A* grid
        if (distanceToPlayer <= maxChaseDistance && distanceToPlayer > minChaseDistance && seeker.IsDone() && IsPlayerInsideAStarGrid())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
        else if (distanceToPlayer > maxChaseDistance || !IsPlayerInsideAStarGrid())
        {
            hasValidPath = false;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        // Don't move if player is outside the A* grid
        if (path == null || !hasValidPath || !IsPlayerInsideAStarGrid())
            return;

        float distanceToPlayer = Vector2.Distance(rb.position, target.position);

        
        LookDirection();

        if (distanceToPlayer <= minChaseDistance)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); 
            LookDirection(); 
            return;
        }


        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEnd = true;
            return;
        }

        reachedEnd = false;

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextPointDistance)
        {
            currentWaypoint++;
            if (currentWaypoint > 4.5f && isGrounded)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            hasValidPath = true;
        }
        else
        {
            hasValidPath = false;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
