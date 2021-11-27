using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private SceneChanger sceneChanger;
    private GameObject gameOverEndingPanel, gameClearPanel;

    private Image gameOverEndingPanelImg;
    private Text salesTxt, gameOverTxt, endingStoryTxt;

    private void Awake()
    {
        var obj = FindObjectsOfType<GameManager>();
        if (obj.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        sceneChanger = GetComponent<SceneChanger>();
        gameOverEndingPanel = transform.GetChild(0).GetChild(0).gameObject;
        gameClearPanel = transform.GetChild(0).GetChild(1).gameObject;
    }

    public void ShowEnding()
    {
        gameOverEndingPanel.SetActive(true);
        gameOverEndingPanelImg.color = new Color(0f, 0f, 0f, 0.5f);
    }

    public void GameStart()
    {
        sceneChanger.Move(SceneChanger.SCENE.STAGE);
    }

    public void GameOver()
    {
        gameOverEndingPanel.SetActive(true);
        gameOverEndingPanelImg.color = Color.black;
    }

    public void GameClear(int sales)
    {
        gameClearPanel.SetActive(true);
        salesTxt.text = sales.ToString();
    }

    public void GoMain()
    {
        sceneChanger.Move(SceneChanger.SCENE.MAIN);
    }
}
