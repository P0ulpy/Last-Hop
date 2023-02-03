using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsPanels;

    [Header("Add it to builds settings")]
    [SerializeField] private string mainGameSceneName = "Game";

    private void Start()
    {
        ShowMenuPanel();
    }

    public void PlayGame()
    {
        PlayButtonSound();

        SceneManager.LoadScene(mainGameSceneName);
    }

    public void Credits()
    {
        ShowCreditsPanel();
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
        ShowMenuPanel();
    }

    private void PlayButtonSound()
    {
        //AudioManager.instance.PlayOneShot("UI_button");
    }

    private void ShowCreditsPanel()
    {
        mainMenuPanel.SetActive(false);
        creditsPanels.SetActive(true);

        PlayButtonSound();
    }

    private void ShowMenuPanel()
    {
        mainMenuPanel.SetActive(true);
        creditsPanels.SetActive(false);

        PlayButtonSound();
    }
}
