using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeMonster : Monster
{
    protected override void UpdateAttack()
    {
        if (isAttack == false)
        {
            anim.SetTrigger("OnAttack");
            isAttack = true;
        }
    }
}
