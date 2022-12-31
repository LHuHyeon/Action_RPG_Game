using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SaleCount : MonoBehaviour
{
    // TODO : 인벤토리 차감 x, 판매할 인벤 슬롯의 색을 회색으로 변경한다.
    // TODO : 회색으로 판매 준비된 슬롯은 인벤 이동 금지
    // TODO : 판매 등록되면 판매할 인벤 슬롯 위치를 기억하기
    // TODO : 판매되면 설정된 개수만큼 아이템을 인벤에서 차감 후 골드 지급
    // TODO : 취소할 시 회색을 원래 색으로 변경 후 초기화

    public UI_Inven_Item invenSlot; // 판매 아이템을 가진 슬롯
    public Slider sliderValue;
    public InputField countText;
    public Text goldText;
    public int itemCount;
    public int goldCount;

    void Update()
    {
        if (invenSlot != null)
        {
            itemCount = Mathf.Clamp(itemCount, 1, ((int)sliderValue.maxValue));
            countText.text = itemCount.ToString();
            goldCount = invenSlot.item.itemCoin * itemCount;
            goldText.text = goldCount.ToString();
        }
    }

    // 판매할 아이템 설정
    public void SetSlot(UI_Inven_Item _slot, int _itemSlotCount)
    {
        invenSlot = _slot;
        sliderValue.maxValue = _itemSlotCount;
        itemCount = 1;
    }

    // 슬라이더 값 설정 버튼
    public void SliderValueChange()
    {
        itemCount = ((int)sliderValue.value);
    }

    // 확인 버튼
    public void CheckButton()
    {
        // 아이템 개수가 최대 개수보다 작을 때
        if (itemCount < invenSlot.itemCount)
            invenSlot.SetCount(-itemCount);
        else
            invenSlot.ClearSlot();  // 슬롯 초기화

        CancelButton();             // 종료
    }

    // 취소 버튼
    public void CancelButton()
    {
        invenSlot = null;
        sliderValue.value = 1;
        sliderValue.maxValue = 1;
        countText.text = "";
        goldText.text = "";
        gameObject.SetActive(false);
    }

    // 버튼 클릭 시 아이템 개수 +1
    public void PlusButton()
    {
        sliderValue.value = ++itemCount;
    }

    // 버튼 클릭 시 아이템 개수 -1
    public void MinusButton()
    {
        sliderValue.value = --itemCount;
    }
}
