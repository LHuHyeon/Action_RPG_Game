using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 오브젝트 위에 표실될 이름 클래스
public class UI_NameBar : UI_Base
{
    ItemPickUp itemObj;
    Item.ItemType itemType;

    enum RectTransforms
    {
        Background,
    }

    enum Texts
    {
        NameText,
    }

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<RectTransform>(typeof(RectTransforms));

        itemObj = transform.parent.GetComponent<ItemPickUp>();
        Transform parent = transform.parent;
        transform.rotation = Camera.main.transform.rotation;
    
        if (itemObj != null)
        {
            itemType = itemObj.item.itemType;
            transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y + 0.07f);
        }
        else
            transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y + 0.15f);
    }

    void OnEnable()
    {
        // Init 보다 먼저 실행되기 때문에 딜레이
        Invoke("SetName", 0.000001f);
    }

    void Update()
    {
        Transform parent = transform.parent;   

        if (itemObj != null)
        {
            if (itemType == Item.ItemType.Used)
                transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y + 0.07f);
            else if (itemType == Item.ItemType.Equipment)
                transform.position = parent.position + (Vector3.up * 0.5f);
        }
            
        transform.rotation = Camera.main.transform.rotation;
    }

    void SetName()
    {
        Vector2 pos;

        // 아이템일 경우
        if (itemObj != null)
        {
            GetText((int)Texts.NameText).text = itemObj.item.itemName + $" ({itemObj.itemCount})";
            pos = new Vector2(50*GetText((int)Texts.NameText).text.Length, Get<RectTransform>((int)RectTransforms.Background).sizeDelta.y);
        }
        else 
        {
            GetText((int)Texts.NameText).text = transform.root.name;
            pos = new Vector2(100*GetText((int)Texts.NameText).text.Length, Get<RectTransform>((int)RectTransforms.Background).sizeDelta.y);
        }

        Get<RectTransform>((int)RectTransforms.Background).sizeDelta = pos;
    }
}
