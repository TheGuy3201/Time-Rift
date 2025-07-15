// Attach this script to the Canvas GameObject
using UnityEngine;

public class FollowPlayerUI : MonoBehaviour
{
    [SerializeField] Transform player;

    void LateUpdate()
    {
        transform.position = player.position;
    }
}
