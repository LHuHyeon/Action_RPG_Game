using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerInfo : UI_Scene
{
    // 체력, 마나, 1~9 아이템 관리 UI
    List<UI_BaseSlot> slots;

    public List<UI_BaseSlot> GetSlot() { return slots; }
    
    enum GameObjects
    {
        UsingSlot,
    }

    public override void Init()
    {
        slots = new List<UI_BaseSlot>();

        Bind<GameObject>(typeof(GameObjects));
        SlotReset();
    }

    // BaseSlot에 똑같은 아이템이 존재할 시
    public void OverlabCheck(Item _item)
    {
        for(int i=0; i<9; i++)
        {
            if (slots[i].item != null)
            {
                if (slots[i].item.itemName == _item.itemName)
                    slots[i].ClearSlot();
            }
        }
    }

    void SlotReset()
    {
        GameObject parentSlot = Get<GameObject>((int)GameObjects.UsingSlot);

        // 그리드 안에 있는 자식을 모두 삭제
        foreach(Transform child in parentSlot.transform) 
            Managers.Resource.Destroy(child.gameObject);

        // 실제 인벤토리 정보를 참고해서 자식을 다시 채움
        for(int i=0; i<9; i++)
            slots.Add(Managers.UI.MakeSubItem<UI_BaseSlot>(parent: parentSlot.transform, name: "ItemSlot"));
    }
}
