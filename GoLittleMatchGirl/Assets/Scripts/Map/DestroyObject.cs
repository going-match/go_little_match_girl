using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector2 limitMin = new Vector3(-15, 0, 0);

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsPlaying())
        {
            if (transform.position.x < limitMin.x)
            {
                Destroy(gameObject);
            }
        }

    }
}
