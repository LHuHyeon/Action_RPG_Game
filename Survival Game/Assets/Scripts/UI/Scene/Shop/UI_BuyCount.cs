using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuyCount : UI_ItemCount
{
    public Item item;
    public Text goldText;
    public int buyGold;     // 구매할 가격

    public int itemGold;    // 아이템 가격

    protected override void CountUpdate()
    {
        base.CountUpdate();
        
        buyGold = itemGold * itemCount;
        goldText.text = buyGold.ToString();
    }

    // 판매할 아이템 설정
    public void BuySetting(Item _item)
    {
        item = _item;
        itemGold = (int)(_item.itemCoin*1.5f);
        sliderValue.maxValue = Managers.Game.playerStat.Gold / itemGold;
    }

    // 확인 버튼
    public override void CheckButton()
    {
        // 인벤에 아이템 지급
        Managers.Game.baseInventory.AcquireItem(item, itemCount);

        // 플레이어 골드 가격만큼 차감
        Managers.Game.playerStat.Gold -= buyGold;

        // 초기화
        Clear();
    }

    // 취소 버튼
    public override void CancelButton()
    {
        Clear();
    }

    // 초기화
    public override void Clear()
    {
        item = null;
        goldText.text = "";
        buyGold = 0;
        itemGold = 0;

        base.Clear();
    }
}
