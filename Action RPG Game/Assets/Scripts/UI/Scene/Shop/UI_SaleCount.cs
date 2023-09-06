using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SaleCount : UI_ItemCount
{
    public UI_Inven_Item invenSlot; // 판매 아이템을 가진 슬롯
    public UI_SaleSlot saleSlot;    // 판매 아이템을 등록할 상점 슬롯
    public Text goldText;
    public int goldCount;

    // 판매 수량, 골드 업데이트
    protected override void CountUpdate()
    {
        if (invenSlot != null)
        {
            base.CountUpdate();
            
            goldCount = invenSlot.item.itemCoin * itemCount;
            goldText.text = goldCount.ToString();
        }
    }

    // 판매할 아이템 설정
    public void SetSlot(UI_Inven_Item _invenSlot, UI_SaleSlot _saleSlot)
    {
        _invenSlot.IsClick = false;     // 판매 등록된 인벤은 클릭 금지시키기
        invenSlot = _invenSlot;
        saleSlot = _saleSlot;
        sliderValue.maxValue = _invenSlot.itemCount;
    }

    // 확인 버튼
    public override void CheckButton()
    {
        // 판매 등록
        saleSlot.SaleSetting(invenSlot, itemCount);
        Clear();             // 초기화
    }

    // 취소 버튼
    public override void CancelButton()
    {
        invenSlot.IsClick = true;
        Clear();
    }

    // 초기화
    public override void Clear()
    {
        invenSlot = null;
        saleSlot = null;
        goldText.text = "";

        base.Clear();
    }
}
