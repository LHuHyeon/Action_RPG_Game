using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    // 키 입력 메소드들을 한번에 실행하기 위한 변수
    public Action KeyAction = null;     // 키 대리자
    public Action<Define.MouseEvent> MouseAction = null;    // 마우스 대리자

    public void OnUpdate()
    {
        // 대화 중일 경우 
        if (TalkManager.instance.isDialouge)
        {
            Managers.Game._player.GetComponent<PlayerController>().StopMove();
            return;
        }

        if (KeyAction != null)
        {  
            KeyAction.Invoke();
        }

        // UI를 클릭했거나, 인벤토리/퀘스트 목록(이)가 활성화 됐을 경우
        if (EventSystem.current.IsPointerOverGameObject() || Managers.Game.isInventory || QuestManager.instance.isQuestList)
            return;

        if (MouseAction != null)
        {
            if (Input.GetMouseButtonDown(0))
                MouseAction.Invoke(Define.MouseEvent.LeftDown);
            if (Input.GetMouseButtonDown(1))
                MouseAction.Invoke(Define.MouseEvent.RightDown);
        }
    }

    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
    }
}
