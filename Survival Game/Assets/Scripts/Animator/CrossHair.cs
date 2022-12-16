using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void MovingAnim(bool _flag)
    {
        anim.SetBool("Moving", _flag);
    }

    public void DiveRollAnim(bool _flag)
    {
        anim.SetBool("DiveRoll", _flag);
    }

    public void FireAnim()
    {
        if (anim.GetBool("Moving"))
            anim.SetTrigger("Moving_Fire");
        else
            anim.SetTrigger("Idle_Fire");   
    }
    
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
