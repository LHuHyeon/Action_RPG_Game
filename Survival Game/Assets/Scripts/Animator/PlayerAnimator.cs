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

            switch(_state)
            {
                case Define.WeaponState.Hand:
                    anim.SetTrigger("OnHand");
                    break;
                case Define.WeaponState.Sword:
                    anim.SetTrigger("OnSword");
                    break;
                case Define.WeaponState.Gun:
                    anim.SetTrigger("OnGun");
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
    
    // 총 발사 애니메이션
    public void OnShot()
    {
        anim.SetTrigger("OnShot");
    }

    // 무기 체인지 애니메이션 이벤트 (Event)
    public void OnChangeEvent()
    {
        // if (State == Define.WeaponState.Hand)
        // {
        //     Managers.Weapon.weaponActive.SetActive(false);     // 무기 비활성화
        //     Managers.Weapon.weaponActive = null;               // 들고 있는 무기 초기화
        // }
        // else if (State == Define.WeaponState.Sword || State == Define.WeaponState.Gun)
        //     Managers.Weapon.weaponActive.SetActive(true);      // 무기 활성화

        StopCoroutine(DelayChange());
        StartCoroutine(DelayChange());
    }

    // 무기 체인지
    IEnumerator DelayChange()
    {
        yield return new WaitForSeconds(0.15f);
        if (State == Define.WeaponState.Hand)
        {
            Managers.Weapon.weaponActive.SetActive(false);     // 무기 비활성화
            Managers.Weapon.weaponActive = null;               // 들고 있는 무기 초기화
        }
        else if (State == Define.WeaponState.Sword || State == Define.WeaponState.Gun)
            Managers.Weapon.weaponActive.SetActive(true);      // 무기 활성화
    }

    // 공격 시 충돌 여부 체크하는 애니메이션 이벤트 (Event)
    public void OnAttackCollistion()
    {
        playerObj.attackCollistion.SetActive(true);
    }
}
