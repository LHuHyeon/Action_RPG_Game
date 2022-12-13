using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // [경로] 총알 몸통 : Bullet_Body, 발사되는 총알 : Bullet_Shot
    string prefabPath = "WeaponObj/Bullet/";

    public Transform firePos;       // 발사 위치
    public float fireSpeed;         // 발사 속도
    public float accuracy = 0.03f;  // 총의 기본 반동

    public int damage;              // 총 데미지
    public int currentBulletCount;  // 현재 총알 개수
    public int maxBulletCount;      // 최대 총알 개수

    public bool isReload;           // 장전 중인지

    void Start()
    {
        Managers.Input.MouseAction -= Shot;
        Managers.Input.MouseAction += Shot;
    }

    // 총알 발사
    public void Shot(Define.MouseEvent evt)
    {
        if (evt == Define.MouseEvent.LeftDown)
        {
            if (Managers.Weapon.weaponState == Define.WeaponState.Gun)
            {
                CrossHair crossHair = Managers.Weapon.crossHair;
                Vector3 fireAccuracy = new Vector3
                                    (Random.Range(-crossHair.GetAccuracy() - accuracy, -crossHair.GetAccuracy() + accuracy),
                                     Random.Range(-crossHair.GetAccuracy() - accuracy, -crossHair.GetAccuracy() + accuracy),
                                     0);

                GameObject bullet = Managers.Resource.Instantiate(prefabPath+"Bullet_Shot");
                bullet.transform.position = firePos.position;
                bullet.transform.localRotation = Quaternion.LookRotation(firePos.forward + fireAccuracy);

                bullet.GetComponent<Bullet>().fireSpeed = fireSpeed;
                bullet.GetComponent<Bullet>().damage = damage;

                Managers.Weapon.crossHair.FireAnim();
            }
        }
    }
}
