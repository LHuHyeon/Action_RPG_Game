using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Inven_Item : UI_Base
{
    public Item item;
    public Image itemImage;
    public Text itemCountText;
    public int itemCount;
    
    public override void Init()
    {
        // Get<GameObject>((int)GameObjects.ItemIcon).BindEvent((PointerEventData)=>{Debug.Log($"아이템 클릭! : {_name}");});
    }

    // 투명도 설정 (0 ~ 255)
    void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
        itemCountText.color = color;
    }

    // 아이템 등록
    public void AddItem(Item _item, int count = 1)
    {
        item = _item;
        itemImage.sprite = item.itemImage;

        if (Item.ItemType.Equipment != _item.itemType)
        {
            itemCount = count;
            itemCountText.text = itemCount.ToString();
        }
        else
        {
            itemCount = 0;
            itemCountText.text = "";
        }

        SetColor(255);
    }

    // 아이템 개수 업데이트
    public void SetCount(int count = 1)
    {
        itemCount += count;
        itemCountText.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    // 아이템 버리기
    public void ClearSlot()
    {
        item = null;
        itemImage.sprite = null;
        itemCount = 0;
        itemCountText.text = "0";

        SetColor(0);
    }
}
