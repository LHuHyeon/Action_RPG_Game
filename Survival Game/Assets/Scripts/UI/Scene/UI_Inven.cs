using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inven : UI_Scene
{
    GameObject baseInventory;       // 인벤토리 오브젝트

    List<UI_Inven_Item> slots;      // 인벤토리 슬롯을 담는 리스트

    enum GameObjects
    {
        Inventory,
        GridPanel,
    }

    enum Texts
    {
        Coin_Text,
    }

    public override void Init()
    {
        slots = new List<UI_Inven_Item>();
        
        Bind<Text>(typeof(Texts));
        GetText((int)Texts.Coin_Text).text = "0";
        
        Bind<GameObject>(typeof(GameObjects));
        GameObject gridPanel = Get<GameObject>((int)GameObjects.GridPanel);

        // 그리드 안에 있는 자식을 모두 삭제
        foreach(Transform child in gridPanel.transform) 
            Managers.Resource.Destroy(child.gameObject);

        // 실제 인벤토리 정보를 참고해서 자식을 다시 채움
        for(int i=0; i<15; i++)
            slots.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(parent: gridPanel.transform, name: "Slot"));

        baseInventory = GetObject((int)GameObjects.Inventory);
        baseInventory.SetActive(false);
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
                baseInventory.SetActive(false);
        }
    }

    // 아이템을 흭득한 경우
    public void AcquireItem(Item _item, int count = 1)
    {
        if (Item.ItemType.Equipment != _item.itemType)  // 장비가 아닐 때
        {
            for(int i=0; i<slots.Count; i++)
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
}
