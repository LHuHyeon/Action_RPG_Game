using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 플레이어 정보 UI 클래스
public class UI_PlayerInfo : UI_Scene
{
    public List<UI_BaseSlot> slots;    // BaseSlot(메인 슬롯) 저장
    PlayerStat _stat;
    
    enum GameObjects
    {
        UsingSlot,
        HPBar,
        SPBar,
        EXPBar,
    }

    enum Texts
    {
        HP_Text,
        SP_Text,
        Exp_Text,
    }

    public override void Init()
    {
        slots = new List<UI_BaseSlot>();
        _stat = Managers.Game._player.GetComponent<PlayerStat>();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));
        SlotReset();
    }

    // Update보다 덜 호출하게 만들려고 FixedUpdate 사용
    void FixedUpdate()
    {
        float ratioHP = (float)_stat.Hp / _stat.MaxHp;
        float ratioSP = (float)_stat.Sp / _stat.MaxSp;
        float ratioEXP = (float)_stat.Exp / _stat.totalExp;

        // 체력, 스테미나 게이지
        GetObject((int)GameObjects.HPBar).GetComponent<Slider>().value = ratioHP;
        GetObject((int)GameObjects.SPBar).GetComponent<Slider>().value = ratioSP;

        // 체력, 스테미나 텍스트
        GetText((int)Texts.HP_Text).text = _stat.Hp + " / " + _stat.MaxHp;
        GetText((int)Texts.SP_Text).text = _stat.Sp + " / " + _stat.MaxSp;

        // Slider 값이 NaN이 된다면 값 수정이 불가능하기 때문에 사전에 처리해야 한다.
        if (float.IsNaN(ratioEXP) == false)
        {
            GetObject((int)GameObjects.EXPBar).GetComponent<Slider>().value = ratioEXP;
            GetText((int)Texts.Exp_Text).text = (ratioEXP*100).ToString("F1")+"%";
        }
        else
        {
            GetObject((int)GameObjects.EXPBar).GetComponent<Slider>().value = 0;
            GetText((int)Texts.Exp_Text).text = "0.0%";
        }
    }

    // 메인 슬롯 아이템 등록
    public void ItemRegistration(Item _item, int count = 1, UI_Inven_Item _invenSlot=null)
    {
        for(int i=0; i<slots.Count; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, count, _invenSlot);
                return;
            }
        }
    }

    // 메인 슬롯의 중복된 아이템 초기화
    public void OverlabClear(UI_Inven_Item _invenSlot)
    {
        UI_BaseSlot[] slots = Managers.Game.playerInfo.slots.ToArray();
        for(int i=0; i<slots.Length; i++)
        {
            // 다른 메인 슬롯에 연결된 인벤슬롯이 내꺼와 같다면 그 메인슬롯을 초기화
            if (slots[i].haveinvenSlot == _invenSlot)
            {
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
        {
            slots.Add(Managers.UI.MakeSubItem<UI_BaseSlot>(parent: parentSlot.transform, name: "ItemSlot"));
            slots[i].slotNumber.text = (i+1).ToString();
        }
    }
}
