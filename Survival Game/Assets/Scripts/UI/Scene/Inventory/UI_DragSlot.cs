using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DragSlot : MonoBehaviour
{
    public static UI_DragSlot instance;

    public UI_Inven_Item dragSlot;

    // 아이템 이미지
    [SerializeField]
    private Image imageItem;

    void Start()
    {
        instance = this;
    }

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
