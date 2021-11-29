using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EndingUI : MonoBehaviour
{

    private Image cutSceneImg;
    private Image endingPanelBg;
    private GameObject ending;
    private RectTransform endingRT;
    private Text endingTxt;
    private Image[] optionButtonImg;    // 선택지 버튼 배경

    private bool isPrintEnd;
    private int selectedOptionIndex;
    private int effectSpeed = 1;

    private string[] endingStr =
    {
            "\"하아…\"\n" +
            "성냥팔이 소녀가 하얀 솜사탕 같은 입김을 내뿜었습니다.\n" +
            "\"오늘은 많이 못 팔았네… 그래도 조금만 더 힘내볼까?\"\n" +
            "발걸음을 내딛는 순간, 누군가 성냥팔이 소녀의 어깨를\n" +
            "붙잡았습니다.\n" +
            "놀란 성냥팔이 소녀는 뿌리친 후 뒷걸음질 치며\n" +
            "거리를 두었습니다.\n" +
            "\"누, 누구세요?\"\n" +
            "\"성냥씨 맞으신가요?\"\n" +
            "성냥팔이 소녀를 멈춘 것은 경찰관이었습니다.\n" +
            "\"성냥씨를 무단 주거 침입 및 강매죄로 체포하겠습니다.\"\n" +
            "\"네, 네?! 뭐라고요?!\"\n",

            "\"드디어...!\"\n" +
            "성냥팔이 소녀가 가위로 줄을 잘랐고,\n" + 
            "많은 사람들이 환호성을 질러주었습니다.\n" +
            "\"축하드려요!!\"\n" +
            "\"축하합니다!!\"\n" +
            "성냥팔이 소녀는 성냥을 열심히 팔았고,\n" +  
            "덕분에 난방이 잘 되는 따뜻한 건물을 구입해\n" +
            "㈜성냥팔이 판매 회사를 설립했습니다.\n" +
            "\"이게 다 당신 덕분이에요... 감사합니다!\"\n" +
            "당신이 없었더라면, 성냥팔이 소녀는\n" +  
            "그 추운 겨울에 길바닥에서 쓰러지고 말았겠지만,\n" +
            "함께 성냥을 팔아준 덕분에 그녀를 행복하게 만들어\n" +  
            "주었습니다.\n" + 
            "당신은 성냥씨의 구세주~!\n"
    };

    private void Awake()
    {
        cutSceneImg = transform.GetChild(0).GetComponent<Image>();
        endingPanelBg = transform.GetChild(1).GetComponent<Image>();
        ending = transform.GetChild(1).GetChild(0).gameObject;
        endingRT = ending.GetComponent<RectTransform>();
        endingTxt = transform.GetChild(1).GetChild(0).GetComponent<Text>();
        optionButtonImg = new Image[2];
        for (int i = 0; i < optionButtonImg.Length; i++)
        {
            optionButtonImg[i] = ending.transform.GetChild(i).GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        isPrintEnd = false;
        selectedOptionIndex = 1;
        string str = (GameManager.Instance.IsHappyEnd()) ? "Happy" : "Bad";
        cutSceneImg.sprite = Resources.Load<Sprite>("Images/Ending/"+str);
        StartCoroutine(PrintCrt(GameManager.Instance.IsHappyEnd()));
        StartCoroutine(FadeOutCrt());
        StartCoroutine(ScrollCrt());
    }

    private void Update()
    {
        if (isPrintEnd)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveHighlight(Vector2.left);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveHighlight(Vector2.right);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (selectedOptionIndex == 0)
                {
                    GameManager.Instance.ChangeScene(GameManager.SCENE.INGAME);
                }
                else GameManager.Instance.ChangeScene(GameManager.SCENE.MAIN);
            }
        }
        else
        {
            // 엔터키 누르면 애니메이션 스킵
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopAllCoroutines();
                endingTxt.text = endingStr[Convert.ToInt32(GameManager.Instance.IsHappyEnd())];
                endingPanelBg.color = new Color(0f, 0f, 0f, 0.5f);
                endingRT.anchoredPosition = new Vector2(endingRT.anchoredPosition.x, 0);
                isPrintEnd = true;
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

    // 엔딩 텍스트 출력 애니메이션; 한글자씩 출력
    private IEnumerator PrintCrt(bool isHappyEnd)
    {
        int index = 0;
        while (index<endingStr[Convert.ToInt32(isHappyEnd)].Length)
        {
            endingTxt.text += endingStr[Convert.ToInt32(isHappyEnd)][index];
            yield return new WaitForSeconds(0.05f);
            index++;
        }
        endingTxt.text = endingStr[Convert.ToInt32(isHappyEnd)];
        isPrintEnd = true;
    }

    // 컷신 페이드아웃 애니메이션
    private IEnumerator FadeOutCrt()
    {
        endingPanelBg.color = new Color(0f, 0f, 0f, 0f);
        float alpha = 0f;
        while (alpha < 0.5f)
        {
            alpha += effectSpeed*0.01f;
            endingPanelBg.color = new Color(0f, 0f, 0f, alpha);
            yield return new WaitForSeconds(0.05f);
        }
    }

    // 엔딩 텍스트 스크롤 애니메이션
    private IEnumerator ScrollCrt()
    {
        float posX = endingRT.anchoredPosition.x;
        float posY = -200;
        while (posY<0)
        {
            posY += effectSpeed;
            endingRT.anchoredPosition = new Vector2(posX, posY);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
