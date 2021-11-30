using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Sprite[] roofSprite = new Sprite[3];
    [SerializeField] private Sprite[] windowOffSprite = new Sprite[3];  // �� ���� â��(3����)
    [SerializeField] private Sprite[] windowOnSprite = new Sprite[3];  // �� ���� â��(3����)
    [SerializeField] private Sprite[] windowObstacleSprite = new Sprite[6];     // �� ���� ��ֹ�(����/ĥ����) â�� (�� 3����)

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

        // ���� �� �ʿ� ���Ͽ� �ٴ��� Ÿ�� ��� Ȱ��ȭ(������ �� �÷��̾ �߾����� �̵��ϴ� ���� ������ ����)
        for (int i = 0; i < tileNum; i++)
        {
            tile[0, 0, i].SetActive(true);
            //tileCol[0, 0, i].isTrigger = false;
        }
        // ���� �� �ʿ� ���Ͽ� â�� Ÿ�� �Ҳ���/�������Ϲ� â�����θ� ����
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

            // �� �̵� �� ����
            for (int i=0; i<mapNum; i++)
            {
                Vector3 pos = map[i].transform.position;
                pos.x -= (Time.deltaTime * speed * 5f);

                // �ش� ���� �� ������ ������ �� �� �� ����
                if (pos.x < -20)
                {
                    pos.x = 40;
                    SetMap(i);
                }
                map[i].transform.position = pos;
            }
        }
    }

    // �ϳ��� �� ��Ʈ ����; ���� �� ��Ʈ�� ��ġ �� ��� �ǹ� ����
    private void SetMap(int index)
    {
        //map[index].transform.position = new Vector3();

        // �ǹ� ����
        for(int i=0; i<buildingNum; i++) SetBuilding(index, i);

        // ���� ����
        for(int i=0; i<floorNum; i++) SetFloor(index, i);

    }

    // �ϳ��� �ǹ� ����; �ǹ� ����, ���� ����, â�� ���� ����
    private void SetBuilding(int mIndex, int bIndex)
    {
        // �ǹ� ����
        int f = Random.Range(2, 4);
        for(int i=0; i<buildingBlockNum; i++)
        {
            buildingBlock[mIndex, bIndex, i].SetActive((i < f));
        }

        // ���� ����
        int r = Random.Range(0, 3);
        roofSR[mIndex, bIndex].sprite = roofSprite[r];
        Vector3 roofPos = roofSR[mIndex, bIndex].transform.localPosition;
        roofPos.y = floorPosition + buildingBlockHeight * f + roofPosition[r];
        roofSR[mIndex, bIndex].transform.localPosition = roofPos;        

        // �ش� �ǹ��� ��� â�� �ʱ�ȭ
        for(int i=0; i<windowNum; i++)
        {
            // ������ �°� Ȱ��ȭ/��Ȱ��ȭ ����
            window[mIndex, bIndex, i].SetActive(i < f*2);

            // â�� Ÿ�� ����
            int randType = Random.Range(0, windowTypeNum);
            windowSR[mIndex, bIndex, i].sprite = windowOffSprite[randType];
            window[mIndex, bIndex, i].tag = "Untagged";
        }

        // â�� ���� ����(�ִ� 3�� Ȱ��ȭ)
        for (int i=0; i<3; i++)
        {
            // �ٲ� â�� �ε��� ����
            int randIndex = Random.Range(0, windowNum);

            // ���������� �������� ������ ��ֹ� â��, �׷��� ������ ������ �Ϲ�â��
            float obstacleRand = Random.Range(0f, 1f);
            if (obstacleRand <= windowObstaclePercentage)
            {
                window[mIndex, bIndex, randIndex].tag = "WindowObstacle";
                // â�� Ÿ�� ����
                int randType = Random.Range(0, windowTypeNum*2);
                windowSR[mIndex, bIndex, randIndex].sprite = windowObstacleSprite[randType];
            }
            else
            {
                window[mIndex, bIndex, randIndex].tag = "Window";
                // â�� Ÿ�� ����
                int randType = Random.Range(0, windowTypeNum);
                windowSR[mIndex, bIndex, randIndex].sprite = windowOnSprite[randType];
            }
        }
    }

    // �� ���� Ÿ�� ����; Ȱ��ȭ ���� ����
    private void SetFloor(int mIndex, int fIndex)
    {
        // Ÿ�� �ʱ�ȭ(Ȱ��ȭ)
        for(int i=0; i<tileNum; i++)
        {
            tile[mIndex, fIndex, i].SetActive(true);
        }

        // �ش� ���� Ÿ�� �� ����(0~3)
        int randTileNum = Random.Range(0, 4);
        int randIndex = 0;

        // �ٴ����̰ų� Ÿ�ϼ� 2���� ��� 0~3��° �� ��ĭ ��Ȱ��ȭ
        if (fIndex == 0 || randTileNum==2)
        {
            randIndex = Random.Range(0, tileNum - 1);
            tile[mIndex, fIndex, randIndex].SetActive(false);
        }

        // �ٴ��� Ư�� Ȯ���� ��ĭ ����
        if (fIndex == 0)
        {
            float rand = Random.Range(0f, 1f);
            if(rand<=floorFullPercentage) tile[mIndex, fIndex, randIndex].SetActive(true);
        }

        // �ٴ����� �ƴ� ���
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
                // Ÿ�ϼ� 2,3���� ��� 1~2��° �� ��ĭ ��Ȱ��ȭ
                // �տ��� ��Ȱ��ȭ�� �ε����� �ߺ��Ǿ�� �Ű澲�� ����
                randIndex = Random.Range(1, 3);
                tile[mIndex, fIndex, randIndex].SetActive(false);
            }
        }
    }
}