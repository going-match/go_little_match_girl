using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Match : MonoBehaviour
{
    private GameObject lastWindow;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("Window") && collision.gameObject!=lastWindow)
        {
            lastWindow = collision.gameObject;
            GameManager.Instance.AddScore(1);
        }
    }
}
