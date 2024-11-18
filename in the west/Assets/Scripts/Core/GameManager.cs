using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SoundManager;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    private GameObject _menuCanvas;
    private GameObject _helpCanvas;

    //[HideInInspector]
    public GameObject Player;

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
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        UpdatePlayTime();
        UpdateInput();
        UpdateLevel();
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

    private void UpdateLevel()
    {
        if(GameInstance.instance.PlayerEXP >= GameInstance.instance.MaxEXP)
        {
            GameInstance.instance.PlayerLevel++;
            GameInstance.instance.PlayerEXP -= GameInstance.instance.MaxEXP;
            GameInstance.instance.MaxEXP += 2;
            UiManager.uiManager.UpgradeUi.gameObject.SetActive(true);
            UpgradeManager.upgradeManager.ChooseModule();
            SoundManager.soundManager.PlaySfx(SoundManager.Sfx.OpenUpgrade);
            Time.timeScale = 0;
            GameInstance.instance.bUpgrading = true;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlayScene")
            Player = GameObject.Find("Player");
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
        GameInstance.instance.PlayerEXP = 0;
        GameInstance.instance.PlayerLevel = 0;
        GameInstance.instance.MaxEXP = 3;
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

        UpgradeManager.upgradeManager.Init();
    }

    public void GameClear()
    {
        GameInstance.instance.bPlaying = false;
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
