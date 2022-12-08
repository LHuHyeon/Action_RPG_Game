using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // 총알 몸통 : Bullet_Body, 발사되는 총알 : Bullet_Shot
    string prefabPath = "WeaponObj/Bullet/";

    public Transform firePos;       // 발사 위치

    public float fireSpeed;         // 발사 속도

    void Start()
    {
        Managers.Input.MouseAction -= Shot;
        Managers.Input.MouseAction += Shot;
    }

    public void Shot(Define.MouseEvent evt)
    {
        if (evt == Define.MouseEvent.LeftDown)
        {
            GameObject bullet = Managers.Resource.Instantiate(prefabPath+"Bullet_Shot");
            bullet.transform.position = firePos.position;

            bullet.GetComponent<Bullet>().fireSpeed = fireSpeed;
        }
    }
}
