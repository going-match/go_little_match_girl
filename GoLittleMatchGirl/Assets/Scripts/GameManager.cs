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

    [Header("수치 설정")]
    [Range(1, 3)]
    public int readyTxtEffectSpeed;

    private Text readyTxt;

    private PLAYMODE state;                 // 인게임 플레이 상태

    private bool isClear;                   // 게임 클리어(완주) 여부
    private bool isCenter;            // 게임 시작 시 플레이어가 화면의 중앙에 위치했는지 여부

    private int stage;                      // 인게임 현재 단계(=속도)
    private int lifeNum;                    // 플레이어 생명 개수
    private int score;                      // 성냥 판매개수
    private int goal;                       // 목표 판매개수
    private float spendTime;                // 인게임 소요시간
    private float endTime;                  // 인게임 종료시간

    private float[] stageSpeed = { 1.5f, 2.0f, 2.5f };
    private int[] stageBoundary = { 53, 106 };

    public enum PLAYMODE { READY, PLAY, PAUSE, END }

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

    private void Update()
    {
        if (spendTime < endTime)
        {
            // 플레이 중 시간 카운트
            if (state==PLAYMODE.PLAY) spendTime += Time.deltaTime;

            // 각 스테이지 기준점을 넘길 때마다 난이도 조정
            if (stage < 2 && spendTime > stageBoundary[stage])
            {
                stage++;
                inGameUI.SetFillAreaColor(stage);
            }
        }
        else if (state == PLAYMODE.PLAY)
        {
            state = PLAYMODE.END;
            ShowResult();
        }
    }

    public void ResetStage()
    {
        state = PLAYMODE.READY;
        isClear = true;

        stage = 0;
        lifeNum = 3;
        score = 0;
        goal = 20;
        spendTime = 0f;
        endTime = 159f;
    }

    public void StartPlay()
    {
        readyPanel.SetActive(false);
        state = PLAYMODE.PLAY;
        audioController.Play(AudioController.AUDIO.INGAME);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        Debug.Log("게임 재개");
        pausePanel.SetActive(false);
        state = PLAYMODE.PLAY;
        audioController.Resume();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        Debug.Log("일시정지");
        pausePanel.SetActive(true);
        state = PLAYMODE.PAUSE;
        audioController.Pause();
    }

    public void GameOver()
    {
        Debug.Log("게임오버");
        isClear = false;
        ChangeScene(SCENE.RESULT);
    }

    public void ShowResult()
    {
        Debug.Log("플레이 결과 표시");

        ChangeScene(SCENE.RESULT);
    }

    public void AddScore(int value)
    {
        score += value;
        if (score < 0) score = 0;
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

    public  PLAYMODE GetState()
    {
        return state;
    }

    public bool isReady()
    {
        return state == PLAYMODE.READY;
    }

    public bool IsPlaying()
    {
        return state == PLAYMODE.PLAY;
    }

    public bool IsPaused()
    {
        return state == PLAYMODE.PAUSE;
    }

    public bool IsPlayerCenter()
    {
        return isCenter;
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
        isCenter = value;
    }

    public void ChangeScene(SCENE scene)
    {
        pausePanel.SetActive(false);
        audioController.Stop();
        SceneManager.LoadScene((int)scene);
    }

    // 씬 로드 시 호출
    private void LoadedSceneEvent(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name+" 씬 로드됨");

        if(scene.buildIndex == (int)SCENE.MAIN)
        {
            audioController.Play(AudioController.AUDIO.MAIN);
        }

        // 인게임 씬일 때
        if (scene.buildIndex == (int)SCENE.INGAME)
        {
            ResetStage();
            inGameUI = GameObject.Find("Canvas_InGame").GetComponent<InGameUI>();
            // 시작 준비
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

    private IEnumerator ReadyTextEffectCrt()
    {
        float time = 0f;
        while (state==PLAYMODE.READY)
        {
            readyTxt.color = new Color(1f, 1f, 1f, Mathf.Abs(Mathf.Cos(readyTxtEffectSpeed * time)) * 0.5f + 0.5f);
            time += Time.deltaTime;
            yield return null;
        }
    }
}