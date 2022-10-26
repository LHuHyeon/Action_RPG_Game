using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator anim;

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

            switch(_state)
            {
                case Define.WeaponState.Hand:
                    anim.SetTrigger("OnHand");
                    break;
                case Define.WeaponState.Sword:
                    anim.SetTrigger("OnSword");
                    break;
                case Define.WeaponState.Total:
                    anim.SetTrigger("OnTotal");
                    break;
            }

            checkWeapon = _state;
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // 블랜드 트리는 모두 같은 Moving 파라메터를 가지고 있음.
    public void OnAnimMoving(float horizontal, float vertical)
    {
        anim.SetFloat("Horizontal", horizontal);
        anim.SetFloat("Vertical", vertical);
    }

    // 공격 애니메이션
    public void OnAttack()
    {
        anim.SetTrigger("OnAttack");
    }

    // 무기 체인지 애니메이션 이벤트
    public void OnChangeEvent()
    {
        if (State == Define.WeaponState.Hand)
            Managers.Weapon.currentWeapon.SetActive(false);
        else if (State == Define.WeaponState.Sword)
            Managers.Weapon.currentWeapon.SetActive(true);
    }

    // 공격 시 충돌 여부 체크하는 애니메이션 이벤트
    public void OnAttackCollistion()
    {
        Managers.Weapon.attackCollistion.SetActive(true);
    }
}
