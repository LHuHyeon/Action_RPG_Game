using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // [경로] 총알 몸통 : Bullet_Body, 발사되는 총알 : Bullet_Shot
    string prefabPath = "WeaponObj/Bullet/";

    public Gun currentGun;          // 현재 총기
    public Animator anim;           // 애니메이션

    public Transform theCam;            // 발사 위치
    public GameObject hitEffectPrefab;  // 피격 이팩트
    public GameObject fireEffectPrefab; // 발사 이팩트

    public bool isReload = false;          // 장전 중인지
    RaycastHit hitInfo;

    // 총 세팅
    public void SetGun(Gun gun)
    {
        currentGun = gun;
    }

    void Start()
    {
        Managers.Input.KeyAction -= KeyReload;
        Managers.Input.KeyAction += KeyReload;
        Managers.Input.MouseAction -= GunShot;
        Managers.Input.MouseAction += GunShot;
    }

    // 총 발사
    void GunShot(Define.MouseEvent evt)
    {
        if (Managers.Weapon.weaponState == Define.WeaponState.Gun)
        {
            if (evt == Define.MouseEvent.LeftDown)
            {
                if (currentGun.currentBulletCount > 0)
                    Shot();
                else
                    Reload();
            }
        }
    }

    // R키 장전
    void KeyReload()
    {
        if (Managers.Weapon.weaponState == Define.WeaponState.Gun)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
        }
    }

    // 장전
    void Reload()
    {
        if (currentGun.currentBulletCount < currentGun.maxBulletCount)
        {
            // 인벤토리 총알 개수 확인
            int bulletCount = Managers.Game.baseInventory.GetItem(currentGun.bullt, currentGun.maxBulletCount);
            Debug.Log($"장전! : {bulletCount}");
            if (bulletCount > 0 && !isReload)
            {
                StartCoroutine(ReloadTime());
            }
        }
    }    

    // 장전 시간
    IEnumerator ReloadTime()
    {
        isReload = true;
        anim.SetBool("Reload", true);

        yield return new WaitForSeconds(currentGun.reloadTime);

        // 인벤토리에서 총알 가져오기 (아이템, 감소할 개수)
        // 반환 값이 마이너스 값으로 오므로 한번더 - 곱해주기
        int bulltCount = -(Managers.Game.baseInventory.ImportItem(currentGun.bullt, -(currentGun.maxBulletCount-currentGun.currentBulletCount)));
        currentGun.currentBulletCount += bulltCount;

        isReload = false;
        anim.SetBool("Reload", false);
    }

    // 총알 발사
    public void Shot()
    {
        if (!isReload)
        {
            // 총 발사 애니메이션
            anim.GetComponent<PlayerAnimator>().OnShot();

            // 반동 설정
            CrossHair crossHair = Managers.Weapon.crossHair;
            Vector3 direction = (crossHair.transform.position-theCam.transform.position).normalized;
            Debug.DrawRay(theCam.transform.position, direction*2f, Color.red, 3f);
            if (Physics.Raycast(theCam.transform.position, direction + 
                    new Vector3 (Random.Range(-crossHair.GetAccuracy() - currentGun.accuracy, -crossHair.GetAccuracy() + currentGun.accuracy),
                                Random.Range(-crossHair.GetAccuracy() - currentGun.accuracy, -crossHair.GetAccuracy() + currentGun.accuracy),
                                0), 
                out hitInfo, 
                currentGun.fireRange, ((-1) - (1 << 9))))   // (-1) - (1 << 9) LayerMask 대신 비트 연산자를 사용하여 조금이라도 최적화
            {
                Debug.Log("발사!");
                if (hitInfo.collider.CompareTag("Monster"))
                {
                    hitInfo.collider.GetComponent<MonsterController>().TakeDamage(Managers.Game.playerStat, currentGun.damage, false);
                    Debug.Log("명중!");
                }

                // 피격 이팩트 생성
                GameObject clone = Managers.Resource.Instantiate(prefabPath+"BulletEffect");
                clone.transform.position = hitInfo.point+(Vector3.up*0.3f);
                clone.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
                Managers.Resource.Destroy(clone, 2f);
            }

            // 총알 개수 차감 
            currentGun.currentBulletCount--;
            crossHair.FireAnim();
        }
    }
}
