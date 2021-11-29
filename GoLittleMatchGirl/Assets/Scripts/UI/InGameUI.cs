using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private GameObject lifePanel;
    [SerializeField] private Text scoreTxt;
    [SerializeField] private Slider progressSlider;

    private Image[] lifeImg;
    private Image fillAreaBg;

    private void Awake()
    {
        lifeImg = new Image[lifePanel.transform.childCount];
        fillAreaBg = progressSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        for(int i=0; i<lifePanel.transform.childCount; i++)
        {
            lifeImg[i] = lifePanel.transform.GetChild(i).GetComponent<Image>();
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying())
        {
            progressSlider.value = GameManager.Instance.GetSpendTimeByPercent();
        }
    }

    public void SetLifeIcon(int lifeNum)
    {
        for (int i=0; i<lifeImg.Length; i++)
        {
            if (i < lifeNum)
            {
                lifeImg[i].color = new Color(1f, 1f, 1f);
            }
            else lifeImg[i].color = new Color(0.3f, 0.3f, 0.3f);
        }
    }

    public void SetScoreTxt(int score)
    {
        scoreTxt.text = string.Format("{0:D2}", score);
    }

    public void SetFillAreaColor(int stage)
    {
        fillAreaBg.color = new Color(0.97f, 0.8f-(0.3f*(stage)), 0f);
    }
}
