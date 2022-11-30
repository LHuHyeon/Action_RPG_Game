using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerInfo : UI_Scene
{
    // TODO : 체력, 스테미너 관리 예정
    // 1~9 아이템 관리 UI
    List<UI_BaseSlot> slots;    // BaseSlot(메인 슬롯) 저장
    PlayerStat _stat;

    public List<UI_BaseSlot> GetSlot() { return slots; }
    
    enum GameObjects
    {
        UsingSlot,
        HPBar,
        SPBar,
    }

    enum Texts
    {
        HP_Text,
        SP_Text,
    }

    public override void Init()
    {
        slots = new List<UI_BaseSlot>();
        _stat = Managers.Game._player.GetComponent<PlayerStat>();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));
        SlotReset();
    }

    void Update()
    {
        float ratioHP = (float)_stat.Hp / _stat.MaxHp;
        float ratioSP = (float)_stat.Sp / _stat.MaxSp;

        // 체력, 스테미나 게이지
        GetObject((int)GameObjects.HPBar).GetComponent<Slider>().value = ratioHP;
        GetObject((int)GameObjects.SPBar).GetComponent<Slider>().value = ratioSP;

        // 체력, 스테미나 텍스트
        GetText((int)Texts.HP_Text).text = _stat.Hp + " / " + _stat.MaxHp;
        GetText((int)Texts.SP_Text).text = _stat.Sp + " / " + _stat.MaxSp;
    }

    // 메인 슬롯 아이템 등록
    public void ItemRegistration(Item _item, int count = 1)
    {
        for(int i=0; i<slots.Count; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, count);
                return;
            }
        }
    }

    // 메인 슬롯의 아이템 개수 수정
    public void SetItemCount(UI_Inven_Item invenSlot)
    {
        for(int i=0; i<slots.Count; i++)
        {
            if (slots[i].item != null)
            {
                if (slots[i].item.itemName == invenSlot.item.itemName)
                {
                    slots[i].SetCount(invenSlot.itemCount);
                    return;
                }
            }
        }
    }

    // 메인 슬롯의 _item을 모두 제거
    public void ClearSlot(Item _item)
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

    // 1~9의 슬롯 초기화
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
