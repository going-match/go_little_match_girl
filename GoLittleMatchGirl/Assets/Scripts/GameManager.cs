using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public AudioController audioController;
    private static GameManager instance;
    private InGameUI inGameUI;
    private GameObject readyPanel, pausePanel;

    [Header("��ġ ����")]
    [Range(1, 3)]
    public int readyTxtEffectSpeed;

    private IEnumerator inGameCrt;
    private Text readyTxt;

    private bool isStarted;                 // ���� ���� ����
    private bool isPlaying;                 // �ΰ��� �ð��帧 ����
    private bool isClear;                   // ���� Ŭ���� ����
    private bool isPaused;
    private bool isPlayerCenter;            // ���� ���� �� �÷��̾ ȭ���� �߾ӿ� ��ġ�ߴ��� ����

    private int stage;                      // �ΰ��� ���� �ܰ�(=�ӵ�)
    private int lifeNum;                    // �÷��̾� ���� ����
    private int score;                      // ���� �ǸŰ���
    private int goal;                       // ��ǥ �ǸŰ���
    private float spendTime;                // �ΰ��� �ҿ�ð�
    private float endTime;                  // �ΰ��� ����ð�

    private float[] stageSpeed = { 0.15f, 0.2f, 0.25f };
    private int[] stageBoundary = { 53, 160 };

    public enum SCENE { MAIN, INGAME, RESULT, ENDING, };

    public static GameManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (instance == null)
                    Debug.Log("no Singleton obj");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        audioController = transform.GetComponent<AudioController>();
        readyPanel = transform.GetChild(0).GetChild(0).gameObject;
        pausePanel = transform.GetChild(0).GetChild(1).gameObject;
        readyTxt = readyPanel.transform.GetChild(0).GetComponent<Text>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += LoadedSceneEvent;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= LoadedSceneEvent;
    }

    public void ResetStage()
    {
        isStarted = false;
        isPlaying = false;
        isClear = true;

        stage = 0;
        lifeNum = 3;
        score = 0;
        goal = 20;
        spendTime = 0f;
        endTime = 180f;
    }

    public void StartPlay()
    {
        readyPanel.SetActive(false);
        inGameCrt = TimeCrt();
        StartCoroutine(inGameCrt);
        isStarted = true;
        isPlaying = true;
        audioController.Play(AudioController.AUDIO.INGAME);
    }

    public void Resume()
    {
        Debug.Log("���� �簳");
        pausePanel.SetActive(false);
        isPlaying = true;
        isPaused = false;
        audioController.Resume();
    }

    public void Pause()
    {
        Debug.Log("�Ͻ�����");
        pausePanel.SetActive(true);
        isPlaying = false;
        isPaused = true;
        audioController.Pause();
    }

    public void GameOver()
    {
        Debug.Log("���ӿ���");
        isClear = false;
        ChangeScene(SCENE.RESULT);
    }

    public void ShowResult()
    {
        Debug.Log("�÷��� ��� ǥ��");
        isPlaying = false;
        StopCoroutine(inGameCrt);
        ChangeScene(SCENE.RESULT);
    }

    public void AddScore(int value)
    {
        score += value;
        inGameUI.SetScoreTxt(score);
        audioController.PlayAnother(AudioController.AUDIO.ARM);
        audioController.PlayAnother(AudioController.AUDIO.WINDOW);
    }

    public void AddLifeNum(int value)
    {
        if (value > 0) audioController.PlayAnother(AudioController.AUDIO.POTION);
        else audioController.PlayAnother(AudioController.AUDIO.OBSTACLE);

        lifeNum = Mathf.Clamp(lifeNum + value, 0, 3);
        inGameUI.SetLifeIcon(lifeNum);
        if (lifeNum == 0)
        {
            GameOver();
        }
    }

    public bool IsStarted()
    {
        return isStarted;
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public bool IsPlayerCenter()
    {
        return isPlayerCenter;
    }

    public bool IsClear()
    {
        return isClear;
    }
    
    public bool IsHappyEnd()
    {
        return (score>=goal);
    }

    public float GetSpendTimeByPercent()
    {
        return spendTime/endTime;
    }

    public int GetScore()
    {
        return score;
    }

    public int GetLifeNum()
    {
        return lifeNum;
    }

    public int GetStageNum()
    {
        return stage;
    }

    public float GetStageSpeed()
    {
        return stageSpeed[stage];
    }

    public void ChangePlayerCenterFlag(bool value)
    {
        isPlayerCenter = value;
    }

    public void ChangeScene(SCENE scene)
    {
        pausePanel.SetActive(false);
        audioController.Stop();
        SceneManager.LoadScene((int)scene);
    }

    // �� �ε� �� ȣ��
    private void LoadedSceneEvent(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name+" �� �ε��");

        if(scene.buildIndex == (int)SCENE.MAIN)
        {
            audioController.Play(AudioController.AUDIO.MAIN);
        }

        // �ΰ��� ���� ��
        if (scene.buildIndex == (int)SCENE.INGAME)
        {
            ResetStage();
            inGameUI = GameObject.Find("Canvas_InGame").GetComponent<InGameUI>();
            // ���� �غ�
            readyPanel.SetActive(true);
            ChangePlayerCenterFlag(false);
            StartCoroutine(ReadyTextEffectCrt());
        }
        if (scene.buildIndex == (int)SCENE.RESULT)
        {
            if (isClear) audioController.Play(AudioController.AUDIO.GAMECLEAR);
            else audioController.Play(AudioController.AUDIO.GAMEOVER);
        }
        if (scene.buildIndex == (int)SCENE.ENDING)
        {
            if (IsHappyEnd()) audioController.Play(AudioController.AUDIO.HAPPYEND);
            else audioController.Play(AudioController.AUDIO.BADEND);
        }
    }

    // �÷��� �� �ð� ī��Ʈ. ������ ��� �� �ε�
    private IEnumerator TimeCrt()
    {
        while (spendTime<endTime)
        {
            // �÷��� �� �ð� ī��Ʈ
            if (isPlaying) spendTime += Time.deltaTime;
            //Debug.Log("spendtime:"+spendTime);

            // �� �������� �������� �ѱ� ������ ���̵� ����
            if (stage<2 && spendTime > stageBoundary[stage])
            {
                stage++;
                inGameUI.SetFillAreaColor(stage);
            }
            yield return null;
        }
        ShowResult();
    }

    private IEnumerator ReadyTextEffectCrt()
    {
        float time = 0f;
        while (!isPlaying)
        {
            readyTxt.color = new Color(1f, 1f, 1f, Mathf.Abs(Mathf.Cos(readyTxtEffectSpeed * time)) * 0.5f + 0.5f);
            time += Time.deltaTime;
            yield return null;
        }
    }
}