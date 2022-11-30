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
                damage = Managers.Weapon.currentWeapon.damage;
            else
                damage = 0;

            Stat playerStat = Managers.Game._player.GetComponent<Stat>();
            other.GetComponent<MonsterController>().TakeDamage(playerStat, damage);

            Debug.Log($"공격 데미지 : {playerStat.Attack + damage}");
        }
    }

    IEnumerator AutoDisable()
    {
        yield return new WaitForSeconds(0.1f);

        gameObject.SetActive(false);
    }
}
