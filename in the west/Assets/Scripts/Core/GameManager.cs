using System.Collections;
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
                UiManager.uiManager.ExitMenu();
            }
            else
            {
                if (!GameInstance.instance.bUpgrading)
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
        UiManager.uiManager.MainUi.ChangeWeapon();
        GameInstance.instance.PlayerHp = 5;
        UiManager.uiManager.MainUi.ChangePlayerHp();
        GameInstance.instance.PistolBullets = 6;
        GameInstance.instance.RifleBullets = 3;

        for (int i = 0; i < GameInstance.instance.ItemInventroy.Length; i++)
        {
            GameInstance.instance.ItemInventroy[i] = 0; 
        }

        GameInstance.instance.Item1 = false;
        GameInstance.instance.Item2 = false;
        GameInstance.instance.bHatItem = false;
    }

    public void GameClear()
    {
        UiManager.uiManager.MainUi.GameClear();
        SoundManager.soundManager.PlaySfx(SoundManager.Sfx.GameClear);
    }

    public void GameOver()
    {
        GameInstance.instance.bPlaying = false;
        SoundManager.soundManager.StopBgm();
        SoundManager.soundManager.PlaySfx(SoundManager.Sfx.GameOver);
    }
}
