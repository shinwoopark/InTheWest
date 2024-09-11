using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    public MainUi MainUi;

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

    private void GameStart()
    {
        GameInstance.instance.Stage = 1;
    }

    private void Start()
    {
        SoundManager.soundManager.PlayBgm(SoundManager.Bgm.MainMenu);
    }

    private void Update()
    {
        if (!GameInstance.instance.bPlaying) return;

        GameInstance.instance.PlayTime += Time.deltaTime;

        UpdateInput();
    }

    private void UpdateInput()
    {
        if (!GameInstance.instance.bShoping && Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameInstance.instance.bPause)
            {
                Time.timeScale = 1;
                GameInstance.instance.bPause = false;
            }           
            else
            {
                Time.timeScale = 0;
                GameInstance.instance.bPause = true;
            }

            MainUi.Puase();
        }
    }

    public void GameOver()
    {
        GameInstance.instance.bPlaying = false;
        SoundManager.soundManager.StopBgm();
    }
}
