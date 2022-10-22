using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    RaycastHit hit;
    Texture2D _attackIcon;
    Texture2D _handIcon;

    // 마우스 커서 상태
    public enum CursorType
    {
        None,
        Attack,
        Hand,
    }
    CursorType _cursorType = CursorType.None;

    void Start()
    {
        _attackIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Attack");
        _handIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Hand");
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.gameObject.layer == 7){
                if (_cursorType != CursorType.Attack){
                    Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 3.9f, 0), CursorMode.Auto);
                    _cursorType = CursorType.Attack;
                }
            }
            else{
                if (_cursorType != CursorType.Hand){
                    Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3.1f, 0), CursorMode.Auto);
                    _cursorType = CursorType.Hand;
                }
            }
        }
    }
}
