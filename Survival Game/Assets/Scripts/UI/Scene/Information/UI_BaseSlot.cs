using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_BaseSlot : UI_Base
{
    public Item item;
    public Image itemImage;     // UI 이미지
    public Text itemCountText;  // UI 개수
    public int itemCount;       // 아이템 개수
    public GameObject currentEffect;    // 플레이어가 들고 있다면
    public Text slotNumber;

    public UI_Inven_Item haveinvenSlot; // 인벤 슬롯에 있는 슬롯과 연결
    
    public override void Init()
    {
        // 아이템이 존재할 시 마우스로 들 수 있다.
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (item != null && Managers.Game.isInventory)
            {
                UI_DragSlot.instance.baseSlot = this;
                UI_DragSlot.instance.DragSetImage(itemImage);

                UI_DragSlot.instance.transform.position = eventData.position;
            }
        }, Define.UIEvent.BeginDrag);

        // 마우스를 드래그할 시 아이템이 이동됨.
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (item != null && Managers.Game.isInventory)
                UI_DragSlot.instance.transform.position = eventData.position;

        }, Define.UIEvent.Drag);

        // 드래그가 끝났을 때
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (Managers.Game.isInventory)
            {
                // 아이템을 버린 위치가 UI가 아니라면
                if (item != null && !EventSystem.current.IsPointerOverGameObject())
                    ClearSlot();

                // 더블 체크
                Managers.Game._player.GetComponent<ActionController>().TakeUpSlot();

                // 들고 있는 임시 아이템 초기화
                UI_DragSlot.instance.SetColor(0);
                UI_DragSlot.instance.baseSlot = null;
            }

        }, Define.UIEvent.EndDrag);

        // 이 슬롯에 마우스 클릭이 끝나면 아이템 받기
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (Managers.Game.isInventory)
            {
                if (UI_DragSlot.instance.baseSlot != null)
                    ChangeSlot();
                else if (UI_DragSlot.instance.dragSlot != null)
                    ConnectionSlot();
            }

        }, Define.UIEvent.Drop);
    }

    // 인벤토리에서 대표 슬롯에 등록할 때
    private void ConnectionSlot()
    {
        UI_DragSlot.instance.dragSlot.havebaseSlot.ClearSlot();
        AddItem(UI_DragSlot.instance.dragSlot.item, UI_DragSlot.instance.dragSlot.itemCount, UI_DragSlot.instance.dragSlot);
        UI_DragSlot.instance.dragSlot = null;
    }

    // 현재 슬롯을 다른 슬롯과 바꿀 때 사용하는 메소드
    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;
        UI_Inven_Item _tempSlot = haveinvenSlot;

        UI_BaseSlot baseSlot = UI_DragSlot.instance.baseSlot;
        AddItem(baseSlot.item, baseSlot.itemCount, baseSlot.haveinvenSlot);

        if (_tempItem != null)
            UI_DragSlot.instance.baseSlot.AddItem(_tempItem, _tempItemCount, _tempSlot);
        else
            UI_DragSlot.instance.baseSlot.ClearSlot();
    }

    // 아이템 등록
    public void AddItem(Item _item, int count = 1, UI_Inven_Item _invenSlot=null)
    {
        item = _item;
        itemImage.sprite = item.itemImage;
        
        // 인벤 슬롯 연결
        if (_invenSlot != null)
        {
            haveinvenSlot = _invenSlot;
            haveinvenSlot.havebaseSlot = this;
        }

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

    // 투명도 설정 (0 ~ 255)
    void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
        itemCountText.color = color;
    }

    // 아이템 개수 업데이트
    public void SetCount(int count)
    {
        itemCount = count;
        itemCountText.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    // 슬롯 초기화
    public void ClearSlot()
    {
        item = null;
        itemImage.sprite = null;
        itemCount = 0;
        itemCountText.text = "0";
        haveinvenSlot = null;

        SetColor(0);
    }
}
