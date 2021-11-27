using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject optionModal;
    public GameObject instructionModal;
    public FadeIn fader;

    public void Open()
    {
        if (instructionModal.activeSelf)
            return;

        OnMenuOpened();
    }

    public void Resume()
    {
        SceneManager.LoadScene("MainScene");
       // OnMenuClosed();
       // Time.timeScale = 1f;
    }

    public void Toggle()
    {
        if (gameObject.activeSelf)
            Resume();
        else
            Open();
    }

    private void OnMenuOpened()
    {
        Time.timeScale = 0f;
    }

    private void OnMenuClosed()
    {
        optionModal.SetActive(false);
    }

    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Hyerin");
    }
    public void OnExplainButtonClicked()
    {
        SceneManager.LoadScene("ExplainScene");
    }
    public void OnPeopleButtonClicked()
    {
        SceneManager.LoadScene("PeopleScene");
    }
    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    public void OnResumeButtonClicked()
    {
        if (fader == null) return;
        Resume();
    }

    public void OnGalleryButtonClicked()
    {

    }

    public void OnOptionButtonClicked()
    {
        optionModal.SetActive(true);
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

}