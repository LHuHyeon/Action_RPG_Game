using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 아이템 정보 출력 클래스
public class UI_SlotItemTip : MonoBehaviour
{
    [SerializeField] private Image itemIcon;    // 이미지
    [SerializeField] private Text itemName;     // 이름
    [SerializeField] private Text itemGrade;    // 등급
    [SerializeField] private Text itemLevel;    // 레벨
    [SerializeField] private Text itemStat;     // 능력치
    [SerializeField] private Text itemDesc;     // 설명
    [SerializeField] private Text mouseInfo;    // 마우스 설명
    
    // 활성화
    public void ShowItemTip(Item _item)
    {
        gameObject.SetActive(true);

        itemName.text = _item.itemName;
        itemGrade.text = _item.itemGrade.ToString();
        itemIcon.sprite = _item.itemImage;
        itemDesc.text = _item.itemDesc;

        // 장비 or 아이템 분류
        EqItem eqItem = _item as EqItem;
        if (eqItem != null)
        {
            itemLevel.text = "필요 레벨 " + eqItem.minLevel;
            mouseInfo.text = "우클릭, 드래그 - 장착";
            itemStat.text = Managers.Game.itemDatabase.GetStat(_item);
        }
        else
        {
            if (_item.itemType == Item.ItemType.Used)
            {
                itemLevel.text = "사용 아이템";
                mouseInfo.text = "우클릭 - 사용";
                itemStat.text = Managers.Game.itemDatabase.GetStat(_item);
            }
            else if (_item.itemType == Item.ItemType.ETC)
            {
                itemLevel.text = "기타 아이템";
                mouseInfo.text = "";
            }
        }
    }

    // 비활성화
    public void HideItemTip()
    {
        gameObject.SetActive(false);
    }
}
