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

    protected float distance;     // 타겟과의 사이 거리
    protected float rValue=0;     // 랜덤 값

    protected Animator anim;
    protected NavMeshAgent nav;

    void Start()
    {
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
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
            case Define.MonsterState.Die:
                UpdateDie();
                break;
        }
    }

    protected void UpdateIdle()
    {
        // 주변 탐색
        Collider[] scanPlayer = Physics.OverlapSphere(transform.position, scanRange, 1 << 6);

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

        // 사정거리 안에 있으면
        if (distance <= scanRange)
        {
            anim.SetBool("IsMoving", true);
            nav.SetDestination(lockTarget.transform.position);

            // 공격 범위에 들어오면
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
                rValue = Random.Range(0.3f, 0.7f);
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
    protected abstract void UpdateDie();
    
    // 타겟과 나의 거리
    protected float TargetDistance(Transform _target)
    {
        return (_target.position - transform.position).magnitude;
    }
}
