using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] float finalY = 2f;
    [SerializeField] float speed = 0.2f;
    [SerializeField] float waitTime = 5.2f;

    float originX;
    float originY;
    private void Start()
    {
        finalY = door.transform.position.y + 2f;
        originX = door.transform.position.x;
        originY = door.transform.position.y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Invoke("StartOpen",0.1f);
            Invoke("StartClose", 5.1f);
        }
    }

    private void StartOpen()
    {
        StartCoroutine(OpenDoor());
        Debug.Log("Opening...");
    }
    private void StartClose()
    {
        StartCoroutine(CloseDoor());
        Debug.Log("Closing...");
    }

    private IEnumerator OpenDoor()
    {
        while (door.transform.position.y < finalY)
        {
            float newY = door.transform.position.y + speed;
            door.transform.position = new Vector2(originX, newY);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator CloseDoor()
    {
        while (door.transform.position.y > originY)
        {
            float newY = door.transform.position.y - speed;
            door.transform.position = new Vector2(originX, originY);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
