using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator anim;
    PlayerController playerObj;
    ActionController playerAction;

    private Define.WeaponState checkWeapon; // 같은 무기를 또 들려고 하는지 체크
    
    bool hasAttack = true;      // 공격 가능 여부
    bool onAttack = false;      // 공격 여부
    float attackTime;           // 공격 시간
    float animTime = 0.81f;     // 최대 공격 시간

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
                    animTime = 0.81f;
                    break;
                case Define.WeaponState.Sword:
                    anim.SetTrigger("OnSword");
                    animTime = 1.2f;
                    break;
            }

            checkWeapon = _state;
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        playerObj = transform.parent.GetComponent<PlayerController>();
        playerAction = GetComponent<ActionController>();
    }
    
    void Update()
    {
        // 연속 공격
        if (onAttack)
        {
            attackTime += Time.deltaTime;
            if (attackTime >= animTime)
            {
                anim.ResetTrigger("OnAttack");
                StartCoroutine(HasAttack());

                onAttack = false;
                playerObj.stopMoving = false;
            }
        }

        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && !hasAttack)
        {
            onAttack = false;
            playerObj.stopMoving = false;
        }
    }

    // 공격이 끝나면 딜레이 후 공격 가능
    IEnumerator HasAttack()
    {
        hasAttack = false;
        yield return new WaitForSeconds(0.2f);
        hasAttack = true;
    }

    // 구르기가 끝났을 때 (Event)
    public void ExitDiveRoll()
    {
        anim.SetBool("HasRoll", false);

        if (_state == Define.WeaponState.Hand)
            anim.SetTrigger("OnRollHand");
        else if (_state == Define.WeaponState.Sword)
            anim.SetTrigger("OnRollSword");
        
        Managers.Game.isDiveRoll = false;
        playerObj.GetComponent<PlayerStat>().AddSpeed = 0;
    }

    // 트리거가 계속 켜지는 것을 방지 (Event)
    public void ExitDiveTrigger()
    {
        anim.ResetTrigger("OnRollHand");
        anim.ResetTrigger("OnRollSword");
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
        if (hasAttack)
        {
            // 연속 공격을 위한 코드
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
            {
                animTime = anim.GetCurrentAnimatorStateInfo(0).length;
                onAttack = false;
            }

            if (!onAttack)
            {
                playerObj.stopMoving = true;
                anim.SetTrigger("OnAttack");
                
                attackTime = 0;
                onAttack = true;
            }
        }
        // -----------------------------------------------------------------------
        // 1. 마우스 클릭 시 SetTrigger은 한번만 눌러지도록 만들기
        // 2. 으아ㅏ엉렬우렁ㄹ

        if (!onAttack)
        {
            anim.SetTrigger("OnAttack");
            onAttack = true;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
        {
            
        }
    }

    // 무기 체인지 애니메이션 이벤트 (Event)
    public void OnChangeEvent()
    {
        if (State == Define.WeaponState.Hand)
        {
            Managers.Weapon.currentWeapon.SetActive(false);     // 무기 비활성화
            Managers.Weapon.currentWeapon = null;               // 들고 있는 무기 초기화
        }
        else if (State == Define.WeaponState.Sword)
            Managers.Weapon.currentWeapon.SetActive(true);      // 무기 활성화
    }

    // 공격 시 충돌 여부 체크하는 애니메이션 이벤트 (Event)
    public void OnAttackCollistion()
    {
        Managers.Weapon.attackCollistion.SetActive(true);
    }

    // 공격이 끝나면 (Event)
    public void ExitAttack()
    {
        anim.ResetTrigger("OnAttack");
    }
}
