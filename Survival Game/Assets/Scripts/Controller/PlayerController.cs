using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    [SerializeField]
    private Transform characterBody;    // Player 오브젝트
    [SerializeField]
    private Transform cameraArm;        // 카메라 회전 중심 객체
    [SerializeField]
    private GameObject currentWeapon;   // 현재 무기

    [SerializeField]
    private float maxRadius = 2f;   // 오브젝트 체크 반경

    [SerializeField]
    private float attackDelay = 0.8f;
    [SerializeField]
    private bool stopMoving = false;

    Vector2 moveInput;           // 이동 키 입력 확인

    PlayerAnimator playerAnim;   // 플레이어 애니메이션
    PlayerStat _stat;

    public override void Init()
    {
        _stat = GetComponent<PlayerStat>();
        playerAnim = characterBody.GetComponent<PlayerAnimator>();
        WorldObjectType = Define.WorldObject.Player;

        // weapon 매니저에게 공격범위 지정시키기
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
        if (stopMoving)
            return;

        Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
        Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

        characterBody.forward = lookForward;
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

    // 주변 오브젝트 체크
    private void TargetCheck()
    {
        int targetMask = LayerMask.GetMask("Block");

        // 주변 콜라이더 탐색
        Collider[] hitCollider = Physics.OverlapSphere(characterBody.position, maxRadius, targetMask);
        
        for(int i=0; i<hitCollider.Length; i++)
        {
            // TODO : 아이템 줍기 and 주변 아이템 이름 활성화
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log(hitCollider[i].name + " 체크!");
                Destroy(hitCollider[i].gameObject);
                break;
            }
        }
    }

    // 커서에 닿는 오브젝트 확인 ( TODO : 인벤토리에 쓸 코드 )
    private Collider CursorTarget(int _mask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, 100f, _mask);

        if (hit.collider == null)
            return null;

        return hit.collider;
    }

    // 키 입력
    private void KeyboradEvent()
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
    }

    private void MouseEvent(Define.MouseEvent evt)
    {
        playerAnim.OnAttack();
        StartCoroutine(AttackDelay());
    }

    IEnumerator AttackDelay()
    {
        stopMoving = true;
        
        yield return new WaitForSeconds(attackDelay);

        stopMoving = false;
    }
}
