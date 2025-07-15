using UnityEngine;

public class BulletController : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Play("BlastPistolSFX");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Destroy things");
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyNavigation>().TakeDamage(25f);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(25f);
        }
        Destroy(this.gameObject);
    }
}
