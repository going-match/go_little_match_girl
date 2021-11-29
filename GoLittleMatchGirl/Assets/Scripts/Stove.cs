using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stove : MonoBehaviour
{
    private GameObject light, particle;

    private void Awake()
    {
        light = transform.GetChild(0).gameObject;
        particle = transform.GetChild(1).gameObject;
    }

    private void OnDisable()
    {
        light.SetActive(false);
        particle.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        light.SetActive(true);
        particle.SetActive(true);
    }
}
