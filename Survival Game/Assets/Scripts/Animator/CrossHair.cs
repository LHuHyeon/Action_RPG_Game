using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 조준점 클래스
public class CrossHair : MonoBehaviour
{
    private Animator anim;
    float gunAccuracy;

    void Start()
    {
        anim = GetComponent<Animator>();
        Managers.Weapon.crossHair = this;
        gameObject.SetActive(false);
    }

    // 움직일 때
    public void MovingAnim(bool _flag)
    {
        anim.SetBool("Moving", _flag);
    }

    // 구를 때
    public void DiveRollAnim(bool _flag)
    {
        anim.SetBool("DiveRoll", _flag);
    }

    // 총을 발사할 때
    public void FireAnim()
    {
        if (anim.GetBool("Moving"))
            anim.SetTrigger("Moving_Fire");
        else
            anim.SetTrigger("Idle_Fire");   
    }
    
    // 반동 적용
    public float GetAccuracy()
    {
        if (anim.GetBool("Moving"))             // 움직일 때
            gunAccuracy = 0.005f;
        else if (anim.GetBool("DiveRoll"))      // 구를 때
            gunAccuracy = 0.01f;
        else
            gunAccuracy = 0.001f;                // 멈출 때

        return gunAccuracy;
    }
}
