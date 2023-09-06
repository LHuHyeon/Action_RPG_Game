using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 프리팹
public class ItemPickUp : MonoBehaviour
{
    public Item item;
    public int itemCount = 1;

    GameObject obj;

    void Start()
    {
        // 이름 적용
        obj = Managers.UI.MakeWorldSpaceUI<UI_NameBar>(transform).gameObject;
    }
    
    void Update()
    {
        // 주변 플레이어 확인 후 이름 활성화
        Vector3 playerPos = Managers.Game._player.transform.position;
        float distance = Vector3.Distance(transform.position, playerPos);
        
        if (distance >= 2f)
            obj.SetActive(false);
        else
            obj.SetActive(true);
    }
}
