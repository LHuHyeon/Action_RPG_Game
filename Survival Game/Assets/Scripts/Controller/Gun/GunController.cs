using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    /*
    1. 발사
    3. 장전, 장전 시간
    5. 발사 이팩트
    7. 애니메이션
    9. 맞췄을 때 표시
    */

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

    void Update()
    {
        if (Managers.Weapon.weaponState == Define.WeaponState.Gun)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentGun.currentBulletCount > 0)
                    Shot();
                else
                    Reload();
                
                Debug.Log("총 발사!");
            }
            
            if (Input.GetKeyDown(KeyCode.R))
                Reload();
        }
    }

    // 장전
    public void Reload()
    {
        if (currentGun.currentBulletCount < currentGun.maxBulletCount)
        {
            // 인벤토리 총알 개수 확인
            int bulltCount = Managers.Game.baseInventory.GetItem(currentGun.bullt, currentGun.maxBulletCount);
            if (bulltCount > 0 && !isReload)
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

        // 인벤토리에서 총알 가져오기
        int bulltCount = Managers.Game.baseInventory.ImportItem(currentGun.bullt, currentGun.maxBulletCount-currentGun.currentBulletCount);
        currentGun.currentBulletCount += bulltCount;

        isReload = false;
        anim.SetBool("Reload", false);
    }

    // 총알 발사
    public void Shot()
    {
        if (!isReload)
        {
            CrossHair crossHair = Managers.Weapon.crossHair;
            if (Physics.Raycast(theCam.transform.position, theCam.transform.forward + 
                    new Vector3 (Random.Range(-crossHair.GetAccuracy() - currentGun.accuracy, -crossHair.GetAccuracy() + currentGun.accuracy),
                                Random.Range(-crossHair.GetAccuracy() - currentGun.accuracy, -crossHair.GetAccuracy() + currentGun.accuracy),
                                0), 
                out hitInfo, 
                currentGun.fireRange))
            {
                // GameObject clone = Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                GameObject clone = Managers.Resource.Instantiate(prefabPath+"BulletEffect", hitInfo.transform);
                Managers.Resource.Destroy(clone, 2f);
            }

            currentGun.currentBulletCount--;
            crossHair.FireAnim();
        }
    }
}
