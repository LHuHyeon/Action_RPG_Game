using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator anim;
    PlayerController playerObj;

    private Define.WeaponState checkWeapon; // 같은 무기를 또 들려고 하는지 체크

    private Define.WeaponState _state = Define.WeaponState.Hand;
    public Define.WeaponState State
    {
        get { return _state; }
        set {
            _state = value;

            if (checkWeapon == _state)
                return;

            anim.SetTrigger("OnChange");

            // 이전 상태 비활성화
            switch(checkWeapon)
            {
                case Define.WeaponState.Hand:
                    anim.SetBool("IsHand", false);
                    break;
                case Define.WeaponState.Sword:
                    anim.SetBool("IsSword", false);
                    break;
            }

            // 바뀌는 상태 활성화
            switch(_state)
            {
                case Define.WeaponState.Hand:
                    anim.SetBool("IsHand", true);
                    break;
                case Define.WeaponState.Sword:
                    anim.SetBool("IsSword", true);
                    break;
            }

            checkWeapon = _state;
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        playerObj = transform.root.GetComponent<PlayerController>();
    }

    // 공격 시 충돌 여부 체크하는 애니메이션 이벤트
    public void OnAttackCollistion()
    {
        playerObj.attackCollistion.SetActive(true);
    }

    // 무기 체인지 애니메이션 이벤트
    public void OnChange()
    {
        if (anim.GetBool("IsHand") == true)
        {
            Managers.Weapon.weaponActive.SetActive(false);
            Managers.Weapon.weaponActive = null;
        }
        else if (anim.GetBool("IsSword") == true)
        {
            Managers.Weapon.weaponActive.SetActive(true);
        }
    }

    // 가드가 끝날 때 애니메이션 이벤트
    public void ExitGuard()
    {
        playerObj.state = Define.PlayerState.Idle;
    }
}
