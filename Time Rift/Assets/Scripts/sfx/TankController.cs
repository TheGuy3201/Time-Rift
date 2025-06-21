using UnityEngine;

public class TankController : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 20f;
    [SerializeField] Rigidbody2D bulletRB;
    [SerializeField] Transform barrel;
    [SerializeField] Transform muzzle;

    Vector3 aim;

    private void Update()
    {
        //Get mousePosition
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        //Calculate Direction vector
        Vector3 direction = mousePosition - barrel.position;

        //rotate barrel (using FromtoRotation)
        Quaternion rotation = Quaternion.FromToRotation(barrel.right, direction);
        barrel.rotation = rotation * barrel.rotation;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //fire bullet
            Rigidbody2D rb = Instantiate(bulletRB, muzzle.position, barrel.rotation);

            //Plays shooting sfx
            AudioManager.Play("shot");

            //Move it
            rb.linearVelocity = rb.transform.right * bulletSpeed;

            //Destroy it
            Destroy(rb.gameObject, 1.0f);
        }
    }
}
