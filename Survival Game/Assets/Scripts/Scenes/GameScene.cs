using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        SceneType = Define.Scene.Game;

        Managers.Game.baseInventory = Managers.UI.ShowSceneUI<UI_Inven>();
        Managers.Game.playerInfo = Managers.UI.ShowSceneUI<UI_PlayerInfo>("UI_Info");
        Managers.Game.itemDatabase = GameObject.Find("ItemEffectDatabase").GetComponent<ItemEffectDatabase>();

        Managers.UI.ShowSceneUI<UI_Talk>();

        // Managers.Game.Spawn(Define.WorldObject.Monster, "Zombie1");
    }

    public override void Clear()
    {
    }
}
