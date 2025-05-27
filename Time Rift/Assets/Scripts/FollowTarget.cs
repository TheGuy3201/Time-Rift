using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] float speed = 2.5f;
    Vector2 targetPos; //is updated in the Update method
                       // Update is called approximately once per frame
    void Update()
    {
        targetPos = target.position;
    }
    // FixedUpdate is called exactly once per frame
    private void FixedUpdate()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        transform.position = Vector2.MoveTowards(
        transform.position, //your current position
        targetPos, //the intended target
        speed * Time.fixedDeltaTime * distance //the amount to move in a cycle
        );
    }
}