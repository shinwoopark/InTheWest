using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUi : MonoBehaviour
{
    public void AddItem1()
    {
        GameInstance.instance.ItemInventroy[0] = GameInstance.instance.ItemInventroy[0] + 3;
        CloseShop();
    }

    public void AddItem2()
    {
        GameInstance.instance.ItemInventroy[1]++;
        CloseShop();
    }

    public void AddItem3()
    {
        GameInstance.instance.ItemInventroy[2]++;
        CloseShop();
    }

    public void AddItem4()
    {
        GameInstance.instance.ItemInventroy[3]++;
        CloseShop();
    }

    private void CloseShop()
    {
        GameInstance.instance.bShoping = false;
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
