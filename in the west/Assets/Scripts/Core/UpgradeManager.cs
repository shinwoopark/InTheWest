using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager upgradeManager;

    public List<int> UpgradeModule = new List<int>();

    private void Awake()
    {
        if (upgradeManager == null)
        {
            upgradeManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        Init();
    }

    public void Init()
    {
        UpgradeModule.Clear();

        for (int i = 0; i < 9; i++)
            UpgradeModule.Add(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ChooseModule();
        }
    }

    public void ChooseModule()
    {
        List<int> availableNumbers = new List<int>();

        for (int i = 0; i < UpgradeModule.Count; i++)
        {
            if (UpgradeModule[i] < 3)
            {
                availableNumbers.Add(i);
            }
        }

        int[] chooseModules = new int[3];

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, availableNumbers.Count);
            chooseModules[i] = availableNumbers[randomIndex];
            availableNumbers.RemoveAt(randomIndex);
        }

        int disposableModule = 0;

        if (!GameInstance.instance.bHatItem && Random.Range(0, 4) == 0)
            disposableModule = 9;
        if (disposableModule == 0 && GameInstance.instance.PlayerHp < 5 && Random.Range(0, 4) == 0)
            disposableModule = 10;
        if (disposableModule == 0 && Random.Range(0, 4) == 0)
            disposableModule = 11;

        UiManager.uiManager.UpgradeUi.SetModule(chooseModules, disposableModule);
    }
}
