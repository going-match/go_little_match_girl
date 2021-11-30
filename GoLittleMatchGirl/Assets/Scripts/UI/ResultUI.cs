using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    private GameObject[] panel;
    private Image[] optionButtonImg;    // 게임오버 시 선택지 버튼 배경
    private Text scoreTxt;

    private bool isGameClear;
    private bool isScoreCrtEnd;

    private float endingWaitTime;

    private int selectedOptionIndex;    // 0: 다시하기, 1: 메인화면

    private void Awake()
    {
        panel = new GameObject[transform.childCount];
        for(int i=0; i<panel.Length; i++)
        {
            panel[i] = transform.GetChild(i).gameObject;
        }

        optionButtonImg = new Image[2];
        for (int i = 0; i < optionButtonImg.Length; i++)
        {
            optionButtonImg[i] = panel[0].transform.GetChild(i).GetComponent<Image>();
        }
        scoreTxt = panel[1].transform.GetChild(0).GetComponent<Text>();
    }

    private void OnEnable()
    {
        selectedOptionIndex = 0;
        optionButtonImg[0].color = new Color(1f, 1f, 1f, 0.3f);
        isGameClear = GameManager.Instance.IsClear();
        isScoreCrtEnd = false;
        panel[1].SetActive(isGameClear);
        if (isGameClear) StartCoroutine(ScoreTxtCrt());
    }

    private void Update()
    {
        // 게임클리어 및 점수출력 애니메이션 종료 후
        if (isGameClear && isScoreCrtEnd)
        {
            // 엔터 입력 또는 3초 대기 후 엔딩 씬으로 자동 전환
            if (Input.GetKeyDown(KeyCode.Return) || endingWaitTime >= 3.0f)
            {
                GameManager.Instance.ChangeScene(GameManager.SCENE.ENDING);
            }
            else endingWaitTime += Time.deltaTime;
        }
        
        if(!isGameClear)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                GameManager.Instance.audioController.PlayAnother(AudioController.AUDIO.BUTTON);
                MoveHighlight(Vector2.left);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                GameManager.Instance.audioController.PlayAnother(AudioController.AUDIO.BUTTON);
                MoveHighlight(Vector2.right);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GameManager.Instance.audioController.PlayAnother(AudioController.AUDIO.BUTTON);
                if (selectedOptionIndex == 0)
                {
                    GameManager.Instance.ChangeScene(GameManager.SCENE.INGAME);
                }
                else GameManager.Instance.ChangeScene(GameManager.SCENE.MAIN);
            }
        }
    }

    private void MoveHighlight(Vector2 dir)
    {
        optionButtonImg[selectedOptionIndex].color = new Color(1f, 1f, 1f, 0f);
        selectedOptionIndex += (int)dir.x;
        if (selectedOptionIndex < 0 || selectedOptionIndex == optionButtonImg.Length) selectedOptionIndex -= (int)dir.x * optionButtonImg.Length;
        optionButtonImg[selectedOptionIndex].color = new Color(1f, 1f, 1f, 0.3f);
    }

    private IEnumerator ScoreTxtCrt()
    {
        int score = 0;
        while (score <= GameManager.Instance.GetScore())
        {
            scoreTxt.text = string.Format("{0:D2}", score);
            score++;
            yield return new WaitForSeconds(0.05f);
        }
        isScoreCrtEnd = true;
    }

    private IEnumerator EndingWaitCrt()
    {
        float time = 0f;
        while (time < 3f)
        {
            time += Time.deltaTime;
            yield return null;
        }


    }
}
