using System;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [SerializeField] Transform barrel;
    [SerializeField] Transform muzzle;
    [SerializeField] float angleSpeed = 0.5f;
    [SerializeField] float bulletSpeed = 15f;
    [SerializeField] Rigidbody2D bullet;

    private void Update()
    {
        float delta = Input.GetAxis("Vertical");
        barrel.rotation = Quaternion.Euler(barrel.eulerAngles + (angleSpeed * delta * Vector3.forward));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireCannon();
        }
    }

    private void FireCannon()
    {
        Rigidbody2D rb = Instantiate(bullet, muzzle.position, barrel.rotation);

        rb.linearVelocity = rb.transform.right * bulletSpeed;
        Destroy(rb.gameObject, 1f);
    }
}
