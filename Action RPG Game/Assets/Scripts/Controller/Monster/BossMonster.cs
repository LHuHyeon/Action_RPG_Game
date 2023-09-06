using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : Monster
{
    [SerializeField]
    private GameObject Flames;

    int attackCount=5;  // 공격 패턴 개수
    float attackTime=0;   // 공격 이 안될 때

    protected override void UpdateAttack()
    {
        Vector3 dir = lockTarget.transform.position-transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);

        attackTime += Time.deltaTime;

        if (_stat.Hp < _stat.MaxHp / 2)
        {
            anim.SetTrigger("OnRoar1");
        }

        if (isAttack == false)
        {
            // int attackRange = Random.Range(0, attackCount);
            int attackRange = 3;

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
                case 3:
                    anim.SetTrigger("OnRoar1");
                    break;
                case 4:
                    anim.SetTrigger("OnRoar2");
                    break;
            }
            isAttack = true;
            attackTime = 0;
        }

        if (attackTime > 3f)
        {
            ExitAttack();
            attackTime = 0;
        }
    }

    public void OnFlame()
    {
        Flames.SetActive(true);
        Flames.GetComponent<ParticleSystem>().Play();
        anim.speed = 0.3f;
    }

    public void CloseFlame()
    {
        anim.speed = 1f;
        Flames.GetComponent<ParticleSystem>().Stop();
    }

    public void SpawnMonster()
    {
        
    }

    protected override void HitAnim() {}
}
