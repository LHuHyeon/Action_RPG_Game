using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Define.PlayerState state = Define.PlayerState.Idle;
    public List<GameObject> weaponList = new List<GameObject>();

    public Transform cameraArm;        // 카메라 회전 중심 객체
    public Transform character;         // 캐릭터

    public bool stopMoving = false;

    bool onAttack = false;

    public float runSpeed = 3;
    float walkSpeed;
    float baseSpeed;

    float attackTime;

    float maxCurrentTime = 2f;      // 스테미나 재생 대기 시간
    float currentTime = 0f;         // 스테미나 재생 가능 시간 체크

    Vector2 moveInput;           // 이동 키 입력 확인
    Animator anim;
    PlayerStat _stat;

    void Start()
    {
        _stat = GetComponent<PlayerStat>();           // 스탯
        anim = character.GetComponent<Animator>();    // 애니메이션
        
        Managers.Game._player = gameObject;
        Managers.Game.playerStat = _stat;

        // weapon 매니저에게 공격범위 지정시키기
        Managers.Weapon.attackCollistion = Util.FindChild(gameObject, "AttackCollistion");

        // 카메라 오브젝트
        cameraArm = Util.FindChild<Transform>(transform.root.gameObject, "CameraArm");
        walkSpeed = _stat.MoveSpeed;
        baseSpeed = walkSpeed;

        anim.SetBool("IsHand", true);
        
        // 키 입력 관련 메소드 등록
        Managers.Input.KeyAction -= () => {
            KeyboradEvent();
        };
        Managers.Input.KeyAction += () => {
            KeyboradEvent();
        };

        // 마우스 입력 관련 메소드 등록
        Managers.Input.MouseAction -= MouseEvent;
        Managers.Input.MouseAction += MouseEvent;
    }

    void Update()
    {
        Stamina();  // 스테미나 회복
        CameraLookAround();

        // 상태에 따라 작동
        switch (state)
        {
            case Define.PlayerState.Idle:
                UpdateIdle();
                break;
            case Define.PlayerState.Moving:
                UpdateMoving();
                break;
            case Define.PlayerState.Attack:
                UpdateAttack();
                break;
            case Define.PlayerState.Guard:
                UpdateGuard();
                break;
            case Define.PlayerState.Die:
                UpdateDie();
                break;
        }
    }

    // 스테미나 재생
    private void Stamina()
    {
        if (_stat.Sp < _stat.MaxSp)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= maxCurrentTime)
            {
                _stat.Sp += 1;
                if (_stat.Sp == _stat.MaxSp)
                {
                    _stat.Sp = _stat.MaxSp;
                }
            }
        }
        if (_stat.Sp < 0)
            _stat.Sp = 0;
    }

    // 스테미나 +/-
    public void SetStamina(int value)
    {
        _stat.Sp += value;
        currentTime = 0f;
    }

    // 구르기 메소드
    private void DiveRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !Managers.Game.isDiveRoll && _stat.Sp >= 30)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("DiveRoll"))
            {
                state = Define.PlayerState.Moving;

                Moving();
                if (state == Define.PlayerState.Idle)
                {
                    DelayDive();
                    return;
                }

                Managers.Game.isDiveRoll = true;
                _stat.AddSpeed = 1f;
                SetStamina(-30);

                anim.SetTrigger("OnDiveRoll");
                Invoke("DelayDive", 1f);

                // 조준점 변화
                if (Managers.Weapon.weaponState == Define.WeaponState.Gun)
                    Managers.Weapon.crossHair.DiveRollAnim(true);
            }
        }
    }

    void DelayDive()
    {
        onAttack = false;

        Managers.Game.isDiveRoll = false;
        _stat.AddSpeed = 0;
    }

    // w, a, s, d 움직임 메소드
    private void Moving()
    {
        // 공격 중이라면 종료
        if (state == Define.PlayerState.Attack)
            return;
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical   = Input.GetAxisRaw("Vertical");

        Running(horizontal, vertical);
        moveInput = new Vector2(horizontal, vertical);  // 키 입력값 받기

        bool isMove = moveInput.magnitude != 0;
        if (isMove)
            state = Define.PlayerState.Moving;
        else
            state = Define.PlayerState.Idle;

        // 총을 들었을 때 움직임에 따른 조준점 변화
        if (Managers.Weapon.weaponState == Define.WeaponState.Gun)
        {
            Managers.Weapon.crossHair.MovingAnim(isMove);
        }
    }

    // 달리기 or 걷기
    void Running(float horizontal, float vertical)
    {
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetFloat("Horizontal", horizontal);
            anim.SetFloat("Vertical", vertical);
            baseSpeed = runSpeed;
        }
        else
        {
            if (horizontal < 0)
                anim.SetFloat("Horizontal", -0.25f);
            else if (horizontal > 0)
                anim.SetFloat("Horizontal", 0.25f);
            else 
                anim.SetFloat("Horizontal", 0);
            
            if (vertical < 0)
                anim.SetFloat("Vertical", -0.25f);
            else if (vertical > 0)
                anim.SetFloat("Vertical", 0.25f);
            else 
                anim.SetFloat("Vertical", 0);

            baseSpeed = walkSpeed;
        }
    }

    // 움직임 멈추기
    public void StopMove()
    {
        state = Define.PlayerState.Idle;
    }

    // 키 입력
    private void KeyboradEvent()
    {
        // 구르기 중인가?
        if (Managers.Game.isDiveRoll)
            return;

        // 캐릭터의 방향은 카메라 기준
        // transform.forward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;

        // 멈추기
        if (stopMoving || TalkManager.instance.isDialouge)
        {
            StopMove();
            return;
        }

        if (state == Define.PlayerState.Moving)
            DiveRoll();

        Moving();
    }

    // 마우스 입력
    private void MouseEvent(Define.MouseEvent evt)
    {
        // 공격
        if (Input.GetMouseButtonDown(0) && !Managers.Game.isDiveRoll)
        {
            // 0.5f ~ maxAnimTime 사이에 공격키를 누를 시 공격 가능
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.91f &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f && onAttack)
            {
                onAttack = false;
            }

            // 공격!
            if (!onAttack)
            {
                anim.SetTrigger("OnAttack");    // 공격 애니메이션
                state = Define.PlayerState.Attack;

                attackTime = 0;
                onAttack = true;
            }
        }

        // 방어
        if (Input.GetMouseButton(1) && !Managers.Game.isDiveRoll)
        {
            anim.SetBool("IsGuard", true);
            state = Define.PlayerState.Guard;
        }
        else
        {
            if (state == Define.PlayerState.Guard)
            {
                anim.SetBool("IsGuard", false);
                state = Define.PlayerState.Idle;
            }
        }
    }

    // 멈춘 상태 메소드
    public void UpdateIdle()
    {
        // 몇초 뒤 멈춘 특정 모션 취하기
    }

    // 움직이는 상태 메소드
    void UpdateMoving()
    {
        Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
        Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

        // 구르기할 땐 구르는 방향 바라보기
        if (Managers.Game.isDiveRoll == false)
            character.forward = lookForward;
        else
            character.forward = moveDir;

        transform.position += moveDir.normalized * Time.deltaTime * _stat.MoveSpeed;
    }

    // 공격 상태 메소드
    public void UpdateAttack()
    {
        // 공격 취소 시간 계산
        attackTime += Time.deltaTime;

        // 연속 공격 가능 시간이 지날 시
        if (attackTime > 0.88f)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.91f)
            {
                // 공격 중단
                anim.ResetTrigger("OnAttack");

                onAttack = false;
                state = Define.PlayerState.Idle;
            }
        }
    }

    // 방어 상태 메소드
    public void UpdateGuard()
    {

    }

    public void UpdateDie()
    {
        
    }

    // TPS형 카메라 조작
    private void CameraLookAround()
    {
        // if (TalkManager.instance.isDialouge == false && Managers.Game.isShop == false && Managers.Game.isUIMode == false)
        //     return;

        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraArm.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
            x = Mathf.Clamp(x, -1f, 70f);
        else
            x = Mathf.Clamp(x, 335f, 361f);

        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }
}
