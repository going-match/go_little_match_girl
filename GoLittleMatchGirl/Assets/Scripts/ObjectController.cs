using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    private GameObject potionGroup;
    private GameObject stoveGroup;
    private GameObject[] potion;
    private GameObject[] stove;

    private float speed;

    private float[,] potionSpawnPercentage = { { 0.2f, 0.3f }, { 0.5f, 0.6f }, { 0.7f, 0.8f } };
    private float[,] stoveSpawnPercentage = { { 0.1f, 0.2f }, { 0.4f, 0.7f }, { 0.8f, 1f } };
    private bool[] isPotionSpawned;     // 阿 积己备埃俊辑 积己登菌绰瘤 咯何
    private bool[] isStoveSpawned;

    private int potionSpawnNum = 3;
    private int stoveSpawnNum = 3;

    private void Awake()
    {
        potionGroup = transform.GetChild(0).gameObject;
        stoveGroup = transform.GetChild(1).gameObject;

        isPotionSpawned = new bool[potionSpawnNum];
        isStoveSpawned = new bool[stoveSpawnNum];

        potion = new GameObject[potionGroup.transform.childCount];
        for (int i=0; i<potion.Length; i++)
        {
            potion[i] = potionGroup.transform.GetChild(i).gameObject;
        }

        stove = new GameObject[stoveGroup.transform.childCount];
        for (int i=0; i<stove.Length; i++)
        {
            stove[i] = stoveGroup.transform.GetChild(i).gameObject;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying())
        {
            speed = GameManager.Instance.GetStageSpeed();

            // 器记 积己
            for (int i = 0; i < potionSpawnNum; i++)
            {
                if (!isPotionSpawned[i] &&
                    GameManager.Instance.GetSpendTimeByPercent() > potionSpawnPercentage[i, 0] &&
                    GameManager.Instance.GetSpendTimeByPercent() < potionSpawnPercentage[i, 1])
                {
                    isPotionSpawned[i] = true;
                    Debug.Log("器记 积己");
                    SpawnPotion();
                }
            }
            for (int i=0; i<potion.Length; i++)
            {
                // 拳搁 观栏肺 荤扼瘤搁 厚劝己拳
                if (potion[i].transform.position.x < -10f)
                {
                    potion[i].SetActive(false);
                }
                // 劝己拳 吝捞搁 谅钎 捞悼
                if (potion[i].activeSelf)
                {
                    Vector3 potionPos = potion[i].transform.position;
                    potionPos.x -= (Time.deltaTime * speed * 5f);
                    potion[i].transform.position = potionPos;
                }
            }

            // 拳肺 积己
            for (int i = 0; i < stoveSpawnNum; i++)
            {
                if (!isStoveSpawned[i] &&
                    GameManager.Instance.GetSpendTimeByPercent() > stoveSpawnPercentage[i, 0] &&
                    GameManager.Instance.GetSpendTimeByPercent() < stoveSpawnPercentage[i, 1])
                {
                    isStoveSpawned[i] = true;
                    Debug.Log("拳肺 积己");
                    SpawnStove();
                }
            }
            for (int i=0; i<stove.Length; i++)
            {
                // 拳搁 观栏肺 荤扼瘤搁 厚劝己拳
                if (stove[i].transform.position.x < -10f)
                {
                    stove[i].SetActive(false);
                }

                // 劝己拳 吝捞搁 谅钎 捞悼
                if (stove[i].activeSelf)
                {
                    Vector3 stovePos = stove[i].transform.position;
                    stovePos.x -= (Time.deltaTime * speed * 5f);
                    stove[i].transform.position = stovePos;
                }
            }
        }
    }

    private void SpawnPotion()
    {
        int randFloor = Random.Range(0, 3);
        potion[randFloor].SetActive(true);

        Vector3 potionPos = potion[randFloor].transform.position;
        potionPos.x = Random.Range(10, 20);
        potion[randFloor].transform.position = potionPos;
    }

    private void SpawnStove()
    {
        int randFloor = Random.Range(1, 2);
        stove[randFloor].SetActive(true);

        Vector3 stovePos = stove[randFloor].transform.position;
        stovePos.x = Random.Range(10, 20);
        stove[randFloor].transform.position = stovePos;
    }
}
