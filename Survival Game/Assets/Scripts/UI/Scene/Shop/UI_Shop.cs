using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Shop : UI_Scene
{
    public Define.ShopState shopState = Define.ShopState.Sale;

    private int saleGold;
    public int SaleGold{
        get { return saleGold; }
        set {
            saleGold = value;
            GetText((int)Texts.Gold_Text).text = saleGold.ToString();
        }
    }

    List<UI_BuySlot> buySlots;      // 구매 슬롯
    List<UI_SaleSlot> saleSlots;    // 판매 슬롯

    enum GameObjects
    {
        BackGround,
        Buy_Grid,
        Sale_Grid,
        SaleObj,
        Buy_CountCheck,
        Sale_CountCheck,    // 판매할 아이템 개수 체크
        Sale_Content,       // 판매할 때 아이템이 올려지면 Active 용도
    }

    enum Texts
    {
        Gold_Text,
    }

    public override void Init()
    {
        buySlots = new List<UI_BuySlot>();
        saleSlots = new List<UI_SaleSlot>();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));

        AddEventSystem();
        SlotReset();

        Clear();

        GetObject((int)GameObjects.Sale_CountCheck).SetActive(false);
        GetObject((int)GameObjects.Buy_CountCheck).SetActive(false);

        gameObject.SetActive(false);
    }

    // 클릭 이벤트 관련 코드
    void AddEventSystem()
    {
        // 판매 아이템 받기
        Get<GameObject>((int)GameObjects.Sale_Grid).BindEvent((PointerEventData eventData) => {
            if (UI_DragSlot.instance.dragSlot != null)
            {
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
            buySlots.Add(Managers.UI.MakeSubItem<UI_BuySlot>(parent: buyGrid.transform, name: "Buy_Slot"));
            buySlots[i].buyCount = GetObject((int)GameObjects.Buy_CountCheck).GetComponent<UI_BuyCount>();
            buySlots[i].gameObject.SetActive(false);
        }

        for(int i=0; i<15; i++)
        {
            saleSlots.Add(Managers.UI.MakeSubItem<UI_SaleSlot>(parent: saleGrid.transform, name: "Sale_Slot"));
            saleSlots[i].gameObject.SetActive(false);
        }
    }

    // 상점 이용
    public void UseShop()
    {
        if (shopState == Define.ShopState.Buy)
        {
            GetObject((int)GameObjects.Buy_Grid).SetActive(true);
            GetObject((int)GameObjects.SaleObj).SetActive(false);
        }
        if (shopState == Define.ShopState.Sale)
        {
            GetObject((int)GameObjects.SaleObj).SetActive(true);
            GetObject((int)GameObjects.Buy_Grid).SetActive(false);
        }
    }
    
    // 구매창 버튼
    public void BuyButton()
    {
        shopState = Define.ShopState.Buy;
        UseShop();
    }

    // 판매창 버튼
    public void SaleButton()
    {
        shopState = Define.ShopState.Sale;
        UseShop();
    }

    // 구매 슬롯 세팅
    public void BuySetting(Item[] _items)
    {
        // npc가 들고있던 판매아이템 생성
        for(int i=0; i<_items.Length; i++)
        {
            buySlots[i].BuySlotSetting(_items[i]);
        }
    }

    // 판매 아이템 등록
    void SaleConnection()
    {
        for(int i=0; i<saleSlots.Count; i++)
        {
            if (saleSlots[i].item == null)
            {
                if (UI_DragSlot.instance.dragSlot.itemCount > 1)
                {
                    // 개수가 1개 이상일 시 개수 선택창 On
                    GetObject((int)GameObjects.Sale_CountCheck).SetActive(true);
                    UI_SaleCount _go = GetObject((int)GameObjects.Sale_CountCheck).GetComponent<UI_SaleCount>();
                    _go.SetSlot(UI_DragSlot.instance.dragSlot, saleSlots[i]);
                }
                else
                {
                    // 개수가 1개라면 바로 등록
                    saleSlots[i].SaleSetting(UI_DragSlot.instance.dragSlot);
                } 

                UI_DragSlot.instance.SetColor(0);
                UI_DragSlot.instance.dragSlot = null;
                return;
            }
        }
    }

    // 판매하기 (Button)
    public void Selling()
    {
        for(int i=0; i<saleSlots.Count; i++)
        {
            if (saleSlots[i].item != null)
            {
                // 인벤토리 개수 차감
                saleSlots[i].invenSlot.SetCount(-saleSlots[i].ItemCount);

                // 골드 지급
                Managers.Game.playerStat.Gold += saleSlots[i].gold;

                // 들고 있는 아이템체크
                Managers.Game._player.GetComponent<ActionController>().TakeUpSlot();

                // 초기화
                saleSlots[i].Clear();
            }
            else
                break;
        }
        SaleGold = 0;
    }

    public void Clear()
    {
        shopState = Define.ShopState.Buy;
        SaleGold = 0;
        
        GetObject((int)GameObjects.Sale_CountCheck).GetComponent<UI_SaleCount>().Clear();
        GetObject((int)GameObjects.Sale_CountCheck).SetActive(false);
    }
}
