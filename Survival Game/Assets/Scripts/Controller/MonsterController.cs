using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public void TakeDamage(int damage)
    {
        GetComponent<Animator>().SetTrigger("OnHit");
        Debug.Log("몬스터가 " + damage + "를 입었습니다!");
    }
}
