using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuySlot : UI_Base
{
    public Item item;       // 아이템
    public Image itemImage; // 아이템 Image
    public Text itemName;   // 이름 txt
    public Text goldCount;  // 가격 txt (가격은 item.itemCoin * 1.5)

    public UI_BuyCount buyCount;

    public override void Init()
    {
    }

    // 구매 슬롯 세팅
    public void BuySlotSetting(Item _item)
    {
        item = _item;
        itemImage.sprite = _item.itemImage;
        itemName.text = item.itemName;
        goldCount.text = ((int)(item.itemCoin * 1.5f)).ToString();

        gameObject.SetActive(true);
    }

    // 구매할 아이템 개수 체크 (Button)
    public void BuyCountCheck()
    {
        if (Managers.Game.playerStat.Gold >= ((int)(item.itemCoin * 1.5f)))
        {
            buyCount.gameObject.SetActive(true);
            buyCount.BuySetting(item);
        }
        else
        {
            Debug.Log("Gold가 부족합니다!");
        }
    }
}
