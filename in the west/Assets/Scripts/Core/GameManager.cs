using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    private UIManager _uiManager;

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

        _uiManager = GetComponent<UIManager>();
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
                GameInstance.instance.bPause = false;
            else
                GameInstance.instance.bPause = true;

            _uiManager.Puase();
        }
    }

    private void UpdateTime()
    {
        if (GameInstance.instance.bPause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void GameOver()
    {
        GameInstance.instance.bPlaying = false;
    }
}
