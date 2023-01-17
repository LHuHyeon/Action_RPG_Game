using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 인벤토리 클래스
public class UI_Inven : UI_Scene
{
    private int gold;
    public int Gold{
        get { return gold; }
        set {
            gold = value;
            GetText((int)Texts.Coin_Text).text = gold.ToString();
        }
    }

    List<UI_Inven_Item> slots;      // 인벤토리 슬롯을 담는 리스트

    Vector2 startPosition;          // 타이틀을 눌렀을 때 시작좌표

    enum GameObjects
    {
        Inventory,      // 바로 밑에 자식
        CountCheck,     // 개수 체크
        GridPanel,      // 인벤 슬롯 정렬
        ItemTip,        // 아이템 정보창
        Title,
    }

    enum Texts
    {
        Coin_Text,
    }

    public override void Init()
    {
        slots = new List<UI_Inven_Item>();
        
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        
        SlotReset();    // 인벤토리 초기화
        EventSetting(); // EventSystem 설정

        Managers.Input.KeyAction -= OnInventory;
        Managers.Input.KeyAction += OnInventory;

        baseObject = GetObject((int)GameObjects.Inventory);
        baseObject.SetActive(false);

        HideItemTip();
        GetObject((int)GameObjects.CountCheck).SetActive(false);
    }

    // EventSystem 세팅
    void EventSetting()
    {
        // 인벤토리 옮기기 EventSystem 등록
        GetObject((int)GameObjects.Title).BindEvent((PointerEventData eventData)=>{
            baseObject.transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
            Managers.UI.OnUI(this);
        }, Define.UIEvent.Drag);

        // ui를 클릭할 시 order 우선순위
        GetObject((int)GameObjects.Inventory).BindEvent((PointerEventData eventData)=>{
            Managers.UI.OnUI(this);
        }, Define.UIEvent.Click);
    }

    // 슬롯 초기화 
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

    // 인벤토리 On/Off
    void OnInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            MissActive();   // Active가 bool과 엇갈렸는지 확인
            Managers.Game.isInventory = !Managers.Game.isInventory;

            // 활성화/비활성화
            Managers.Game.IsActive(Managers.Game.isInventory, this);

            if (Managers.Game.isInventory)
                baseObject.transform.position = new Vector3(1920, 540, 0);   // 위치 초기화
            else
                GetObject((int)GameObjects.CountCheck).SetActive(false);     // 아이템 개수 설정 UI 비활성화
        }
    }

    // 아이템을 흭득한 경우
    public void AcquireItem(Item _item, int count = 1)
    {
        int tempCount=count;

        if (Item.ItemType.Equipment != _item.itemType)  // 장비가 아닐 때
        {
            for(int i=0; i<slots.Count; i++)            // 인벤토리 슬롯 확인
            {
                if (slots[i].item != null)              // 아이템이 존재 한다면
                {
                    if (slots[i].item == _item)         // 아이템이 같다면
                    {
                        // 인벤토리안에 같은 아이템 찾아서 넣기
                        tempCount = FillSlot(_item, count);
                        if (tempCount == 0)
                            return;
                        else
                            break;
                    }
                }
            }
        }

        // 개수 확인 후 빈 슬롯에 넣기
        for(int i=0; i<slots.Count; i++)
        {
            if (slots[i].item == null)
            {
                if (tempCount > _item.maxCount)
                {
                    slots[i].AddItem(_item, _item.maxCount);
                    // TODO Remove
                    // Managers.Game.playerInfo.ItemRegistration(_item, _item.maxCount, slots[i]);
                    tempCount -= _item.maxCount;
                }
                else
                {
                    slots[i].AddItem(_item, tempCount);
                    // TODO Remove
                    // Managers.Game.playerInfo.ItemRegistration(_item, tempCount, slots[i]);
                    return;
                }
            }
        }
        Debug.Log("인벤토리가 가득찼습니다!");
    }

    // 슬롯의 아이템 개수가 초과되어 다른 슬롯에 넣어야할 때
    public int FillSlot(Item _item, int _count)
    {
        if (_item != null)
        {
            for(int i=0; i<slots.Count; i++)
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item == _item)
                    {
                        // 개수를 더했을 때 최대 개수보다 작다면
                        if (_count+slots[i].itemCount <= _item.maxCount)
                        {
                            // 현재 슬롯에 채우기
                            slots[i].SetCount(_count);
                            return 0;
                        }
                        else    // 크다면
                        {
                            // 남은 개수 빼주기
                            _count -= (_item.maxCount - slots[i].itemCount);

                            // 현재 슬롯에 최대 개수만큼 채우기
                            slots[i].SetCount(_item.maxCount - slots[i].itemCount);
                        }
                    }
                }
            }
        }
        return _count;
    }

    // 아이템 가져오기
    public int ImportItem(Item _item, int count)
    {
        if (_item != null)
        {
            int itemCount = GetItem(_item, count);
            SetItemCount(_item.itemName, itemCount);    // 인벤토리 차감
            return itemCount;
        }

        return 0;
    }

    // 아이템 필요한 만큼 찾기
    public int GetItem(Item _item, int itemCount)
    {
        for(int i=0; i<slots.Count; i++)
        {
            if (slots[i].item != null)
            {
                if (slots[i].item == _item)
                {
                    if (slots[i].itemCount >= itemCount)    // 개수가 있으면 그대로 반환
                    {
                        return itemCount;
                    }
                    else                                    // 없으면 있는 만큼 반환
                        return slots[i].itemCount;
                }         
            }
        }

        return 0;
    }

    // 아이템 사용
    public void UsingItem(UI_BaseSlot baseSlot, UI_Inven_Item invenSlot)
    {
        if (baseSlot == null && invenSlot == null)
            return;
        
        Item _item = null;
        UI_Inven_Item _slot = null;

        if (baseSlot != null)
        {
            _item = baseSlot.item;
            _slot = baseSlot.haveinvenSlot;
        }
        else if (invenSlot != null)
        {
            _item = invenSlot.item;
            _slot = invenSlot;
        }

        if (_item != null)
        {
            if (_item.itemType == Item.ItemType.Used)
            {
                Managers.Game.itemDatabase.UseItem(_item);
                _slot.SetCount(-1);
            }            
        }
        else
            Debug.Log("사용 가능한 아이템이 없습니다.");
    }

    // 아이템 개수 추가 (or 감소)
    public void SetItemCount(string itemName, int itemCount)
    {
        for(int i=0; i<slots.Count; i++)
        {
            if (slots[i].item != null)
            {
                if (slots[i].item.itemName == itemName)
                {
                    slots[i].SetCount(itemCount);
                    return;
                }         
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

    // Active가 bool과 엇갈렸는지 확인
    void MissActive()
    {
        if (baseObject.activeSelf == true)
            Managers.Game.isInventory = true;
        else
            Managers.Game.isInventory = false;
    }
}
