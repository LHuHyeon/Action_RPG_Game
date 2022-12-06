using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SlotItemTip : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemName;
    [SerializeField] private Text itemStat;
    [SerializeField] private Text itemDesc;
    
    // 활성화
    public void ShowItemTip(Item _item)
    {
        gameObject.SetActive(true);

        itemIcon.sprite = _item.itemImage;
        itemName.text = _item.itemName;
        itemDesc.text = _item.itemDesc;
        itemStat.text = Managers.Game.itemDatabase.GetStat(_item);
    }

    // 비활성화
    public void HideItemTip()
    {
        gameObject.SetActive(false);
    }
}
