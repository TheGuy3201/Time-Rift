using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform toFollow;
    [SerializeField] float smoothTime = 3f; //smaller values will move more quickly
    Vector3 velocity;
    float y, z; //want to fix y and z for the camera
    private void Start()
    {
        y = transform.position.y;
        z = transform.position.z;
        velocity = new Vector3(0, y, z);
    }
    void LateUpdate()
    {
        Vector3 target = new Vector3(toFollow.position.x, y, z);
        transform.position = Vector3.SmoothDamp(
        transform.position,
        target,
        ref velocity,
        smoothTime);
    }
}