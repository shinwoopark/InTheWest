using System.Collections.Generic;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public static GameInstance instance;

    public bool bPlaying;
    public bool bPause;

    public float PlayTime;

    public int Stage;
    public bool bUpgrading;

    public int PlayerHp;
    public int PlayerLevel;
    public int PlayerEXP = 0;
    public int MaxEXP = 3;

    public string PlayerWeapon = "Pistol";
    public int PistolBullets = 6;
    public int MaxPistolBullets = 6;
    public int RifleBullets = 0;
    public int MaxRifleBullets = 3;

    public int[] ItemInventroy;
    public bool Item1;  //빠른 장전
    public bool Item2;  //구르기 강화
    public bool bHatItem;  //1회 무적

    public float HittingCoolBodyTime;

    public bool bBossSpawn;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
}
