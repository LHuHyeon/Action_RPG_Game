using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 아이템을 드래그하여 이동시킬 때 사용될 클래스
public class UI_DragSlot : MonoBehaviour
{
    public static UI_DragSlot instance;

    public UI_Inven_Item dragSlot;  // 인벤 슬롯
    public UI_BaseSlot baseSlot;    // 캐릭터 슬롯

    // 아이템 이미지
    [SerializeField]
    private Image imageItem;

    void Start()
    {
        instance = this;
    }

    // 드래그 할 경우 활성화
    public void DragSetImage(Image _itemImage)
    {
        imageItem.sprite = _itemImage.sprite;
        SetColor(1);
    }

    public void SetColor(float _alpha)
    {
        Color color = imageItem.color;
        color.a = _alpha;
        imageItem.color = color;
    }
}
