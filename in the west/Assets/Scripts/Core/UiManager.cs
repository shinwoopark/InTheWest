using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager uiManager;

    public HelpUi HelpUi;
    public MainUi MainUi;
    public UpgradeUi UpgradeUi;

    public GameObject MenuCanvas;
    public GameObject HelpCanvas;
    public GameObject MainCanvas;
    public GameObject UpgradeCanvas;

    private void Awake()
    {
        if (uiManager == null)
        {
            uiManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void SetMenu()
    {
        MenuCanvas.SetActive(true);
        HelpCanvas.SetActive(false);
        MainCanvas.SetActive(false);
        UpgradeCanvas.SetActive(false);
    }

    //Buttons

    public void PlayGame()
    {
        MenuCanvas.SetActive(false);
        MainCanvas.SetActive(true);

        GameManager.manager.GameStart();
    }

    public void Help()
    {
        HelpUi.SetPosition();

        MenuCanvas.SetActive(false);
        HelpCanvas.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ExitMenu()
    {
        GameInstance.instance.bPlaying = false;
        GameInstance.instance.bPause = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("MainScene");
        SetMenu();
    }

    public void Restart()
    {
        SceneManager.LoadScene("PlayScene");
        StartCoroutine(ResetItem());
    }

    private IEnumerator ResetItem()
    {
        yield return new WaitForSeconds(0.1f);
        GameManager.manager.GameStart();
    }
}
