using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager
{
    // 1. 가지고 있는 무기 모드 여기에 저장
    // 2. 현재 들고있는 무기를 여기서 확인
    public GameObject currentWeapon;        // 현재 무기
    public GameObject attackCollistion;     // 공격 시 충돌처리 해줄 객체

    private Dictionary<string, GameObject> swordDic = new Dictionary<string, GameObject>();

    private Define.WeaponState _state = Define.WeaponState.Hand;
    public Define.WeaponState State
    {
        get { return _state; }
        set {
            _state = value;

            switch(_state)
            {
                case Define.WeaponState.Hand:
                    break;
                case Define.WeaponState.Sword:
                    break;
            }
        }
    }

    // 무기 장착
    public void EquipWeapon(Transform pos, ItemPickUp _item)
    {
        
    }
}
