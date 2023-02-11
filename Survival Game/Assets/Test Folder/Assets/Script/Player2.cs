using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    public Define.PlayerState state = Define.PlayerState.Idle;

    public int runSpeed = 3;
    public int walkSpeed = 2;
    float baseSpeed;

    float attackTime;

    public Transform character;   // 플레이어 캐릭터
    public Transform cameraArm;   // 카메라 회전 중심 객체

    bool onAttack = false;
    bool isDive = false;

    Vector2 moveInput;
    Animator anim;

    void Start()
    {
        anim = character.GetComponent<Animator>();
        baseSpeed = walkSpeed;

        anim.SetBool("IsHand", true);
    }

    void Update()
    {
        DiveRoll();             // 구르기
        CameraLookAround();     // 카메라 회전

        CheckMove();            // 움직임 체크
        MouseInput();           // 클릭 체크

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

    // 구르기
    void DiveRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space) && state != Define.PlayerState.Idle)
        {
            if (isDive == false)
            {
                state = Define.PlayerState.Moving;

                CheckMove();
                if (state == Define.PlayerState.Idle)
                {
                    DelayDive();
                    return;
                }

                isDive = true;
                baseSpeed = 3f;

                anim.SetTrigger("OnDiveRoll");
                Invoke("DelayDive", 1f);
            }
        }
    }

    void DelayDive()
    {
        isDive = false;
        onAttack = false;
        baseSpeed = walkSpeed;
    }

    void MouseInput()
    {
        // 공격
        if (Input.GetMouseButtonDown(0) && !isDive)
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
        if (Input.GetMouseButton(1) && !isDive)
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

    // 움직임 확인
    void CheckMove()
    {
        // 공격 중이거나 구를땐 종료
        if (state == Define.PlayerState.Attack || isDive == true)
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

    // 멈춘 상태 메소드
    public void UpdateIdle()
    {
        // 몇초 뒤 멈춘 특정 모션 취하기
    }

    // 움직이는 상태 메소드
    public void UpdateMoving()
    {
        Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
        Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

        // 구르기할 땐 구르는 방향 바라보기
        if (isDive == false)
            character.forward = lookForward;
        else
            character.forward = moveDir;

        transform.position += moveDir.normalized * Time.deltaTime * baseSpeed;
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
                Debug.Log("공격중단");
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
