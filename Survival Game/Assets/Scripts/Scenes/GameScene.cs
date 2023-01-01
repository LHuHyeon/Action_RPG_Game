using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        SceneType = Define.Scene.Game;

        Managers.Game.baseInventory = Managers.UI.ShowSceneUI<UI_Inven>("Inventory/UI_Inven");
        Managers.Game.playerInfo = Managers.UI.ShowSceneUI<UI_PlayerInfo>("Information/UI_Info");
        Managers.Game.itemDatabase = GameObject.Find("ItemEffectDatabase").GetComponent<ItemEffectDatabase>();
        
        Managers.Game.shopObj = Managers.UI.ShowSceneUI<UI_Shop>("Shop/UI_Shop");

        Managers.UI.ShowSceneUI<UI_Talk>("Talk/UI_Talk");
        Managers.UI.ShowSceneUI<UI_Quest>("Quest/UI_Quest");

        // Managers.Game.Spawn(Define.WorldObject.Monster, "Zombie1");
    }

    public override void Clear()
    {
    }
}
