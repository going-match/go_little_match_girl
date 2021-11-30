using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Sprite[] roofSprite = new Sprite[3];
    [SerializeField] private Sprite[] windowOffSprite = new Sprite[3];  // 불 꺼진 창문(3종류)
    [SerializeField] private Sprite[] windowOnSprite = new Sprite[3];  // 불 켜진 창문(3종류)
    [SerializeField] private Sprite[] windowObstacleSprite = new Sprite[6];     // 불 켜진 장애물(리스/칠면조) 창문 (각 3종류)

    private GameObject[] map;
    private GameObject[,] floor;
    private GameObject[,] building;
    private GameObject[,,] tile;
    //private BoxCollider2D[,,] tileCol;
    private SpriteRenderer[,] roofSR;
    private GameObject[,,] buildingBlock;
    private GameObject[,,] window;
    private SpriteRenderer[,,] windowSR;

    private float floorPosition = -6f;
    private float buildingBlockHeight = 2f;
    private float[] roofPosition = { 2.7f, 2.2f, 1.95f };

    private int mapNum = 3;
    private int buildingNum = 5;
    private int tileNum = 5;
    private int floorNum = 3;
    private int buildingBlockNum = 3;
    private int windowNum = 6;
    private int windowTypeNum = 3;

    private float windowObstaclePercentage = 0.1f;
    private float floorFullPercentage = 0.7f;

    private float speed;

    private void Awake()
    {
        map = new GameObject[transform.childCount];
        floor = new GameObject[mapNum, floorNum];
        building = new GameObject[mapNum, buildingNum];

        tile = new GameObject[mapNum, floorNum, tileNum];
        //tileCol = new BoxCollider2D[mapNum, floorNum, tileNum];

        buildingBlock = new GameObject[mapNum, buildingNum, buildingBlockNum];
        roofSR = new SpriteRenderer[mapNum, buildingNum];
        window = new GameObject[mapNum, buildingNum, windowNum];
        windowSR = new SpriteRenderer[mapNum, buildingNum, windowNum];

        for (int mIndex = 0; mIndex < mapNum; mIndex++)
        {
            map[mIndex] = transform.GetChild(mIndex).gameObject;

            for (int fIndex = 0; fIndex < floorNum; fIndex++)
            {
                floor[mIndex,fIndex] = map[mIndex].transform.GetChild(fIndex).gameObject;

                for (int tIndex = 0; tIndex < tileNum; tIndex++)
                {
                    tile[mIndex, fIndex, tIndex] = floor[mIndex, fIndex].transform.GetChild(tIndex).gameObject;
                    //tileCol[mIndex, fIndex, tIndex] = tile[mIndex,fIndex,tIndex].GetComponent<BoxCollider2D>();
                }
            }

            for (int bIndex = 0; bIndex < buildingNum; bIndex++)
            {
                building[mIndex, bIndex] = map[mIndex].transform.GetChild(bIndex+floorNum).gameObject;

                for (int wIndex = 0; wIndex < windowNum; wIndex++)
                {
                    window[mIndex, bIndex, wIndex] = building[mIndex, bIndex].transform.GetChild(0).GetChild(wIndex).gameObject;
                    windowSR[mIndex, bIndex, wIndex] = window[mIndex,bIndex,wIndex].GetComponent<SpriteRenderer>();
                }

                for (int bbIndex = 0; bbIndex < buildingBlockNum; bbIndex++)
                {
                    buildingBlock[mIndex, bIndex, bbIndex] = building[mIndex, bIndex].transform.GetChild(bbIndex + 1).gameObject;
                }
                roofSR[mIndex, bIndex] = building[mIndex, bIndex].transform.GetChild(buildingBlockNum+1).GetComponent<SpriteRenderer>();
            }
        }
    }

    private void Start()
    {
        speed = GameManager.Instance.GetStageSpeed();
        for (int i = 0; i < mapNum; i++)
        {
            SetMap(i);
        }

        // 시작 전 맵에 대하여 바닥층 타일 모두 활성화(시작할 때 플레이어가 중앙으로 이동하는 도중 떨어짐 방지)
        for (int i = 0; i < tileNum; i++)
        {
            tile[0, 0, i].SetActive(true);
            //tileCol[0, 0, i].isTrigger = false;
        }
        // 시작 전 맵에 대하여 창문 타일 불꺼진/불켜진일반 창문으로만 구성
        for(int i=0; i < buildingNum; i++)
        {
            for(int j=0; j<windowNum; j++)
            {
                int randType = Random.Range(0, 2);

                windowSR[0, i, j].sprite = windowOnSprite[randType];
                window[0, i, j].tag = ((randType == 1) ? "Window" : "Untagged");
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying() && GameManager.Instance.IsPlayerCenter())
        {
            speed = GameManager.Instance.GetStageSpeed();

            // 맵 이동 및 세팅
            for (int i=0; i<mapNum; i++)
            {
                Vector3 pos = map[i].transform.position;
                pos.x -= (Time.deltaTime * speed * 5f);

                // 해당 맵이 맨 오른쪽 끝으로 갈 때 맵 세팅
                if (pos.x < -20)
                {
                    pos.x = 40;
                    SetMap(i);
                }
                map[i].transform.position = pos;
            }
        }
    }

    // 하나의 맵 파트 세팅; 지정 맵 파트의 위치 및 모든 건물 세팅
    private void SetMap(int index)
    {
        //map[index].transform.position = new Vector3();

        // 건물 결정
        for(int i=0; i<buildingNum; i++) SetBuilding(index, i);

        // 지형 결정
        for(int i=0; i<floorNum; i++) SetFloor(index, i);

    }

    // 하나의 건물 세팅; 건물 층수, 지붕 종류, 창문 종류 결정
    private void SetBuilding(int mIndex, int bIndex)
    {
        // 건물 층수
        int f = Random.Range(2, 4);
        for(int i=0; i<buildingBlockNum; i++)
        {
            buildingBlock[mIndex, bIndex, i].SetActive((i < f));
        }

        // 지붕 종류
        int r = Random.Range(0, 3);
        roofSR[mIndex, bIndex].sprite = roofSprite[r];
        Vector3 roofPos = roofSR[mIndex, bIndex].transform.localPosition;
        roofPos.y = floorPosition + buildingBlockHeight * f + roofPosition[r];
        roofSR[mIndex, bIndex].transform.localPosition = roofPos;        

        // 해당 건물의 모든 창문 초기화
        for(int i=0; i<windowNum; i++)
        {
            // 층수에 맞게 활성화/비활성화 결정
            window[mIndex, bIndex, i].SetActive(i < f*2);

            // 창문 타입 결정
            int randType = Random.Range(0, windowTypeNum);
            windowSR[mIndex, bIndex, i].sprite = windowOffSprite[randType];
            window[mIndex, bIndex, i].tag = "Untagged";
        }

        // 창문 종류 결정(최대 3개 활성화)
        for (int i=0; i<3; i++)
        {
            // 바꿀 창문 인덱스 결정
            int randIndex = Random.Range(0, windowNum);

            // 기준점보다 작은값이 나오면 장애물 창문, 그렇지 않으면 불켜진 일반창문
            float obstacleRand = Random.Range(0f, 1f);
            if (obstacleRand <= windowObstaclePercentage)
            {
                window[mIndex, bIndex, randIndex].tag = "WindowObstacle";
                // 창문 타입 결정
                int randType = Random.Range(0, windowTypeNum*2);
                windowSR[mIndex, bIndex, randIndex].sprite = windowObstacleSprite[randType];
            }
            else
            {
                window[mIndex, bIndex, randIndex].tag = "Window";
                // 창문 타입 결정
                int randType = Random.Range(0, windowTypeNum);
                windowSR[mIndex, bIndex, randIndex].sprite = windowOnSprite[randType];
            }
        }
    }

    // 한 층의 타일 세팅; 활성화 여부 결정
    private void SetFloor(int mIndex, int fIndex)
    {
        // 타일 초기화(활성화)
        for(int i=0; i<tileNum; i++)
        {
            tile[mIndex, fIndex, i].SetActive(true);
        }

        // 해당 층의 타일 수 결정(0~3)
        int randTileNum = Random.Range(0, 4);
        int randIndex = 0;

        // 바닥층이거나 타일수 2개인 경우 0~3번째 중 한칸 비활성화
        if (fIndex == 0 || randTileNum==2)
        {
            randIndex = Random.Range(0, tileNum - 1);
            tile[mIndex, fIndex, randIndex].SetActive(false);
        }

        // 바닥층 특정 확률로 빈칸 없앰
        if (fIndex == 0)
        {
            float rand = Random.Range(0f, 1f);
            if(rand<=floorFullPercentage) tile[mIndex, fIndex, randIndex].SetActive(true);
        }

        // 바닥층이 아닐 경우
        if (fIndex != 0)
        {
            tile[mIndex, fIndex, 4].SetActive(false);

            if (randTileNum < 2)
            {
                randIndex = Random.Range(0, tileNum - 1);

                if (randTileNum == 0) tile[mIndex, fIndex, randIndex].SetActive(false);
                for (int i = 0; i < tileNum - 1; i++)
                {
                    if (i != randIndex) tile[mIndex, fIndex, i].SetActive(false);
                }
            }
            else
            {
                // 타일수 2,3개인 경우 1~2번째 중 한칸 비활성화
                // 앞에서 비활성화된 인덱스와 중복되었어도 신경쓰지 않음
                randIndex = Random.Range(1, 3);
                tile[mIndex, fIndex, randIndex].SetActive(false);
            }
        }
    }
}