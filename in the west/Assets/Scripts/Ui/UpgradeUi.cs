using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UpgradeUi : MonoBehaviour
{
    public Image[] Images;
    public TextMeshProUGUI[] Names;
    public TextMeshProUGUI[] Explanations;

    public Sprite[] ModuleImages;

    private List<string> _ModulesNames = new List<string>
    {
        "마지막 한 발",
        "섬광탄 폭발",
        "헤드샷",
        "재빠른 사격",
        "광전사",
        "행운",
        "멀리 던지기",
        "작은 선물",
        "몸통박치기",
        "카우보이 모자",
        "하트",
        "만능 주머니"
    };

    private List<string> _explanationModules = new List<string>
    {
        "기본 공격 시 마지막 총알이\n추가 피해를 줍니다",
        "섬광탄이 피해를 입힙니다",
        "낮은 확율로 기본 공격이\n적을 즉사 시킵니다 (보스 X)",
        "구르기 직후 기본 공격이\n추가 피해를 줍니다",
        "체력이 1이면\n기본 공격이 강화됩니다",
        "적 처치시 아이템 획득\n확률이 증가합니다",
        "섬광탄의 사거리 증가합니다",
        "낮은 확률로 구르기 사용시\n섬광탄을 남깁니다",
        "일정 시간동안 구르기를\n사용하지 않으면\n다음 구르기가 적을 밀쳐냅니다",
        "모자 아이템을 획득합니다",
        "체력을 1회복합니다",
        "모자 아이템 외 모든 아이템을 1개 획득합니다"
    };

    private int[] _currentModules;

    public void SetModule(int[] modules, int disposableModule)
    {
        _currentModules = new int[modules.Length];

        for (int i = 0; i < modules.Length; i++)
        {
            _currentModules[i] = modules[i];
            Images[i].sprite = ModuleImages[modules[i]];
            Names[i].text = _ModulesNames[modules[i]];
            Explanations[i].text = _explanationModules[modules[i]];
        }

        if (disposableModule != 0)
        {
            int changeModule = Random.Range(0, 3);

            _currentModules[changeModule] = disposableModule;
            Images[changeModule].sprite = ModuleImages[disposableModule];
            Names[changeModule].text = _ModulesNames[disposableModule];
            Explanations[changeModule].text = _explanationModules[disposableModule];
        }
    }

    private void CloseShop()
    {
        GameInstance.instance.bUpgrading = false;
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    //Buttons

    public void ChooseModules(int number)
    {
        if (_currentModules[number] < UpgradeManager.upgradeManager.UpgradeModule.Count)
        {
            UpgradeManager.upgradeManager.UpgradeModule[_currentModules[number]]++;
        }
        else
        {
            switch (_currentModules[number])
            {
                case 9:
                    GameInstance.instance.bHatItem = true;
                    break;
                case 10:
                    GameInstance.instance.PlayerHp++;
                    break;
                case 11:
                    for (int i = 0; i < GameInstance.instance.ItemInventroy.Length; i++)
                        GameInstance.instance.ItemInventroy[i]++;
                    break;
            }
        }
    }
}
