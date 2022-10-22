using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    // 키 입력 메소드들을 한번에 실행하기 위한 변수
    public Action KeyAction = null;     // 키 대리자
    public Action MouseAction = null;    // 마우스 대리자

    public void OnUpdate()
    {
        // UI 클릭 확인
        // if (EventSystem.current.IsPointerOverGameObject())
        //     return;

        if (KeyAction != null)
        {  
            KeyAction.Invoke();
        }
        if (MouseAction != null)
        {
            MouseAction.Invoke();
        }
    }

    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
    }
}
