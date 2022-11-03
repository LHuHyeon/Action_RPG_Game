using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 오브젝트 위에 표실될 이름 클래스
public class UI_NameBar : UI_Base
{
    ItemPickUp itemObj;

    enum Texts
    {
        NameText,
    }

    public override void Init()
    {
        itemObj = transform.parent.GetComponent<ItemPickUp>();
        Bind<Text>(typeof(Texts));
    }

    void OnEnable()
    {
        // Init 보다 먼저 실행되기 때문에 딜레이
        Invoke("SetName", 0.0001f);
    }

    void Update()
    {
        Transform parent = transform.parent;
        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y + 0.07f);
        transform.rotation = Camera.main.transform.rotation;
    }

    void SetName()
    {
        GetText((int)Texts.NameText).text = itemObj.item.itemName + $" ({itemObj.itemCount})";
    }
}
