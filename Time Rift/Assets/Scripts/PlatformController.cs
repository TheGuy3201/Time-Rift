using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformController : MonoBehaviour
{
    [SerializeField] float amplitude = 1.7f;
    [SerializeField] float angleSpeed = 1f;
    [SerializeField] float waitTime = 0.02f;

    GameObject captureObject;
    float originX;
    float originY;
    float angle = 10f;

    GameObject[] platformsA;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (this.gameObject.CompareTag("MovingPlatform"))
            {
                collision.transform.SetParent(transform);
            }
            if (this.gameObject.CompareTag("TestAppearance"))
            {
                SetPlatformAppearance(true);
            }
            if (this.gameObject.CompareTag("ForestFinish"))
            {
                SceneManager.LoadScene("Corrupted Caverns");
            }
            //if (this.gameObject.CompareTag("PainPlatform"))
            //{
            //    Debug.Log("Still in development!!");
            //}
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
            Debug.Log(g.activeSelf);
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

    
}
