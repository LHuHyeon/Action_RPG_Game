using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeMonster : Monster
{
    public bool isAttack = false;

    protected override void UpdateAttack()
    {
        if (isAttack == false)
        {
            anim.SetTrigger("OnAttack");
            isAttack = true;
        }
    }

    protected override void UpdateDie()
    {

    }

    // Animation Event
    public void ExitAttack()
    {
        isAttack = false;
        state = Define.MonsterState.Ready;
    }
}
