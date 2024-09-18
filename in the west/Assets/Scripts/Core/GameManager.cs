using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    private GameObject _menuCanvas;
    private GameObject _helpCanvas;

    [HideInInspector]
    public int CurrentEnemyCount;

    private void Awake()
    {
        if (manager == null)
        {
            manager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(manager);
            manager = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        //SoundManager.soundManager.PlayBgm(SoundManager.Bgm.MainMenu);
    }

    private void Update()
    {
        UpdatePlayTime();
        UpdateInput();
    }

    private void UpdatePlayTime()
    {
        if (!GameInstance.instance.bPlaying) return;

        GameInstance.instance.PlayTime += Time.deltaTime;
    }

    private void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameInstance.instance.bPlaying)
            {
                UiManager.uiManager.SetMenu();
            }
            else
            {
                if (!GameInstance.instance.bShoping)
                {
                    if (GameInstance.instance.bPause)
                    {
                        Debug.Log("!");
                        Time.timeScale = 1;
                        GameInstance.instance.bPause = false;
                    }
                    else
                    {
                        Debug.Log("?");
                        Time.timeScale = 0;
                        GameInstance.instance.bPause = true;
                    }
                }
            }        
        } 
    }

    public void GameStart()
    {
        SceneManager.LoadScene("PlayScene");
        GameInstance.instance.bPlaying = true;
        GameInstance.instance.bPause = false;
        GameInstance.instance.PlayTime = 0;
        GameInstance.instance.Stage = 1;
        GameInstance.instance.PlayerWeapon = "Pistol";
        GameInstance.instance.PistolBullets = 6;
        GameInstance.instance.RifleBullets = 3;

        for (int i = 0; i < GameInstance.instance.ItemInventroy.Length; i++)
        {
            GameInstance.instance.ItemInventroy[i] = 0; 
        }

        GameInstance.instance.Item1 = false;
        GameInstance.instance.Item2 = false;
        GameInstance.instance.Item3 = false;
        GameInstance.instance.Item4 = false;
    }

    public void GameOver()
    {
        GameInstance.instance.bPlaying = false;
        SoundManager.soundManager.StopBgm();
    }
}
