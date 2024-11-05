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
        UpgradeModule.Add("������ �ѹ�", 0);
        UpgradeModule.Add("����ź ����", 0);
        UpgradeModule.Add("�ص弦", 0);
        UpgradeModule.Add("���� ���", 0);
        UpgradeModule.Add("������", 0);
        UpgradeModule.Add("���", 0);
        UpgradeModule.Add("�ָ� ������", 0);
        UpgradeModule.Add("���� ����", 0);
        UpgradeModule.Add("���� ��ġ��", 0);
    }
}
