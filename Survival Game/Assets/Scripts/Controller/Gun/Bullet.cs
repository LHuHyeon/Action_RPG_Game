using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 총알 클래스
public class Bullet : MonoBehaviour
{
    public float fireSpeed;
    public float destroyTime;
    public int damage;

    void OnEnable()
    {
        Invoke("DelayDestroy", destroyTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * fireSpeed);
    }

    void DelayDestroy()
    {
        Managers.Resource.Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Monster"))
        {
            Stat playerStat = Managers.Game._player.GetComponent<Stat>();
            collisionInfo.gameObject.GetComponent<MonsterController>().TakeDamage(playerStat, damage, false);
            Managers.Resource.Destroy(gameObject);
        }
    }
}
