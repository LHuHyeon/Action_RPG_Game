using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 총을 들었을 때 조준점 업데이트
public class CursorController : MonoBehaviour
{
    Texture2D _handIcon;

    // 마우스 커서 상태
    public enum CursorType
    {
        None,
        Hand,
    }
    CursorType _cursorType = CursorType.None;

    void Start()
    {
        _handIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Hand");
        Cursor.visible = false;
    }

    void Update()
    {
        // 커서가 켜지면 아이콘 활성화
        if (Cursor.visible)
        {
            if (_cursorType != CursorType.Hand){
                Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3.1f, 0), CursorMode.Auto);
                _cursorType = CursorType.Hand;
            }
        }
    }
}
