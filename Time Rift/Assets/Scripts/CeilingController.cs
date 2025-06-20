using System.Collections;
using UnityEngine;

public class CeilingController : MonoBehaviour
{
    [SerializeField] float originGravity;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject gb = collision.gameObject;
        if (gb.CompareTag("Player"))
        {
            originGravity = gb.GetComponent<Rigidbody2D>().gravityScale;
            gb.GetComponent<Rigidbody2D>().gravityScale = 0;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject gb = collision.gameObject;
        if (gb.CompareTag("Player"))
        {
            gb.GetComponent<Rigidbody2D>().gravityScale = originGravity;
        }
    }
}