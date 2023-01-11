using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNpc : NpcController
{
    public Item[] buyItems;         // 판매할 아이템
    
    public UI_Shop shopUI;          // 상점 UI

    public Vector3 invenpos = new Vector3(900, 85, 0);  // 인벤 위치

    protected override void Init()
    {
    }

    // 상호작용
    public override void Interaction()
    {
        OnShop();                       // SHOP UI 호출 및 설정
        shopUI.BuySetting(buyItems);    // 구매 슬롯 설정
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
        // 활성화된 모든 UI 비활성화
        Managers.UI.CloseAllUI();
        
        Managers.Game.isShop = true;

        UIActive(true);
        shopUI.UseShop();

        // 인벤토리 위치 설정
        Managers.Game.baseInventory.baseObject.transform.localPosition = invenpos;
    }

    void UIActive(bool has)
    {
        shopUI.gameObject.SetActive(has);

        Managers.Game.isInventory = has;
        Managers.Game.baseInventory.baseObject.SetActive(has);

        Cursor.visible = has;
    }

    void Clear()
    {
        Managers.Game.isShop = false;

        shopUI.Clear();
        UIActive(false);
    }
}
