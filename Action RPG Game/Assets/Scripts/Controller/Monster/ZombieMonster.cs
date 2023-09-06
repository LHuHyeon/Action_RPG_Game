using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMonster : Monster
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
