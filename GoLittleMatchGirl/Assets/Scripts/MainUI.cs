using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    //[SerializeField] private GameObject start, howTo, developers, exit;
    [SerializeField] private GameObject howtoPanel, developersPanel;

    [Range(0, 1)]
    public float highlightTransparency;

    private Image[] buttonImg;
    private int buttonNum;
    private int lastSelectedButtonNum = 0;

    private void Awake()
    {
        buttonNum = transform.childCount-1;
        buttonImg = new Image[buttonNum];
        for(int i=0; i<buttonNum; i++)
        {
            buttonImg[i] = transform.GetChild(i+1).GetComponent<Image>();
        }
        buttonImg[0].color = new Color(1f, 1f, 1f, highlightTransparency);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.RightArrow)) MoveHighlight(Vector2.up);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) MoveHighlight(Vector2.down);
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (howtoPanel.activeSelf) howtoPanel.SetActive(false);
            else if (developersPanel.activeSelf) developersPanel.SetActive(false);
            else Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (lastSelectedButtonNum)
            {
                case 0:
                    break;
                case 1:
                    howtoPanel.SetActive(true);
                    break;
                case 2:
                    developersPanel.SetActive(true);
                    break;
                case 3:
                    Application.Quit();
                    break;
                default:
                    break;
            }
        }
    }

    private void MoveHighlight(Vector2 dir)
    {
        buttonImg[lastSelectedButtonNum].color = new Color(1f, 1f, 1f, 0f);
        lastSelectedButtonNum-=(int)dir.y;
        if (lastSelectedButtonNum < 0 || lastSelectedButtonNum == buttonNum) lastSelectedButtonNum += (int)dir.y*buttonNum;
        buttonImg[lastSelectedButtonNum].color = new Color(1f, 1f, 1f, highlightTransparency);
    }
}
