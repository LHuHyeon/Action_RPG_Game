using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    [SerializeField]
    float _scanRange = 7f;     // 주변 탐색 거리

    [SerializeField]
    float _attackRange = 2f;    // 공격 거리

    [SerializeField]
    GameObject _lockTarget;

    [SerializeField]
    ItemPickUp[] dropItem;
    [SerializeField]
    int randomNumber=2;

    float distance;     // 타겟과 나의 거리
    bool stopMoving = false;    // 움직임 멈추기

    Vector3 _destPos;         // 도착 좌표
    
    Stat _stat;
    Animator anim;
    NavMeshAgent nav;
    GameObject hpBarUI;

    public override Define.State State
    {
        get { return _state; }
        set {
            _state = value;

            switch (_state){
                case Define.State.Moving:
                    anim.SetTrigger("OnWalk");
                    break;
                case Define.State.Idle:
                    anim.SetTrigger("OnIdle");
                    break;
                case Define.State.Skill:
                    anim.SetTrigger("OnAttack");
                    break;
                case Define.State.Die:
                    break;
            }
        }
    }

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Monster;

        _stat = GetComponent<Stat>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        hpBarUI = Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform).gameObject;
    }

    // 주변 플레이어 탐색
    protected override void UpdateIdle()
    {
        GameObject player = Managers.Game._player.gameObject;

        if (player.isValid()){
            distance = TargetDistance(player);
            if (distance <= _scanRange)
            {
                _lockTarget = player;
                State = Define.State.Moving;
                hpBarUI.SetActive(true);
                return;
            }
            else
            {
                State = Define.State.Idle;
                hpBarUI.SetActive(false);
            }
        }
    }

    protected override void UpdateMoving()
    {
        if (stopMoving)
        {
            nav.SetDestination(transform.position);
            return;
        }

        // 타겟(플레이어)이 존재하면 두 사이 거리가 _attackRange보다 작거나같을때 멈추고 스킬 시전(공격)
        if (_lockTarget != null){
            distance = TargetDistance(_lockTarget);
            if (distance <= _attackRange){
                nav.SetDestination(transform.position); // 타겟을 나로 지정하면 멈춘다.
                State = Define.State.Skill;
                return;
            }
        }

        // 도착 위치 벡터에서 플레이어 위치 벡터를 뺀다.
        Vector3 dir = _destPos - transform.position;
        
        // Vector3.magnitude = 벡터값의 길이
        if (dir.magnitude >= _scanRange)
        {
            State = Define.State.Idle;
            nav.SetDestination(transform.position);
        }
        else
        {
            nav.speed = _stat.MoveSpeed;
            nav.SetDestination(_destPos);   // 타겟에 접근
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);
        }
    }

    // 플레이어 공격
    protected override void UpdateSkill()
    {
        // 스킬 사용 중에 타겟 바라보기
        if (_lockTarget != null){
            Vector3 dir = _lockTarget.transform.position - transform.position;
            dir.y = 0f;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = quat;
        }
    }

    // 공격 애니메이션 event
    void OnHitEvent()
    {
        if (_lockTarget != null){
            // 공격 거리 안에 있으면 상태 Skill로 변경
            distance = TargetDistance(_lockTarget);
            if (distance <= _attackRange)
                State = Define.State.Skill;
            else
                State = Define.State.Moving;

            Stat targetStat = _lockTarget.GetComponent<Stat>();

            // 공격 중 공격 거리에서 안에 있을 시
            distance = TargetDistance(_lockTarget);
            if (distance <= _attackRange)
                targetStat.OnAttacked(_stat);

            // 상대 체력 확인
            if (targetStat.Hp > 0){
                distance = TargetDistance(_lockTarget);
                if (distance <= _attackRange)
                    State = Define.State.Skill;
                else
                    State = Define.State.Moving;
            }
            else
                State = Define.State.Idle;
        }
        else
            State = Define.State.Idle;
    }

    // 타겟과 나의 거리
    float TargetDistance(GameObject target)
    {
        _destPos = target.transform.position;
        _destPos.y = 0;
        return (_destPos - transform.position).magnitude;
    }

    // 죽을 시 아이템 떨어트리기
    public void DeadDropItem()
    {
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
        anim.SetTrigger("OnHit");
        _stat.OnAttacked(attacker, addDamage, isStat);  // 스탯에 영향 주기
        StartCoroutine(DelayHit());                     // 피격 받을 시 딜레이 후 피격 가능

        StopCoroutine(PushedBack());
        StartCoroutine(PushedBack());   // 뒤로 밀리는 코루틴
    }

    // 뒤로 밀리기
    IEnumerator PushedBack()
    {   
        Vector3 force = -((Managers.Game._player.transform.position - transform.position).normalized);
        GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

        yield return new WaitForSeconds(0.4f);

        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    // 공격 딜레이
    IEnumerator DelayHit()
    {
        stopMoving = true;
        yield return new WaitForSeconds(0.7f);
        stopMoving = false;

        State = Define.State.Moving;
    }
}
