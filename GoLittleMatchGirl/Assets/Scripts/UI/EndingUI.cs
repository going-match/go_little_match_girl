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
    private Image[] optionButtonImg;    // ������ ��ư ���

    private bool isPrintEnd;
    private int selectedOptionIndex;
    private int effectSpeed = 1;

    private string[] endingStr =
    {
            "\"�Ͼơ�\"\n" +
            "�������� �ҳడ �Ͼ� �ػ��� ���� �Ա��� ���վ����ϴ�.\n" +
            "\"������ ���� �� �Ⱦҳס� �׷��� ���ݸ� �� ��������?\"\n" +
            "�߰����� ����� ����, ������ �������� �ҳ��� �����\n" +
            "����ҽ��ϴ�.\n" +
            "��� �������� �ҳ�� �Ѹ�ģ �� �ް����� ġ��\n" +
            "�Ÿ��� �ξ����ϴ�.\n" +
            "\"��, ��������?\"\n" +
            "\"���ɾ� �����Ű���?\"\n" +
            "�������� �ҳฦ ���� ���� �������̾����ϴ�.\n" +
            "\"���ɾ��� ���� �ְ� ħ�� �� �����˷� ü���ϰڽ��ϴ�.\"\n" +
            "\"��, ��?! ������?!\"\n",

            "\"����...!\"\n" +
            "�������� �ҳడ ������ ���� �߶���,\n" + 
            "���� ������� ȯȣ���� �����־����ϴ�.\n" +
            "\"���ϵ����!!\"\n" +
            "\"�����մϴ�!!\"\n" +
            "�������� �ҳ�� ������ ������ �ȾҰ�,\n" +  
            "���п� ������ �� �Ǵ� ������ �ǹ��� ������\n" +
            "�߼������� �Ǹ� ȸ�縦 �����߽��ϴ�.\n" +
            "\"�̰� �� ��� �����̿���... �����մϴ�!\"\n" +
            "����� ���������, �������� �ҳ��\n" +  
            "�� �߿� �ܿ￡ ��ٴڿ��� �������� ���Ұ�����,\n" +
            "�Բ� ������ �Ⱦ��� ���п� �׳ฦ �ູ�ϰ� �����\n" +  
            "�־����ϴ�.\n" + 
            "����� ���ɾ��� ������~!\n"
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
            // ����Ű ������ �ִϸ��̼� ��ŵ
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

    // ���� �ؽ�Ʈ ��� �ִϸ��̼�; �ѱ��ھ� ���
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

    // �ƽ� ���̵�ƿ� �ִϸ��̼�
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

    // ���� �ؽ�Ʈ ��ũ�� �ִϸ��̼�
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
