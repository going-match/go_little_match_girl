using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePrefabs2D : MonoBehaviour
{
    // Start is called before the first frame update

    public float moveSpeed = 2;
    private Vector3 moveDirection;
    private float firstFastTime = 3;
    private float secondFastTime = 7;

    public void Setup(Vector3 direction){
      moveDirection = direction;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsPlaying())
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            if ((Time.time >= firstFastTime) && (Time.time <= secondFastTime))
            {
                moveSpeed = 4;
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }
            if ((Time.time > secondFastTime))
            {
                moveSpeed = 6;
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }
        }
    }
}
