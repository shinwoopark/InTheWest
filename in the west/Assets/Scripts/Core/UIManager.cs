using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI PlayTime, PistolBullets, RifleBullets, Item1, Item2, Item3, Item4;
    public GameObject Pause;
    public GameObject[] HitHps;
    public RectTransform[] Weapons;

    private void Update()
    {
        UpdatePlayeTime();
        UpdateBulletCount();
        UpdateItemCount();
    }

    private void UpdatePlayeTime()
    {
        PlayTime.text = GameInstance.instance.PlayTime.ToString("F0");
    }

    private void UpdateBulletCount()
    {
        PistolBullets.text = GameInstance.instance.PistolBullets.ToString();
        RifleBullets.text = GameInstance.instance.RifleBullets.ToString();
    }

    private void UpdateItemCount()
    {
        Item1.text = GameInstance.instance.ItemInventroy[0].ToString();
        Item2.text = GameInstance.instance.ItemInventroy[1].ToString();
        Item3.text = GameInstance.instance.ItemInventroy[2].ToString();
        Item4.text = GameInstance.instance.ItemInventroy[3].ToString();
    }

    public void Puase()
    {
        Pause.SetActive(GameInstance.instance.bPause);
    }

    public void ChangePlayerHp()
    {
        for (int i = 3; i > GameInstance.instance.PlayerHp; i--)
        {
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

    //Buttons
    public void ExitMain()
    {
        SceneManager.LoadScene("MainScene");
    }
}
