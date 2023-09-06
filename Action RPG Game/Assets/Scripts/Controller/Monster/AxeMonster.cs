using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeMonster : Monster
{
    [SerializeField]
    int attackCount=3;        // 공격 패턴 개수

    protected override void UpdateAttack()
    {
        Vector3 dir = lockTarget.transform.position-transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);
        
        if (isAttack == false)
        {
            int attackRange = Random.Range(0, attackCount);

            switch (attackRange)
            {
                case 0:
                    anim.SetTrigger("OnAttack1");
                    break;
                case 1:
                    anim.SetTrigger("OnAttack2");
                    break;
                case 2:
                    anim.SetTrigger("OnAttack3");
                    break;
            }
            isAttack = true;
        }
    }

    protected override void HitAnim()
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                anim.SetTrigger("OnHit1");
                break;
            case 1:
                anim.SetTrigger("OnHit2");
                break;
        }
    }
}
