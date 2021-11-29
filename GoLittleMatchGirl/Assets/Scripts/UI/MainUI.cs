using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainUI : MonoBehaviour
{
    [SerializeField] GameObject explainPanel, peoplePanel;

    [Header("메뉴선택 효과")]
    [Range(0, 1)]
    public float highlightTransparency;

    private Image[] menuButtonImg;

    private int buttonNum;
    private int selectedMenuIndex = 0;

    private void Awake()
    {
        buttonNum = transform.childCount-1;
        menuButtonImg = new Image[buttonNum];
        for(int i=0; i<buttonNum; i++)
        {
            menuButtonImg[i] = transform.GetChild(i + 1).GetComponent<Image>();
        }
        menuButtonImg[0].color = new Color(1f, 1f, 1f, highlightTransparency);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.audioController.PlayAnother(AudioController.AUDIO.BUTTON);
            explainPanel.SetActive(false);
            peoplePanel.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!explainPanel.activeSelf && !peoplePanel.activeSelf)
            {
                GameManager.Instance.audioController.PlayAnother(AudioController.AUDIO.BUTTON);
                switch (selectedMenuIndex)
                {
                    case 0:
                        GameManager.Instance.ChangeScene(GameManager.SCENE.INGAME);
                        break;
                    case 1:
                        explainPanel.SetActive(true);
                        break;
                    case 2:
                        peoplePanel.SetActive(true);
                        break;
                    case 3:
                        Application.Quit();
                        break;
                    default:
                        break;
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GameManager.Instance.audioController.PlayAnother(AudioController.AUDIO.BUTTON);
            MoveHighlight(Vector2.up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            GameManager.Instance.audioController.PlayAnother(AudioController.AUDIO.BUTTON);
            MoveHighlight(Vector2.down);
        }
    }

    private void MoveHighlight(Vector2 dir)
    {
        menuButtonImg[selectedMenuIndex].color = new Color(1f, 1f, 1f, 0f);
        selectedMenuIndex-=(int)dir.y;
        if (selectedMenuIndex < 0 || selectedMenuIndex == buttonNum) selectedMenuIndex += (int)dir.y*buttonNum;
        menuButtonImg[selectedMenuIndex].color = new Color(1f, 1f, 1f, highlightTransparency);
    }
}
