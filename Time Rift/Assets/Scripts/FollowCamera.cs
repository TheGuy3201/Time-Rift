using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform toFollow;
    [SerializeField] float smoothTime = 3f; // smaller values will move more quickly
    Vector3 velocity = Vector3.zero;
    float z; // z can be fixed if you're working in 2D or want side view

    private void Start()
    {
        z = transform.position.z;
    }

    void LateUpdate()
    {
        Vector3 target = new Vector3(toFollow.position.x, toFollow.position.y, z);
        transform.position = Vector3.SmoothDamp(
            transform.position,
            target,
            ref velocity,
            smoothTime);
    }
}
