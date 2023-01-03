using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_ItemCount : MonoBehaviour
{
    public Slider sliderValue;
    public InputField countText;
    public int itemCount;

    void Update()
    {
        if (gameObject.activeSelf)
            CountUpdate();
    }

    protected virtual void CountUpdate()
    {
        itemCount = Mathf.Clamp(itemCount, 1, ((int)sliderValue.maxValue));
        countText.text = itemCount.ToString();
    }

    public abstract void CheckButton();
    public abstract void CancelButton();

    // 슬라이더 값 설정 버튼
    public virtual void SliderValueChange()
    {
        itemCount = ((int)sliderValue.value);
    }

    // 버튼 클릭 시 아이템 개수 +1
    public virtual void PlusButton()
    {
        sliderValue.value = ++itemCount;
    }

    // 버튼 클릭 시 아이템 개수 -1
    public virtual void MinusButton()
    {
        sliderValue.value = --itemCount;
    }

    // 초기화
    public virtual void Clear()
    {
        sliderValue.value = 1;
        sliderValue.maxValue = 1;
        countText.text = "";

        gameObject.SetActive(false);
    }
}
