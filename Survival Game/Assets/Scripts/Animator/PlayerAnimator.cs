using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator anim;
    PlayerController playerObj;

    private Define.WeaponState checkWeapon; // 같은 무기를 또 들려고 하는지 체크
    
    bool hasAttack = true;      // 공격 가능 여부
    bool onAttack = false;      // 공격 여부
    float attackTime;           // 공격 시간
    float animTime = 0.81f;     // 최대 공격 시간
    // float attackDelay = -0.3f;  // 공격 타이밍

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
                    // attackDelay = -0.3f;
                    break;
                case Define.WeaponState.Sword:
                    anim.SetTrigger("OnSword");
                    animTime = 1.2f;
                    // attackDelay = -0.65f;
                    break;
            }

            checkWeapon = _state;
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        playerObj = transform.parent.GetComponent<PlayerController>();
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
                Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
                playerObj.stopMoving = true;
                anim.SetTrigger("OnAttack");
                
                attackTime = 0;
                onAttack = true;
            }
        }
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

    public void ExitAttack()
    {
        anim.ResetTrigger("OnAttack");
    }
}
