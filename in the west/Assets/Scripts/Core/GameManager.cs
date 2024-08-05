using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

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

    private void Update()
    {
        if (!GameInstance.instance.bPlaying) return;

        UpdateInput();
    }

    private void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameInstance.instance.bPaused)
                GameInstance.instance.bPaused = false;
            else
                GameInstance.instance.bPaused = true;
        }
    }

    private void UpdateTime()
    {
        if (GameInstance.instance.bPaused)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void GameOver()
    {
        GameInstance.instance.bPlaying = false;
    }
}
