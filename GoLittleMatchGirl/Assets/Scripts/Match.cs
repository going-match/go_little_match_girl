using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : MonoBehaviour
{
    private GameObject lastWindow;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Window" && collision.gameObject!=lastWindow)
        {
            lastWindow = collision.gameObject;
            Debug.Log("강매 성공!");
        }
    }
}
