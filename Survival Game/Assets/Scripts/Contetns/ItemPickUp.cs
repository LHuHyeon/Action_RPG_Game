using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public Item item;
    public int itemCount = 1;

    GameObject obj;

    void Start()
    {
        obj = Managers.UI.MakeWorldSpaceUI<UI_NameBar>(transform).gameObject;
    }
    
    void Update()
    {
        // 주변 플레이어 확인 후 이름 활성화
        Vector3 playerPos = Managers.Game._player.GetComponent<PlayerController>().playerBody.position;
        float distance = Vector3.Distance(transform.position, playerPos);
        
        if (distance >= 3f)
            obj.SetActive(false);
        else
            obj.SetActive(true);
    }
}
