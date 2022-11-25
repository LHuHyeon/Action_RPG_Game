using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    public Transform playerBody { get; private set; }   // 플레이어 몸통
    Transform cameraArm;        // 카메라 회전 중심 객체

    [SerializeField]
    Transform weaponPos;        // 무기 장착 위치

    [SerializeField]
    private GameObject currentWeapon;   // 현재 무기

    Vector2 moveInput;           // 이동 키 입력 확인

    PlayerAnimator playerAnim;   // 플레이어 애니메이션
    PlayerStat _stat;

    public bool stopMoving = false;

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Player;

        playerBody = Util.FindChild<Transform>(gameObject, "Charector");
        cameraArm = Util.FindChild<Transform>(gameObject, "CameraArm");

        playerAnim = playerBody.GetComponent<PlayerAnimator>();
        _stat = GetComponent<PlayerStat>();
        
        // weapon 매니저에게 공격범위 지정시키기 ( TODO : 플레이어 말고 다른데서 주기 )
        Managers.Weapon.attackCollistion = Util.FindChild(gameObject, "AttackCollistion");
        Managers.Weapon.currentWeapon = currentWeapon;
        Managers.Game._player = gameObject;

        // 키 입력 관련 메소드 등록
        Managers.Input.KeyAction -= () => {
            KeyboradEvent();
            SlotChange();
        };
        Managers.Input.KeyAction += () => {
            KeyboradEvent();
            SlotChange();
        };

        // 마우스 입력 관련 메소드 등록
        Managers.Input.MouseAction -= MouseEvent;
        Managers.Input.MouseAction += MouseEvent;
    }

    protected override void UpdateMoving()
    {
        Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
        Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

        playerBody.forward = lookForward;
        transform.position += moveDir.normalized * Time.deltaTime * _stat.MoveSpeed;
    }

    // 무기 체인지
    private void SlotChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerAnim.State = Define.WeaponState.Hand;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerAnim.State = Define.WeaponState.Sword;
        }
    }

    // 키 입력
    private void KeyboradEvent()
    {
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
        if (evt == Define.MouseEvent.LeftDown)
            playerAnim.OnAttack();
    }
}
