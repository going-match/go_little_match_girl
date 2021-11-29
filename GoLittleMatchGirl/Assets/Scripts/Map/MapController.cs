using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Sprite[] roofSprite = new Sprite[3];
    [SerializeField] private Sprite[] windowSprite = new Sprite[4];     // �Ҳ���â��, ������(�Ϲ�/����/ĥ����)â��

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

                windowSR[0, i, j].sprite = windowSprite[randType];
                window[0, i, j].tag = ((randType == 1) ? "Window" : "Untagged");
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying() && GameManager.Instance.IsPlayerCenter())
        {
            // �� �̵� �� ����
            for(int i=0; i<mapNum; i++)
            {
                Vector3 pos = map[i].transform.position;
                pos.x -= (Time.deltaTime * speed*50f);

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

            windowSR[mIndex, bIndex, i].sprite = windowSprite[0];
            window[mIndex, bIndex, i].tag = "Untagged";
        }

        // â�� ���� ����(�ִ� 3�� Ȱ��ȭ)
        for (int i=0; i<3; i++)
        {
            int randIndex = Random.Range(0, windowNum);
            int randType = Random.Range(1, windowSprite.Length);
            windowSR[mIndex, bIndex, randIndex].sprite = windowSprite[randType];
            if (randType == 1)
            {
                // ������ �Ϲ� â��
                window[mIndex, bIndex, randIndex].tag = "Window";
            }
            else
            {
                // ������ ��ֹ� â��
                window[mIndex, bIndex, randIndex].tag = "WindowObstacle";
            }
        }
    }

    // �� ���� Ÿ�� ����; Ȱ��ȭ ���� ����
    private void SetFloor(int mIndex, int fIndex)
    {
        // Ȱ��ȭ/��Ȱ��ȭ ���� �ʱ�ȭ
        for(int i=0; i<tileNum; i++)
        {
            tile[mIndex, fIndex, i].SetActive(true);
        }

        // �ٴ����� ��� ���� ��ĭ ��: 1~2 (�� �̻��� ���̵��� ���� ���� �� ����)
        if (fIndex == 0)
        {
            //(�ӽ�)�ϴ��� 1���� ��Ȱ��ȭ�غ�
            int randIndex = Random.Range(0,tileNum);
            tile[mIndex, fIndex, randIndex].SetActive(false);
        }
        else
        {
            //(�ӽ�)�ϴ��� 1���� ��Ȱ��ȭ�غ�
            int randIndex = Random.Range(0, tileNum);
            tile[mIndex, fIndex, randIndex].SetActive(false);
        }
    }
}