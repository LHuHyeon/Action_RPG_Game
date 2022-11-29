using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    public List<GameObject> weaponList = new List<GameObject>();

    public Transform playerBody { get; private set; }   // 플레이어 몸통
    Transform cameraArm;        // 카메라 회전 중심 객체

    Vector2 moveInput;           // 이동 키 입력 확인

    PlayerAnimator playerAnim;   // 플레이어 애니메이션
    PlayerStat _stat;

    public bool stopMoving = false;

    float maxCurrentTime = 2f;      // 스테미나 재생 대기 시간
    float currentTime = 0f;         // 스테미나 재생 가능 시간 체크

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Player;

        playerBody = Util.FindChild<Transform>(gameObject, "Charector");    // 캐릭터 오브젝트
        cameraArm = Util.FindChild<Transform>(gameObject, "CameraArm");     // 카메라 오브젝트

        playerAnim = playerBody.GetComponent<PlayerAnimator>();             // 캐릭터 애니메이션
        _stat = GetComponent<PlayerStat>();                                 // 스탯
        
        // weapon 매니저에게 공격범위 지정시키기
        Managers.Weapon.attackCollistion = Util.FindChild(gameObject, "AttackCollistion");
        Managers.Game._player = gameObject;

        // 키 입력 관련 메소드 등록
        Managers.Input.KeyAction -= () => {
            DiveRoll();
            KeyboradEvent();
            // ↓ 키 입력 메소드가 아니지만 업데이트가 필요한 메소드기 때문에 임시 방편으로 나둠
            Stamina();
        };
        Managers.Input.KeyAction += () => {
            DiveRoll();
            KeyboradEvent();
            Stamina();
        };

        // 마우스 입력 관련 메소드 등록
        Managers.Input.MouseAction -= MouseEvent;
        Managers.Input.MouseAction += MouseEvent;
    }

    // 움직임
    protected override void UpdateMoving()
    {
        Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
        Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

        playerBody.forward = lookForward;
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

    // 구르기
    private void DiveRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !Managers.Game.isDiveRoll && _stat.Sp >= 30)
        {
            Managers.Game.isDiveRoll = true;
            playerAnim.GetComponent<Animator>().SetBool("HasRoll", true);
            _stat.AddSpeed = 2f;
            SetStamina(-30);
        }
    }

    // 키 입력
    private void KeyboradEvent()
    {
        if (Managers.Game.isDiveRoll)
            return;

        if (stopMoving)
        {
            State = Define.State.Idle;
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical   = Input.GetAxisRaw("Vertical");

        playerAnim.OnAnimMoving(horizontal, vertical);

        moveInput = new Vector2(horizontal, vertical);  // 키 입력값 받기
        
        bool isMove = moveInput.magnitude != 0;
        if (isMove)
            State = Define.State.Moving;
        else
            State = Define.State.Idle;
    }

    private void MouseEvent(Define.MouseEvent evt)
    {
        if (!Managers.Game.isDiveRoll && evt == Define.MouseEvent.LeftDown && _stat.Sp >= 10)
        {
            playerAnim.OnAttack();
        }
    }
}
