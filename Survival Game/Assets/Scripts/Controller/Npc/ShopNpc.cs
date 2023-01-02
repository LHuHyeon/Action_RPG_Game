using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNpc : NpcController
{
    public Item[] buyItems;         // 구매 아이템 (아이템 가격은 item.coin*1.5)
    
    public UI_Shop shopUI;          // 상점 UI

    public Vector3 invenpos = new Vector3(900, 85, 0);

    protected override void Init()
    {
        
    }

    public override void Interaction()
    {
        OnShop();
    }

    protected override void NPCUpdate()
    {
        if (Managers.Game.isShop)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Clear();
            }
        }
    }

    void OnShop()
    {
        Managers.Game.isShop = true;

        UIActive(true);
        shopUI.UseShop();

        // 인벤토리 위치 설정
        Managers.Game.baseInventory.baseInventory.transform.localPosition = invenpos;
        Cursor.visible = true;
    }

    void UIActive(bool has)
    {
        shopUI.gameObject.SetActive(has);

        Managers.Game.isInventory = has;
        Managers.Game.baseInventory.baseInventory.SetActive(has);
    }

    void Clear()
    {
        Managers.Game.isShop = false;

        shopUI.Clear();
        UIActive(false);

        Cursor.visible = false;
    }
}
