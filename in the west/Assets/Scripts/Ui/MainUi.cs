using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainUi : MonoBehaviour
{
    public TextMeshProUGUI PlayTime, PistolBullets, RifleBullets, RifleBullet, Item1, Item2, Item3;
    public GameObject Item4;
    public RectTransform Item4_tr;

    public GameObject Pause;

    public GameObject[] HitHps;
    public RectTransform[] Weapons;
    public GameObject Buttons;

    public GameObject GameClear_gb;
    public TextMeshProUGUI Record;

    public GameObject GameOver_gb;
    public Image BackGroundColor;

    public Slider EXPBar;

    public GameObject[] UpgradeModules;

    public GameObject HittingBodyCoolTime_gb;
    public Slider HittingBodyCoolTimer;

    private void Update()
    {
        UpdatePlayeTime();
        UpdatePause();
        UpdateGameOver();
        UpdateBulletCount();
        UpdateItemCount();
        UpdateHatItem();
        UpdatePuase();
        UpdateUpgradeModules();
        UpdateLastShot();
        UpdateHittingBodyTimer();
        UpdateEXP();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameClear_gb.SetActive(false);

        BackGroundColor.color = new Color(0, 0, 0, 0);
        Buttons.SetActive(false);
        GameOver_gb.SetActive(false);
    }

    private void UpdatePlayeTime()
    {
        PlayTime.text = GameInstance.instance.PlayTime.ToString("F2");
    }

    private void UpdatePause()
    {
        Pause.SetActive(GameInstance.instance.bPause);
    }

    public void GameClear()
    {
        GameClear_gb.SetActive(true);
        Record.text = GameInstance.instance.PlayTime.ToString("F2");
    }

    private void UpdateGameOver()
    {
        if (GameInstance.instance.bPlaying)
            return;

        GameOver_gb.SetActive(true);

        BackGroundColor.color += new Color(0, 0, 0, 0.3f) * Time.deltaTime;

        if (BackGroundColor.color.a >= 1)
        {
            Buttons.SetActive(true);
        }
    }

    private void UpdateBulletCount()
    {
        PistolBullets.text = GameInstance.instance.PistolBullets.ToString();
        RifleBullets.text = GameInstance.instance.RifleBullets.ToString();
    }

    private void UpdateItemCount()
    {
        RifleBullet.text = GameInstance.instance.ItemInventroy[0].ToString();
        Item1.text = GameInstance.instance.ItemInventroy[1].ToString();
        Item2.text = GameInstance.instance.ItemInventroy[2].ToString();
        Item3.text = GameInstance.instance.ItemInventroy[3].ToString();
    }

    private void UpdateHatItem()
    {
        if (GameInstance.instance.bHatItem)
        {
            float posX = 0;

            for (int i = 1; i < GameInstance.instance.PlayerHp; i++)
            {
                posX += 90;
            }

            Item4_tr.anchoredPosition = new Vector3(-765 + posX, 450, 0);

            Item4.SetActive(true);
        }
        else
            Item4.SetActive(false);
    }

    private void UpdatePuase()
    {
        Pause.SetActive(GameInstance.instance.bPause);
    }

    private void UpdateUpgradeModules()
    {
        for (int i = 0; i < UpgradeModules.Length; ++i)
        {
            if (UpgradeManager.upgradeManager.UpgradeModule[i] > 0)
                UpgradeModules[i].SetActive(true);
            else
                UpgradeModules[i].SetActive(false);
        }
    }

    private void UpdateLastShot()
    {
        if (UpgradeManager.upgradeManager.UpgradeModule[0] > 0)
        {
            if (GameInstance.instance.PistolBullets == 1)
                PistolBullets.color = new Color32(175, 0, 0, 255);
            else
                PistolBullets.color = Color.black;

            if (GameInstance.instance.RifleBullets == 1)
                RifleBullets.color = new Color32(175, 0, 0, 255);
            else
                RifleBullets.color = Color.black;
        }
    }

    private void UpdateHittingBodyTimer()
    {
        if (UpgradeManager.upgradeManager.UpgradeModule[8] > 0)
        {
            HittingBodyCoolTime_gb.transform.position = GameManager.manager.Player.transform.position + new Vector3(0, 1, 0);

            HittingBodyCoolTimer.value = GameInstance.instance.HittingCoolBodyTime;

            if (HittingBodyCoolTimer.value == HittingBodyCoolTimer.maxValue)
                HittingBodyCoolTime_gb.SetActive(false);
            else
                HittingBodyCoolTime_gb.SetActive(true);
        }
        else
            HittingBodyCoolTime_gb.SetActive(false);
    }

    private void UpdateEXP()
    {
        EXPBar.maxValue = GameInstance.instance.MaxEXP;
        EXPBar.value = GameInstance.instance.PlayerEXP;
    }

    public void ChangePlayerHp()
    {
        for (int i = 5; i > GameInstance.instance.PlayerHp; i--)
        {
            if (i > 0)
                HitHps[i - 1].SetActive(true);
        }

        for (int i = 0; i < GameInstance.instance.PlayerHp; i++)
        {
            HitHps[i].SetActive(false);
        }
    }

    public void ChangeWeapon()
    {
        if(GameInstance.instance.PlayerWeapon == "Pistol")
        {
            Weapons[0].anchoredPosition = new Vector3(-815, 350, 0);
            Weapons[0].localScale = Vector3.one;
            Weapons[1].anchoredPosition = new Vector3(-850, 270, 0);
            Weapons[1].localScale = new Vector3(0.75f, 0.75f, 1);
        }
        else
        {
            Weapons[0].anchoredPosition = new Vector3(-850, 350, 0);
            Weapons[0].localScale = new Vector3(0.75f, 0.75f, 1);
            Weapons[1].anchoredPosition = new Vector3(-815, 270, 0);
            Weapons[1].localScale = Vector3.one;
        }
    }
}
