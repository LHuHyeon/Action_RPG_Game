using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        SceneType = Define.Scene.Game;

        Managers.Item.baseInventory = Managers.UI.ShowSceneUI<UI_Inven>();
    }

    public override void Clear()
    {
    }
}
