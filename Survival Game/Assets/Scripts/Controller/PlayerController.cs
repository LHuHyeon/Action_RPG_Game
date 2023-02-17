using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Define.PlayerState state = Define.PlayerState.Idle;
    public List<GameObject> weaponList = new List<GameObject>();

    public GameObject attackCollistion;

    public Transform cameraArm;        // 카메라 회전 중심 객체
    public Transform character;        // 캐릭터

    public bool stopMoving = false;
    public bool onAttack = false;

    float attackCloseTime=0;

    float maxCurrentTime = 2f;      // 스테미나 재생 대기 시간
    float currentTime = 0f;         // 스테미나 재생 가능 시간 체크

    public float runSpeed = 1;
    float baseSpeed;

    Vector2 moveInput;           // 이동 키 입력 확인
    Animator anim;
    PlayerStat _stat;

    void Start()
    {
        _stat = GetComponent<PlayerStat>();           // 스탯
        anim = character.GetComponent<Animator>();    // 애니메이션
        
        Managers.Game._player = this;
        Managers.Game.playerStat = _stat;

        baseSpeed = _stat.MoveSpeed;

        // 카메라 오브젝트
        cameraArm = Util.FindChild<Transform>(transform.root.gameObject, "CameraArm");

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

    // 키 입력
    private void KeyboradEvent()
    {
        // 구르기 중이거나 공격 중이라면 종료
        if (Managers.Game.isDiveRoll || state == Define.PlayerState.Guard)
            return;

        // 멈추기
        if (stopMoving || TalkManager.instance.isDialouge)
        {
            StopMove();
            return;
        }

        character.forward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;

        DiveRoll();
        Moving();
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

                // 움직임 체크
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
                Invoke("DelayDive", 0.9f);
            }
        }
    }

    void DelayDive()
    {
        onAttack = false;
        Managers.Game.isDiveRoll = false;

        _stat.AddSpeed = 0;
        attackCloseTime = 0;
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
    }

    // 달리기 or 걷기
    void Running(float horizontal, float vertical)
    {
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetFloat("Horizontal", horizontal);
            anim.SetFloat("Vertical", vertical);

            baseSpeed = _stat.MoveSpeed + runSpeed;
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

            baseSpeed = _stat.MoveSpeed;
        }
    }

    // 움직임 멈추기
    public void StopMove()
    {
        state = Define.PlayerState.Idle;
    }

    // 마우스 입력
    private void MouseEvent(Define.MouseEvent evt)
    {
        if (Managers.Game.isDiveRoll)
            return;

        // 공격
        if (evt == Define.MouseEvent.LeftDown)
            Attack();

        // 방어
        if (evt == Define.MouseEvent.RightPress)
        {
            anim.SetBool("IsGuard", true);
            state = Define.PlayerState.Guard;
        }
        
        // 방어 해제
        if (evt == Define.MouseEvent.RightUp)
        {
            anim.SetBool("IsGuard", false);
            // state = Define.PlayerState.Idle;
        }
    }

    // 공격 진행
    void Attack()
    {
        // 콤보 체크
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

            onAttack = true;
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

        transform.position += moveDir.normalized * Time.deltaTime * baseSpeed;
    }

    // 공격 상태 메소드
    public void UpdateAttack()
    {
        // 약간의 딜레이로 첫 공격이 움직여지므로 딜레이를 주고 시작
        if (attackCloseTime < 1f)
            attackCloseTime += Time.deltaTime;

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") == false &&
            attackCloseTime > 1f)
        {
            onAttack = false;
            attackCloseTime = 0;
            state = Define.PlayerState.Idle;

            Debug.Log("공격 중단");
        }
    }

    // 방어 상태 메소드
    public void UpdateGuard()
    {
        // 방어 이팩트 구하기
        // 데미지 감소 시키기
    }

    public void UpdateDie() {}

    // TPS형 카메라 조작
    private void CameraLookAround()
    {
        if (TalkManager.instance.isDialouge == true || Managers.Game.isShop == true || Managers.Game.isUIMode == true)
            return;

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
