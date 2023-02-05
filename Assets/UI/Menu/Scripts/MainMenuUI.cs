using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsWinPanels;
    [SerializeField] private GameObject creditsLosePanels;

    [Header("Add it to builds settings")]
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private string mainGameSceneName = "Game";
    [SerializeField] private string creditSceneName = "Credit";
    
    [SerializeField] private bool isCreditsPanelActive = false;
    [SerializeField] private bool isWinPanelActive = false;

    private void Start()
    {
        if(!isCreditsPanelActive)
            ShowMenuPanel();
        else
        {
            if(isWinPanelActive)
                ShowCreditsWinPanel();
            else
                ShowCreditsLosePanel();
        }
    }

    public void PlayGame()
    {
        PlayButtonSound();

        SceneManager.LoadScene(mainGameSceneName);
    }
    
    public void Credit()
    {
        PlayButtonSound();

        SceneManager.LoadScene(creditSceneName);
    }

    public void CreditsWin()
    {
        ShowCreditsWinPanel();
    }
    
    public void CreditsLose()
    {
        ShowCreditsLosePanel();
    }

    public void QuitGame()
    {
        PlayButtonSound();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void BackToMenu()
    {
        PlayButtonSound();

        SceneManager.LoadScene(menuSceneName);
    }

    private void PlayButtonSound()
    {
        //AudioManager.instance.PlayOneShot("UI_button");
    }

    private void ShowCreditsWinPanel()
    {
        mainMenuPanel.SetActive(false);
        creditsWinPanels.SetActive(true);
        creditsLosePanels.SetActive(false);

        PlayButtonSound();
    }
    
    private void ShowCreditsLosePanel()
    {
        mainMenuPanel.SetActive(false);
        creditsWinPanels.SetActive(false);
        creditsLosePanels.SetActive(true);

        PlayButtonSound();
    }

    private void ShowMenuPanel()
    {
        mainMenuPanel.SetActive(true);
        creditsWinPanels.SetActive(false);
        creditsLosePanels.SetActive(false);

        PlayButtonSound();
    }
}
