using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollistion : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(AutoDisable());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
            other.GetComponent<MonsterController>().TakeDamage(77);
    }

    IEnumerator AutoDisable()
    {
        yield return new WaitForSeconds(0.1f);

        gameObject.SetActive(false);
    }
}
