using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] Transform barrel;
    [SerializeField] Transform muzzle;
    [SerializeField] float bulletSpeed = 15f;
    [SerializeField] float bulletLife = 3f;
    [SerializeField] Rigidbody2D bullet;

    float angleSpeed = 200f;

    private void Update()
    {
        float delta = Input.GetAxis("Vertical");
        barrel.rotation = Quaternion.Euler(barrel.eulerAngles + (angleSpeed * delta * Vector3.forward));

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireCannon();
        }
    }

    private void FireCannon()
    {
        Rigidbody2D rb = Instantiate(bullet, muzzle.position, barrel.rotation);

        rb.linearVelocity = rb.transform.right * bulletSpeed;
        
        Destroy(rb.gameObject, bulletLife);
    }
}
