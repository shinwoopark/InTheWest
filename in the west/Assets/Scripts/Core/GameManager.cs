using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    public UIManager UIManager;

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

        GameInstance.instance.PlayTime += Time.deltaTime;

        UpdateInput();
    }

    private void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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

            UIManager.Puase();
        }
    }

    public void GameOver()
    {
        GameInstance.instance.bPlaying = false;
    }
}
