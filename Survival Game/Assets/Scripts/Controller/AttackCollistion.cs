using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 공격 시 충돌 범위를 활성화 
public class AttackCollistion : MonoBehaviour
{
    int damage = 0;

    void OnEnable()
    {
        StartCoroutine(AutoDisable());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            // 무기가 존재하면
            if (Managers.Weapon.weaponActive != null)
            {
                EqItem eqItem = Managers.Weapon.currentWeapon as EqItem;
                damage = eqItem.GetStat()["ATTACK"];
            }
            else
                damage = 0;

            other.GetComponent<Monster>().TakeDamage(Managers.Game.playerStat, damage);

            Debug.Log($"공격 데미지 : {Managers.Game.playerStat.Attack + damage}");
        }
    }

    // 공격 범위 비활성화 
    IEnumerator AutoDisable()
    {
        yield return new WaitForSeconds(0.1f);

        gameObject.SetActive(false);
    }
}
