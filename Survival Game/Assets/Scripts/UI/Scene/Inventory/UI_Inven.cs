using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Inven : UI_Scene
{
    GameObject baseInventory;       // 인벤토리 오브젝트

    List<UI_Inven_Item> slots;      // 인벤토리 슬롯을 담는 리스트

    Vector2 startPosition;          // 타이틀을 눌렀을 때 시작좌표

    enum GameObjects
    {
        Inventory,
        CountCheck,
        GridPanel,
        ItemTip,
        Title,
    }

    enum Texts
    {
        Coin_Text,
    }

    public override void Init()
    {
        slots = new List<UI_Inven_Item>();

        gameObject.GetComponent<Canvas>().sortingOrder = 1;
        
        Bind<Text>(typeof(Texts));
        GetText((int)Texts.Coin_Text).text = "0";
        
        Bind<GameObject>(typeof(GameObjects));
        
        SlotReset();    // 인벤토리 초기화

        // 인벤토리 옮기기 EventSystem 등록
        GetObject((int)GameObjects.Title).BindEvent((PointerEventData eventData)=>{
            baseInventory.transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
        }, Define.UIEvent.Drag);

        baseInventory = GetObject((int)GameObjects.Inventory);
        baseInventory.SetActive(false);

        HideItemTip();
        GetObject((int)GameObjects.CountCheck).SetActive(false);
    }

    void SlotReset()
    {
        GameObject gridPanel = Get<GameObject>((int)GameObjects.GridPanel);

        // 그리드 안에 있는 자식을 모두 삭제
        foreach(Transform child in gridPanel.transform) 
            Managers.Resource.Destroy(child.gameObject);

        // 실제 인벤토리 정보를 참고해서 자식을 다시 채움
        for(int i=0; i<15; i++)
            slots.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(parent: gridPanel.transform, name: "Slot"));
    }

    void Update()
    {
        OnInventory();
    }

    // 인벤토리 On/Off
    void OnInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Managers.Game.isInventory = !Managers.Game.isInventory;

            if (Managers.Game.isInventory)
                baseInventory.SetActive(true);
            else
            {
                baseInventory.SetActive(false);
                GetObject((int)GameObjects.CountCheck).SetActive(false);
                baseInventory.transform.position = new Vector3(1920, 540, 0);
            }
        }
    }

    // 아이템을 흭득한 경우
    public void AcquireItem(Item _item, int count = 1)
    {
        if (Item.ItemType.Equipment != _item.itemType)  // 장비가 아닐 때
        {
            for(int i=0; i<slots.Count; i++)            // 인벤토리 슬롯 확인
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item == _item)
                    {
                        slots[i].SetCount(count);       // 아이템 개수 증가
                        return;
                    }
                }
            }
        }

        for(int i=0; i<slots.Count; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, count);         // 슬롯에 넣기
                return;
            }
        }
    }

    // 아이템 버리기
    public void ItemDump(UI_Inven_Item _slotItem, int itemCount)
    {
        GetObject((int)GameObjects.CountCheck).SetActive(true);
        UI_DumpCount dumpSlotItem = GetObject((int)GameObjects.CountCheck).GetComponent<UI_DumpCount>();
        dumpSlotItem.SetSlot(_slotItem, itemCount);
    }

    // 아이템 정보 활성화
    public void ShowItemTip(Item _item)
    {
        if (_item != null)
            GetObject((int)GameObjects.ItemTip).GetComponent<UI_SlotItemTip>().ShowItemTip(_item);
    }

    // 아이템 정보 비활성화
    public void HideItemTip()
    {
        GetObject((int)GameObjects.ItemTip).GetComponent<UI_SlotItemTip>().HideItemTip();
    }
}
