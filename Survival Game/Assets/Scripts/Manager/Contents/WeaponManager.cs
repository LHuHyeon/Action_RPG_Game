using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager
{
    // 현재 들고있는 무기를 여기서 확인
    public GameObject weaponActive;        // 현재 무기 오브젝트 (활성화/비활성화 용도)
    public Item currentWeapon;             // 현재 무기 아이템
    public GameObject attackCollistion;    // 공격 시 충돌처리 해줄 객체
    public TrailRenderer weaponEffect;     // 무기 이팩트

    // 무기 오브젝트 장착
    public Define.WeaponState EquipWeapon(Item _item)
    {
        List<GameObject> weaponList = Managers.Game._player.GetComponent<PlayerController>().weaponList;
        int count = weaponList.Count;

        // 모든 무기 비활성화 ( 무기 교체 시 다시 활성화됨. )
        if (currentWeapon != _item)
        {
            for(int i=0; i<count; i++)
                weaponList[i].SetActive(false);
        }

        currentWeapon = _item;
        
        // 무기 개수가 많지 않으므로 루프문 사용
        for(int i=0; i<count; i++)
        {
            if (_item.itemName == weaponList[i].name)
            {
                weaponActive = weaponList[i];
                
                if (weaponActive.CompareTag("Sword"))
                {
                    weaponEffect = weaponActive.GetComponentInChildren<TrailRenderer>();
                    return Define.WeaponState.Sword;
                }
                else if (weaponActive.CompareTag("Gun"))
                {
                    return Define.WeaponState.Gun;
                }
            }
        }

        return Define.WeaponState.Hand;
    }

    // 무기 이팩트 
    public void EnabledEffect(bool has)
    {
        if (weaponActive != null)
        {
            if (weaponActive.CompareTag("Sword"))
                weaponEffect.enabled = has;  
        }
    }
}
