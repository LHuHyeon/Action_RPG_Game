using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    public List<GameObject> weaponList = new List<GameObject>();

    Transform cameraArm;        // 카메라 회전 중심 객체

    Vector2 moveInput;           // 이동 키 입력 확인

    PlayerAnimator playerAnim;   // 플레이어 애니메이션
    PlayerStat _stat;
    Animator anim;

    public bool stopMoving = false;

    float maxCurrentTime = 2f;      // 스테미나 재생 대기 시간
    float currentTime = 0f;         // 스테미나 재생 가능 시간 체크

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Player;

        playerAnim = GetComponent<PlayerAnimator>();  // 캐릭터 애니메이션
        _stat = GetComponent<PlayerStat>();           // 스탯
        anim = GetComponent<Animator>();              // 애니메이션
        
        Managers.Game._player = gameObject;
        Managers.Game.playerStat = _stat;

        // weapon 매니저에게 공격범위 지정시키기
        Managers.Weapon.attackCollistion = Util.FindChild(gameObject, "AttackCollistion");

        // 카메라 오브젝트
        cameraArm = Util.FindChild<Transform>(transform.root.gameObject, "CameraArm");
        
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

    // 계속 호출될 메소드
    protected override void PlayUpdate()
    {
        Stamina();  // 스테미나 회복
    }

    // 움직임
    protected override void UpdateMoving()
    {
        Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
        Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

        transform.position += moveDir.normalized * Time.deltaTime * _stat.MoveSpeed;
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
                Managers.Game.isDiveRoll = true;
                anim.SetBool("HasRoll", true);
                SetStamina(-30);
                _stat.AddSpeed = 2f;

                // 조준점 변화
                if (Managers.Weapon.weaponState == Define.WeaponState.Gun)
                    Managers.Weapon.crossHair.DiveRollAnim(true);
            }
        }
        // 구르는 애니메이션게 갇혔다면
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("DiveRoll"))
        {
            GetComponent<PlayerAnimator>().ExitDiveRoll();
        }
    }

    // w, a, s, d 움직임 메소드
    private void Moving()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical   = Input.GetAxisRaw("Vertical");

        playerAnim.OnAnimMoving(horizontal, vertical);

        moveInput = new Vector2(horizontal, vertical);  // 키 입력값 받기
        
        bool isMove = moveInput.magnitude != 0;
        if (isMove)
            State = Define.State.Moving;
        else
            State = Define.State.Idle;

        // 총을 들었을 때 움직임에 따른 조준점 변화
        if (Managers.Weapon.weaponState == Define.WeaponState.Gun)
        {
            Managers.Weapon.crossHair.MovingAnim(isMove);
        }
    }

    // 움직임 멈추기
    public void StopMove()
    {
        playerAnim.OnAnimMoving(0, 0);
        State = Define.State.Idle;
    }

    // 키 입력
    private void KeyboradEvent()
    {
        // 구르기 중인가?
        if (Managers.Game.isDiveRoll)
            return;

        // 캐릭터의 방향은 카메라 기준
        transform.forward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;

        // 멈추기
        if (stopMoving || TalkManager.instance.isDialouge)
        {
            StopMove();
            return;
        }

        if (State == Define.State.Moving)
            DiveRoll();

        Moving();
    }

    // 마우스 입력
    private void MouseEvent(Define.MouseEvent evt)
    {
        if (!Managers.Game.isDiveRoll && evt == Define.MouseEvent.LeftDown && _stat.Sp >= 10)
        {
            if (playerAnim.State != Define.WeaponState.Gun)
                playerAnim.OnAttack();
        }
    }
}
