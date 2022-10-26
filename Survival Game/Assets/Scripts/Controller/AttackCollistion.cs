using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 공격 시 충돌 범위를 활성화 
public class AttackCollistion : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(AutoDisable());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Stat playerStat = Managers.Game._player.GetComponent<Stat>();
            other.GetComponent<MonsterController>().TakeDamage(playerStat);
        }
    }

    IEnumerator AutoDisable()
    {
        yield return new WaitForSeconds(0.1f);

        gameObject.SetActive(false);
    }
}
