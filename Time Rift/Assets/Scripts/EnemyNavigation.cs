using UnityEngine;
using Pathfinding;
using System;

public class EnemyNavigation : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float speed = 200f;
    [SerializeField] float nextPointDistance = 3f;

    [SerializeField] float jumpForce = 5f;
    [SerializeField] Transform groundChecker;
    [SerializeField] float groundCheckDistance = 0.2f; // Adjust as needed
    [SerializeField] LayerMask groundLayer; // Set this in the Inspector

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
    }
    
    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
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



    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Delete enemy");
        }
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
