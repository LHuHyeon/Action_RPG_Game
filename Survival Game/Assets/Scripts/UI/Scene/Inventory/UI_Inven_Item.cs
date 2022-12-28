using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Inven_Item : UI_Base
{
    public Item item;           // 인벤이 들고 있는 Item 정보
    public Image itemImage;     // UI 이미지
    public Text itemCountText;  // UI 개수
    public int itemCount;       // 아이템 개수

    public UI_BaseSlot havebaseSlot;    // 메인 슬롯에 있는 슬롯과 연결

    public override void Init()
    {
        // 커서가 닿을 시 아이템 정보 UI 활성화
        gameObject.BindEvent((PointerEventData)=>{Managers.Game.baseInventory.ShowItemTip(item);}, Define.UIEvent.Enter);
        
        // 커서가 때어지면 아이템 정보 UI 비활성화
        gameObject.BindEvent((PointerEventData)=>{Managers.Game.baseInventory.HideItemTip();}, Define.UIEvent.Exit);

        // 아이템에 커서를 올리고 오른쪽 클릭 시 아이템 사용
        gameObject.BindEvent((PointerEventData)=>{
            if (Input.GetMouseButtonUp(1))
            {
                Managers.Game.baseInventory.UsingItem(null, this);    // 아이템 사용
            }
        }, Define.UIEvent.Click);
        
        // 아이템이 존재할 시 마우스로 들 수 있다.
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (item != null)
            {
                UI_DragSlot.instance.dragSlot = this;
                UI_DragSlot.instance.DragSetImage(itemImage);

                UI_DragSlot.instance.transform.position = eventData.position;
            }
        }, Define.UIEvent.BeginDrag);

        // 마우스를 드래그할 시 아이템이 이동됨.
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (item != null)
                UI_DragSlot.instance.transform.position = eventData.position;

        }, Define.UIEvent.Drag);

        // 드래그가 끝났을 때
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            // 아이템을 버린 위치가 UI가 아니라면
            if (item != null && !EventSystem.current.IsPointerOverGameObject())
            {
                if (itemCount > 1)  // 버릴 아이템 개수 설정
                {
                    Managers.Game.baseInventory.ItemDump(this, itemCount);
                }
                else                // 1개라면 바로 버리기 
                {
                    GameObject _item = Managers.Resource.Instantiate($"Item/{item.itemType}/{item.itemName}");
                    _item.transform.position = Managers.Game._player.transform.position;
                    ClearSlot();
                }
            }

            // 더블 체크
            Managers.Game._player.GetComponent<ActionController>().TakeUpSlot();

            // 들고 있는 임시 아이템 초기화
            UI_DragSlot.instance.SetColor(0);
            UI_DragSlot.instance.dragSlot = null;

        }, Define.UIEvent.EndDrag);

        // 이 슬롯에 마우스 클릭이 끝나면 아이템 받기
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (UI_DragSlot.instance.dragSlot != null)      // 인벤 슬롯을 받을 때
            {
                // 두 슬롯의 아이템이 같은 아이템일 경우 개수 체크
                if (item == UI_DragSlot.instance.dragSlot.item)
                {
                    int addValue = itemCount + UI_DragSlot.instance.dragSlot.itemCount;
                    if (addValue > item.maxCount)
                    {
                        UI_DragSlot.instance.dragSlot.SetCount(-(item.maxCount-itemCount));
                        SetCount(item.maxCount - itemCount);
                    }
                    else
                    {
                        SetCount(UI_DragSlot.instance.dragSlot.itemCount);
                        UI_DragSlot.instance.dragSlot.ClearSlot();  // 들고 있었던 슬롯은 초기화
                    }
                }
                else
                    ChangeSlot();
            }
            else if (UI_DragSlot.instance.baseSlot != null) // 메인 슬롯을 받을 때
                UI_DragSlot.instance.baseSlot.ClearSlot();

        }, Define.UIEvent.Drop);
    }

    // 현재 슬롯을 다른 슬롯과 바꿀 때 사용하는 메소드
    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;
        UI_BaseSlot _tempSlot = havebaseSlot;

        UI_Inven_Item dragSlot = UI_DragSlot.instance.dragSlot;
        AddItem(dragSlot.item, dragSlot.itemCount, dragSlot.havebaseSlot);

        if (_tempItem != null)
            UI_DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount, _tempSlot);
        else
            UI_DragSlot.instance.dragSlot.ClearSlot(false);
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
    public void AddItem(Item _item, int count = 1, UI_BaseSlot _baseSlot = null)
    {
        item = _item;
        itemImage.sprite = item.itemImage;

        if (_baseSlot != null)
        {
            havebaseSlot = _baseSlot;
            havebaseSlot.haveinvenSlot = this;
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

    // 아이템 개수 업데이트
    public void SetCount(int count = 1)
    {
        itemCount += count;
        itemCountText.text = itemCount.ToString();

        // 메인 슬롯을 들고 있으면 똑같이 수정
        if (havebaseSlot != null)
            havebaseSlot.SetCount(itemCount);

        // 개수가 없다면
        if (itemCount <= 0)
            ClearSlot();
    }

    // 슬롯 초기화
    public void ClearSlot(bool isRemove=true)
    {
        item = null;
        itemImage.sprite = null;
        itemCount = 0;
        itemCountText.text = "0";

        if (isRemove)
        {
            if (havebaseSlot != null)
            {
                havebaseSlot.ClearSlot();
                havebaseSlot = null;
            }
        }
        
        SetColor(0);
        Managers.Game.baseInventory.HideItemTip();
    }
}
