using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : MonoBehaviour
{
    public Define.MonsterState state = Define.MonsterState.Idle;

    [SerializeField] protected GameObject lockTarget;     // 발견된 타겟
    [SerializeField] protected float scanRange;           // 타겟 감지 거리
    [SerializeField] protected float attackRange;         // 공격 거리

    [SerializeField] ItemPickUp[] dropItem;
    [SerializeField] int randomNumber=2;

    protected float distance;     // 타겟과의 사이 거리
    protected float rValue=0;     // 준비 시간 랜덤 값

    protected bool isAttack = false;

    protected Stat _stat;
    protected Animator anim;
    protected NavMeshAgent nav;
    
    GameObject hpBarUI;

    void Start()
    {
        _stat = GetComponent<Stat>();
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();

        hpBarUI = Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform).gameObject;
    }

    void FixedUpdate()
    {
        // 상태에 따라 작동
        switch (state)
        {
            case Define.MonsterState.Idle:
                UpdateIdle();
                break;
            case Define.MonsterState.Moving:
                UpdateMoving();
                break;
            case Define.MonsterState.Ready:
                UpdateReady();
                break;
            case Define.MonsterState.Attack:
                UpdateAttack();
                break;
            case Define.MonsterState.Hit:
                UpdateHit();
                break;
            case Define.MonsterState.Die:
                UpdateDie();
                break;
        }
    }

    protected void UpdateIdle()
    {
        // 주변 탐색
        Collider[] scanPlayer = Physics.OverlapSphere(transform.position, scanRange, 1 << 9);

        // 찾으면 움직이기
        if (scanPlayer.Length > 0)
        {
            lockTarget = scanPlayer[0].gameObject;
            state = Define.MonsterState.Moving;
        }
    }

    protected void UpdateMoving()
    {
        distance = TargetDistance(lockTarget.transform);

        // 사정거리 안에 있으면 접근하기
        if (distance <= scanRange)
        {
            anim.SetBool("IsMoving", true);
            nav.SetDestination(lockTarget.transform.position);

            // 공격 범위에 들어오면 공격 준비하기
            if (distance <= attackRange)
            {
                nav.SetDestination(transform.position);    
                state = Define.MonsterState.Ready;
            }
        }
        else
        {
            lockTarget = null;
            anim.SetBool("IsMoving", false);
            nav.SetDestination(transform.position);
            state = Define.MonsterState.Idle;
        }
    }

    protected void UpdateReady()
    {
        distance = TargetDistance(lockTarget.transform);

        if (distance <= attackRange)
        {
            if (rValue == 0)
            {
                rValue = Random.Range(0.15f, 0.3f);
                StartCoroutine(AttackReady());
            }
        }
        else
        {
            StopCoroutine(AttackReady());
            state = Define.MonsterState.Moving;
            anim.SetBool("IsReady", false);
        }
    }

    // 공격 대기 코루틴
    IEnumerator AttackReady()
    {
        anim.SetBool("IsReady", true);

        yield return new WaitForSeconds(rValue);

        if (state == Define.MonsterState.Ready)
            state = Define.MonsterState.Attack;

        rValue = 0;
    }

    // 상속 시 구현.
    protected abstract void UpdateAttack();

    // 공격하는 Animation Event
    protected void OnAttackEvent()
    {
        // 공격 시 상대 체력 감소시키기
        distance = TargetDistance(lockTarget.transform);
        if (distance <= attackRange)
        {
            Stat targetStat = lockTarget.GetComponent<Stat>();
            targetStat.OnAttacked(_stat);
        }
    }

    // 공격이 끝나는 Animation Event
    protected void ExitAttack()
    {
        state = Define.MonsterState.Ready;
        isAttack = false;
    }

    // 필요 시 구현.
    protected virtual void UpdateHit() {}
    protected virtual void UpdateDie() {}

    // 죽을 시 아이템 드랍
    public void DeadDropItem()
    {
        // 아이템 떨어트리기
        for(int i=0; i<dropItem.Length; i++)
        {
            int num = randomNumber + Managers.Game.playerStat.Luk;
            dropItem[i].itemCount = Random.Range(0, num);

            if (dropItem[i].itemCount > 0)
            {
                float randomPos = Random.Range(0.2f, 0.4f);
                GameObject _item = Managers.Resource.Instantiate($"Item/{dropItem[i].item.itemType}/{dropItem[i].item.itemName}");
                _item.transform.position = new Vector3(transform.position.x+randomPos, transform.position.y+0.5f, transform.position.z+randomPos);
            }
        }
    }

    // 공격 받았을 때 [매개변수](공격자 스탯, 추가 데미지, 스탯 공격 여부)
    public void TakeDamage(Stat attacker, int addDamage=0, bool isStat=true)
    {
        state = Define.MonsterState.Hit;
        anim.SetTrigger("OnHit");

        // 스탯에 영향 주기
        _stat.OnAttacked(attacker, addDamage, isStat);

        // 피격 받을 시 딜레이 후 피격 가능
        StartCoroutine(DelayHit());

        // 뒤로 밀리는 코루틴
        StopCoroutine(PushedBack());
        StartCoroutine(PushedBack());
    }

    // 뒤로 밀리기
    IEnumerator PushedBack()
    {   
        Vector3 force = -((Managers.Game._player.transform.position - transform.position).normalized);
        GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

        yield return new WaitForSeconds(0.4f);

        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    // 피격 딜레이
    IEnumerator DelayHit()
    {
        yield return new WaitForSeconds(0.7f);

        ExitAttack();
    }
    
    // 타겟과 나의 거리
    protected float TargetDistance(Transform _target)
    {
        return (_target.position - transform.position).magnitude;
    }
}
