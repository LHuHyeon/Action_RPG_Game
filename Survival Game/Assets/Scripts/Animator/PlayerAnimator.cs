using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public float maxAnimTime;       // 최대 공격 애니메이션 시간 
    public float animTime = 0.9f;   // 애니메이션 공격 시간

    Animator anim;
    PlayerController playerObj;
    ActionController playerAction;

    private Define.WeaponState checkWeapon; // 같은 무기를 또 들려고 하는지 체크
    
    bool onAttack = false;      // 공격 여부
    float attackTime;           // 공격 시간

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
        playerObj = GetComponent<PlayerController>();
        playerAction = GetComponent<ActionController>();
    }
    
    void Update()
    {
        // 공격 중인지 체크
        if (onAttack)
        {
            attackTime += Time.deltaTime;

            // 연속 공격 가능 시간이 지날 시
            if (attackTime > animTime)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
                    anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= maxAnimTime)
                {
                    // 공격 중단
                    anim.ResetTrigger("OnAttack");

                    onAttack = false;
                    playerObj.stopMoving = false;
                }
                Managers.Weapon.EnabledEffect(false);
            }
        }
    }

    // 구르기가 끝났을 때 (Event)
    public void ExitDiveRoll()
    {
        if (Managers.Weapon.weaponState == Define.WeaponState.Gun)
            Managers.Weapon.crossHair.DiveRollAnim(false);

        anim.SetBool("HasRoll", false);

        if (_state == Define.WeaponState.Hand)
            anim.SetTrigger("OnRollHand");
        else if (_state == Define.WeaponState.Sword)
            anim.SetTrigger("OnRollSword");
        else if (_state == Define.WeaponState.Gun)
            anim.SetTrigger("OnRollGun");
        
        Managers.Game.isDiveRoll = false;
        playerObj.GetComponent<PlayerStat>().AddSpeed = 0;
    }

    // 트리거가 계속 켜지는 것을 방지 (Event)
    public void ExitDiveTrigger()
    {
        anim.ResetTrigger("OnRollHand");
        anim.ResetTrigger("OnRollSword");
        anim.ResetTrigger("OnRollGun");
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
        // 0.5f ~ maxAnimTime 사이에 공격키를 누를 시 공격 가능
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= maxAnimTime &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f && onAttack)
        {
            onAttack = false;
        }

        // 공격!
        if (!onAttack)
        {
            anim.SetTrigger("OnAttack");    // 공격 애니메이션
            playerObj.stopMoving = true;    // 멈추기

            onAttack = true;

            attackTime = 0;

            Managers.Weapon.EnabledEffect(true);
        }
    }

    // 총 발사 애니메이션
    public void OnShot()
    {
        anim.SetTrigger("OnShot");
    }

    // 무기 체인지 애니메이션 이벤트 (Event)
    public void OnChangeEvent()
    {
        onAttack = false;
        playerObj.stopMoving = false;
        
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
        Managers.Weapon.attackCollistion.SetActive(true);
    }
}
