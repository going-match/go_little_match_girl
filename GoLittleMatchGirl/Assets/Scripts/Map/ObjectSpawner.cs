using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    //지붕 배열
    [SerializeField]
    private GameObject[] prefabArray;

    [SerializeField]
    private GameObject BuildingBlockPrefab;

    //최초 건물 블록 시작점-건물 이미지 및 배경 사이즈 결정되면 수정해야 함.
    Vector3 buildingStartPosition = new Vector3(-9.7f, -4, 0);

    //창문 prefab 배열
    [SerializeField]
    private GameObject[] prefabWindowArray;

    //창문 처음 시작 위치
    Vector3 windowStartPosition = new Vector3(-10.5f, -4, 0);

    //타일 prefab
    [SerializeField]
    private GameObject TilePrefab;
    //타일 처음 시작 위치
    Vector3 tileStartPosition = new Vector3(-5f, -5, 0);


    void Update()
    {
        if (GameManager.Instance.IsPlaying())
        {
            //창문 이동 위치 지정
            Vector3 windowBuildingPosition = windowStartPosition;

            //빛이 들어오는 창문 수
            int windowLightOnNum = 0;

            //기본 회전값 설정 = 0
            Quaternion rotation = Quaternion.Euler(0, 0, 0);

            //기본 건물 블록의 세로와 가로 길이
            Vector3 verticalGap = new Vector3(0, 2.02f, 0);
            Vector3 horizontalGap = new Vector3(3.8f, 0, 0);

            //건물 높이 결정 랜덤 반복문
            int blockNum = Random.Range(3, 5);
            //Debug.Log("Random height: " + blockNum);

            //한 층당 창문 개수
            int windowNum = 0;

            for (int i = 0; i < blockNum - 1; i++)
            {

                //창문 개수 초기화
                windowNum = 0;

                //건물 블록 생성
                GameObject buildingBlock = Instantiate(BuildingBlockPrefab, buildingStartPosition, rotation);
                //건물 이동 벡터
                Vector3 moveDirection = Vector3.left;
                buildingBlock.GetComponent<MovePrefabs2D>().Setup(moveDirection);
                //건물이 2층 이상일 경우 건물 블록 생성 위치 이동.
                buildingStartPosition = buildingStartPosition + verticalGap;


                while (true)
                {
                    if (windowNum == 2) break;
                    //창문 랜덤 결정
                    int prefabWindowArrayidx = Random.Range(0, 6);
                    //불켜진 창문이 3개인데 랜덤으로 불 켜진 창문이 결정되면 번호 다시뽑기.
                    if ((windowLightOnNum == 3) && (prefabWindowArrayidx >= 3))
                    {
                        prefabWindowArrayidx = Random.Range(0, 3);
                    }
                    //창문 랜덤 번호가 3인 경우 불켜진 창문 개수 세기
                    if (prefabWindowArrayidx >= 3)
                    {
                        windowLightOnNum++;
                    }
                    //창문 생성
                    GameObject windowBlock = Instantiate(prefabWindowArray[prefabWindowArrayidx], windowBuildingPosition, rotation);
                    Vector3 windowMoveDirection = Vector3.left;
                    windowBlock.GetComponent<MovePrefabs2D>().Setup(windowMoveDirection);
                    //두 번째 창문 위치 지정
                    windowBuildingPosition = windowBuildingPosition + new Vector3(1.5f, 0, 0);
                    windowNum++;
                }

                //창문 생성 후 다음 위치 이동.
                windowBuildingPosition = windowBuildingPosition + verticalGap + new Vector3(-3f, 0, 0);


            }

            //마지막 지붕

            //몇 번 buildingBlock 가져올 지 결정.
            int prefabArrayidx = Random.Range(0, 3);
            GameObject lastBuildingBlock = Instantiate(prefabArray[prefabArrayidx], buildingStartPosition, rotation);
            Vector3 lastMoveDirection = Vector3.left;
            lastBuildingBlock.GetComponent<MovePrefabs2D>().Setup(lastMoveDirection);
            buildingStartPosition = buildingStartPosition + verticalGap;

            //건물 생성 시작점 y값 초기화
            buildingStartPosition.y = -4;
            //건물 생성 시작점 x값 이동-이전 좌표는 건물이 생겨서
            buildingStartPosition = buildingStartPosition + horizontalGap;
            //창문 생성 시작점 y값 초기화
            windowStartPosition.y = -4;
            windowStartPosition = windowStartPosition + horizontalGap;

            //타일의 랜덤 사이즈
            int tileScale = Random.Range(2, 4);
            //Debug.Log("타일사이즈: " + tileScale);
            //타일 생성
            GameObject tileBlock = Instantiate(TilePrefab, tileStartPosition, rotation);
            //타일 가로 길이
            tileBlock.transform.localScale = new Vector3(tileScale, 1, 0);
            //타일 y축 높이
            int tileHeight = Random.Range(0, 5);
            tileBlock.transform.position = tileStartPosition + new Vector3(0, tileHeight, 0);
            //타일 방향에 맞게 이동
            Vector3 tileDirection = Vector3.left;
            tileBlock.GetComponent<MovePrefabs2D>().Setup(lastMoveDirection);
            //타일 x축 위치 이동(x축 위치는 임의로 지정했으므로 추후 점프 높이에 맞게 수정 유망)
            Vector3 jumpGap = new Vector3(3.5f, 0, 0);
            tileStartPosition = tileStartPosition + jumpGap;

        }



    }


}
