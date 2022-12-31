using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Shop : UI_Scene
{
    List<UI_BuySlot> buySlots;
    List<UI_SaleSlot> saleSlots;

    enum GameObjects
    {
        BackGround,
        Buy_Grid,
        Sale_Grid,
        SaleObj,
        CountCheck,         // 판매할 아이템 개수 체크
        Sale_Content,       // 판매할 때 아이템이 올려지면 Active 용도
    }

    enum Texts
    {

    }

    public override void Init()
    {
        buySlots = new List<UI_BuySlot>();
        saleSlots = new List<UI_SaleSlot>();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));

        AddEventSystem();
        SlotReset();

        GetObject((int)GameObjects.CountCheck).SetActive(false);
    }

    // 클릭 이벤트 관련 코드
    void AddEventSystem()
    {
        // 판매 아이템 받기
        Get<GameObject>((int)GameObjects.Sale_Grid).BindEvent((PointerEventData eventData) => {
            if (UI_DragSlot.instance.dragSlot != null)
            {
                if (UI_DragSlot.instance.dragSlot.itemCount > 1)
                    GetObject((int)GameObjects.CountCheck).SetActive(true);
                else
                    SaleConnection();
            }
        }, Define.UIEvent.Drop);
    }

    // 각 구매, 판매안의 슬롯 초기화
    void SlotReset()
    {
        GameObject buyGrid = Get<GameObject>((int)GameObjects.Buy_Grid);
        GameObject saleGrid = Get<GameObject>((int)GameObjects.Sale_Grid);

        // 그리드 안에 있는 자식을 모두 삭제
        foreach(Transform child in buyGrid.transform) 
            Managers.Resource.Destroy(child.gameObject);

        foreach(Transform child in saleGrid.transform) 
            Managers.Resource.Destroy(child.gameObject);

        // 실제 인벤토리 정보를 참고해서 자식을 다시 채움
        for(int i=0; i<10; i++)
        {
            buySlots.Add(Managers.UI.MakeSubItem<UI_BuySlot>(parent: buyGrid.transform, name: "BuySlot"));
            buySlots[i].gameObject.SetActive(false);
        }

        for(int i=0; i<15; i++)
        {
            saleSlots.Add(Managers.UI.MakeSubItem<UI_SaleSlot>(parent: saleGrid.transform, name: "SaleSlot"));
            saleSlots[i].gameObject.SetActive(false);
        }
    }
    
    public void Buy()
    {

    }

    public void Sale()
    {

    }

    // 판매 아이템 등록
    void SaleConnection()
    {
        for(int i=0; i<saleSlots.Count; i++)
        {
            if (saleSlots[i].item == null)
            {
                UI_Inven_Item _slot = UI_DragSlot.instance.dragSlot;
                saleSlots[i].item = _slot.item;
                saleSlots[i].itemImage = _slot.itemImage;
                saleSlots[i].itemCount.text = _slot.itemCount.ToString();
            }
        }
    }
}
