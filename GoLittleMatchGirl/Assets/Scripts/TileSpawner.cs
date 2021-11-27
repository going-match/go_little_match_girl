using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    //tileprefab 생성
    [SerializeField]
    private GameObject TilePrefab;

    //기본 시작 위치
    Vector3 startTilePosition = new Vector3(15, 15, 0);

    void Update()
    {
        //생성시 기본 회전 조건
        Quaternion rotation = Quaternion.Euler(0,0,0);


    }
}
