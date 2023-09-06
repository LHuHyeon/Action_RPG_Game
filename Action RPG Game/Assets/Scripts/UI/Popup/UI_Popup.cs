using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 팝업 창 UI 들의 부모
public class UI_Popup : UI_Base
{
    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, true);
    }
    
    public virtual void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
