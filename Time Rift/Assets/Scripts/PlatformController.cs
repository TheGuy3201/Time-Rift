using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformController : MonoBehaviour
{
    [SerializeField] float amplitude = 1.7f;
    [SerializeField] float angleSpeed = 1f;
    [SerializeField] float waitTime = 0.02f;
    [SerializeField] float damageFrequency = 2f;
    [SerializeField] float damageAmount = 5f;

    [SerializeField] GameObject EDisplay;

    GameObject captureObject;
    float originX;
    float originY;
    float angle = 10f;

    GameObject[] platformsA;

    GameObject playerInTrigger;
    Coroutine damageCoroutine;
    bool playerInTestAppearanceTrigger = false;


    private void Start()
    {
        originX = transform.position.x;
        originY = transform.position.y;
        if (this.gameObject.CompareTag("MovingPlatform"))
        {
            StartCoroutine(MovePlatform());
        }
        if (this.gameObject.CompareTag("TestAppearance"))
        {
            platformsA = GameObject.FindGameObjectsWithTag("AppearPlatform");
            SetPlatformAppearance(false);
        }
    }

    private void Update()
    {
        if (playerInTestAppearanceTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().keyCount > 0)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().keyCount--;
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().keyCountDisplay.GetComponent<TextMeshProUGUI>().text = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().keyCount + "";
                SetPlatformAppearance(true);
                //trigger door open code, temp is delete gameobject
                //Destroy(other.gameObject);
            }
            else
            {
                Debug.Log("You need a key to open this door");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (this.gameObject.CompareTag("MovingPlatform"))
            {
                collision.transform.SetParent(transform);
            }
            if (this.gameObject.CompareTag("ForestFinish"))
            {
                SceneManager.LoadScene("Corrupted Caverns");
            }
            if (this.gameObject.CompareTag("Dissapear"))
            {
                Destroy(this.gameObject);
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && this.gameObject.CompareTag("DamageZone"))
        {
            playerInTrigger = other.gameObject;
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DealDamageOverTime(playerInTrigger));
            }
        }
        if (this.gameObject.CompareTag("TestAppearance") && other.CompareTag("Player"))
        {
            EDisplay.SetActive(true);
            playerInTestAppearanceTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && this.gameObject.CompareTag("DamageZone"))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
            playerInTrigger = null;
        }
        if (this.CompareTag("TestAppearance") && other.CompareTag("Player"))
        {
            EDisplay.SetActive(false);
            playerInTestAppearanceTrigger = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (this.gameObject.CompareTag("MovingPlatform"))
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.transform.SetParent(null);
            }
        }
    }

    public void SetPlatformAppearance(bool isAppear)
    {
        foreach (GameObject g in platformsA)
        {
            g.SetActive(isAppear);
            //Debug.Log(g.activeSelf);
        }
    }

    IEnumerator MovePlatform()
    {
        while (true)
        {
            angle += angleSpeed;
            float deltaX = Mathf.Sin(angle) * amplitude;
            transform.position = new Vector2(originX + deltaX, originY);

            if (captureObject != null)
            {
                captureObject.transform.position = transform.position;
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator DealDamageOverTime(GameObject player)
    {
        while (true)
        {
            player.GetComponent<PlayerController>()?.TakeDamage(damageAmount);
            yield return new WaitForSeconds(damageFrequency);
        }
    }


}
