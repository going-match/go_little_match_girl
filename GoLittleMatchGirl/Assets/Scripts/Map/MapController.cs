using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Sprite[] roofSprite = new Sprite[3];
    [SerializeField] private Sprite[] windowSprite = new Sprite[4];     // 불꺼진창문, 불켜진(일반/리스/칠면조)창문

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

    private float playerPosOffset = 4f;

    private bool isObjectLoaded = false;

    private List<int>[] activeTileList;

    private float speed;

    private void Awake()
    {
        activeTileList = new List<int>[floorNum];

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

                windowSR[0, i, j].sprite = windowSprite[randType];
                window[0, i, j].tag = ((randType == 1) ? "Window" : "Untagged");
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying() && GameManager.Instance.IsPlayerCenter())
        {
            // 맵 이동 및 세팅
            for(int i=0; i<mapNum; i++)
            {
                Vector3 pos = map[i].transform.position;
                pos.x -= (Time.deltaTime * speed*50f);

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

            windowSR[mIndex, bIndex, i].sprite = windowSprite[0];
            window[mIndex, bIndex, i].tag = "Untagged";
        }

        // 창문 종류 결정(최대 3개 활성화)
        for (int i=0; i<3; i++)
        {
            int randIndex = Random.Range(0, windowNum);
            int randType = Random.Range(1, windowSprite.Length);
            windowSR[mIndex, bIndex, randIndex].sprite = windowSprite[randType];
            if (randType == 1)
            {
                // 불켜진 일반 창문
                window[mIndex, bIndex, randIndex].tag = "Window";
            }
            else
            {
                // 불켜진 장애물 창문
                window[mIndex, bIndex, randIndex].tag = "WindowObstacle";
            }
        }
    }

    // 한 층의 타일 세팅; 활성화 여부 결정
    private void SetFloor(int mIndex, int fIndex)
    {
        // 활성화/비활성화 상태 초기화
        for(int i=0; i<tileNum; i++)
        {
            tile[mIndex, fIndex, i].SetActive(true);
        }

        // 바닥층의 경우 연속 빈칸 수: 1~2 (그 이상은 난이도상 문제 있을 수 있음)
        if (fIndex == 0)
        {
            //(임시)일단은 1개만 비활성화해봄
            int randIndex = Random.Range(0,tileNum);
            tile[mIndex, fIndex, randIndex].SetActive(false);
        }
        else
        {
            //(임시)일단은 1개만 비활성화해봄
            int randIndex = Random.Range(0, tileNum);
            tile[mIndex, fIndex, randIndex].SetActive(false);
        }
    }
}