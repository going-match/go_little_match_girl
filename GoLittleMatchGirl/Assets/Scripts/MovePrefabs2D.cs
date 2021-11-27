using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePrefabs2D : MonoBehaviour
{
    // Start is called before the first frame update

    public float moveSpeed = 1;
    private Vector3 moveDirection;

    public void Setup(Vector3 direction){
      moveDirection = direction;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
