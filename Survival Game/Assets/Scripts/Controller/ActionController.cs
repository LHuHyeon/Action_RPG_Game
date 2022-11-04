using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float maxRadius = 2f;   // 오브젝트 체크 반경

    RaycastHit hit;

    void Update()
    {
        TargetCheck();
    }

    // 주변 오브젝트 체크
    private void TargetCheck()
    {
        // 주변 콜라이더 탐색
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, maxRadius, LayerMask.GetMask("Item"));
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            for(int i=0; i<hitCollider.Length; i++)
            {
                ItemPickUp _item = hitCollider[i].GetComponent<ItemPickUp>();
                if (_item != null)
                {
                    Managers.Game.baseInventory.AcquireItem(_item.item, _item.itemCount);
                    Destroy(hitCollider[i].gameObject);
                    return;
                }
            }
        }
        
    }

    // 커서에 닿는 오브젝트 확인 ( TODO : 인벤토리에 쓸 코드 )
    private Collider CursorTarget(int _mask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, 100f, _mask);

        if (hit.collider == null)
            return null;

        return hit.collider;
    }
}
