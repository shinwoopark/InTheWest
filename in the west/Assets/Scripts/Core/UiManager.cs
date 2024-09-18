using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager uiManager;

    
    public HelpUi HelpUi;
    public MainUi MainUi;
    public ShopUi ShopUi;

    public GameObject MenuCanvas;
    public GameObject HelpCanvas;
    public GameObject MainCanvas;
    public GameObject ShopCanvas;

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
        ShopCanvas.SetActive(false);
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
}
