using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float fireSpeed;
    public float destroyTime;

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
}
