using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Scene UI는 꼭 상속 받아야 할 클래스
public class UI_Scene : UI_Base
{
    public GameObject baseObject;   // Canvas 바로 아래 오브젝트 

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, false);
    }
}
