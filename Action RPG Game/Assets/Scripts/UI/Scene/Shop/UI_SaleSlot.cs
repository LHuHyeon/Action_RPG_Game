using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SaleSlot : UI_Base
{
    public Item item;
    public Image itemImage;
    public Text itemCount_Text;

    private int itemCount;          // 아이템 개수
    public int ItemCount{
        get { return itemCount; }
        set {
            itemCount = value;
            itemCount_Text.text = itemCount.ToString();
        }
    }

    public int gold;

    public UI_Inven_Item invenSlot;  // 판매될 슬롯

    public override void Init()
    {
        
    }

    // 판매 슬롯 세팅
    public void SaleSetting(UI_Inven_Item _slot, int _count=1)
    {
        _slot.IsClick = false;          // 판매 등록된 인벤은 클릭 금지시키기

        item = _slot.item;
        itemImage.sprite = _slot.itemImage.sprite;
        ItemCount = _count;
        gold = _slot.item.itemCoin * _count;
        invenSlot = _slot;

        // TODO : 더 나은 방법이 있다면 교체
        transform.root.GetComponentInChildren<UI_Shop>().SaleGold += gold;

        gameObject.SetActive(true);
    }

    // 판매 취소 버튼
    public void CloseButton()
    {
        // TODO : 더 나은 방법이 있다면 교체
        transform.root.GetComponentInChildren<UI_Shop>().SaleGold -= gold;
        Clear();
    }

    // 초기화
    public void Clear()
    {
        invenSlot.IsClick = true;
        item = null;
        itemImage.sprite = null;
        ItemCount = 0;
        gold = 0;
        invenSlot = null;
        gameObject.SetActive(false);
    }
}
