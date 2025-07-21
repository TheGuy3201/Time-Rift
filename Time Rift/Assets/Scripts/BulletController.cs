using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float DamageAmount;
    public string audioName = "BlastPistolSFX"; 

    private void Start()
    {
        AudioManager.Play(audioName);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Destroy things");
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyNavigation>().TakeDamage(DamageAmount);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(DamageAmount);
        }
        Destroy(this.gameObject);
    }
}
