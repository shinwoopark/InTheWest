using System.Collections.Generic;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public static GameInstance instance;

    public bool bPlaying;
    public bool bPaused;

    public int PlayerHp;

    public string PlayerWeapon = "Pistol";
    public int PistolBullets = 6;
    public int MaxPistolBullets = 6;
    public int RifleBullets = 0;
    public int MaxRifleBullets = 3;

    public int[] ItemInventroy;
    public bool Item1;  //���� ����
    public bool Item2;  //������ ��ȭ
    public bool Item3;  //���� ��ȭ

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
