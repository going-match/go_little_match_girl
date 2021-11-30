using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour
{
  private MeshRenderer render;
  private float offset;
    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsPlaying() && GameManager.Instance.IsPlayerCenter())
        {
            offset += Time.deltaTime * GameManager.Instance.GetStageSpeed() * 0.5f;
            render.material.mainTextureOffset = new Vector2(offset, 0);
        }
    }
}
