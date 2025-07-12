using Pathfinding;
using System;
using System.Threading;
using UnityEngine;

public class EnemyNavigation : MonoBehaviour
{
    [Header("Main Navigation System")]
    [SerializeField] Transform target;
    [SerializeField] float speed = 200f;
    [SerializeField] float nextPointDistance = 3f;
    [SerializeField] float maxChaseDistance = 30f;
    [SerializeField] float minChaseDistance = 7f;


    [Header("Jumping System")]
    [SerializeField] float jumpForce = 5f;
    [SerializeField] Transform groundChecker;
    [SerializeField] float groundCheckDistance = 0.2f; // Adjust as needed
    [SerializeField] LayerMask groundLayer; // Set this in the Inspector

    [Header("Shooting System")]
    [SerializeField] float attackSpeed;
    [SerializeField] string enemyType;

    [SerializeField] Rigidbody2D bullet;
    [SerializeField] Transform barrel;
    [SerializeField] Transform muzzle;
    [SerializeField] float bulletSpeed = 15f;
    [SerializeField] float bulletLife = 3f;

    private bool isLookingRight = true;

    Path path;
    int currentWaypoint = 0;
    bool reachedEnd = false;

    public bool isGrounded
    {
        //get => true;

        get => Physics2D.OverlapCircle(groundChecker.position, groundCheckDistance, groundLayer);
    }

    Seeker seeker;
    Rigidbody2D rb;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
        InvokeRepeating("AIShoot", 0f, attackSpeed);
    }

    private void LookDirection()
    {
        Vector3 direction = target.position - barrel.position;
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
    }
    
    private void AIShoot()
    {
        Rigidbody2D rb = Instantiate(bullet, muzzle.position, barrel.rotation);

        //AudioManager.Play("shot");
        if (isLookingRight)
        {
            rb.linearVelocity = barrel.right * bulletSpeed;
        }
        else if (!isLookingRight)
        {
            rb.linearVelocity = -barrel.right * bulletSpeed;
        }

        Destroy(rb.gameObject, bulletLife);
    }

    void UpdatePath()
    {
        float distanceToPlayer = Vector2.Distance(rb.position, target.position);

        if (distanceToPlayer <= maxChaseDistance && distanceToPlayer > minChaseDistance && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            path = null;
        }

    }

    private void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEnd = true;
            return;
        }
        else
        {
            reachedEnd = false;
        }

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

        //------SHOOTING FUNCTIONALITY HERE-------//
        LookDirection();

    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}
