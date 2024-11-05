using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager upgradeManager;

    public Dictionary<string ,int> UpgradeModule = new Dictionary<string, int>();

    private void Awake()
    {
        if (upgradeManager == null)
        {
            upgradeManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Init()
    {
        UpgradeModule.Add("비장의 한발", 0);
        UpgradeModule.Add("섬광탄 폭발", 0);
        UpgradeModule.Add("해드샷", 0);
        UpgradeModule.Add("빠른 사격", 0);
        UpgradeModule.Add("광전사", 0);
        UpgradeModule.Add("행운", 0);
        UpgradeModule.Add("멀리 던지기", 0);
        UpgradeModule.Add("작은 선물", 0);
        UpgradeModule.Add("몸통 박치기", 0);
    }
}
