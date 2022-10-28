using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float maxRadius = 2f;   // 오브젝트 체크 반경

    UI_Inven inventory;

    RaycastHit hit;

    void Start()
    {
        inventory = Managers.UI.ShowSceneUI<UI_Inven>();
    }

    void Update()
    {
        TargetCheck();
    }

    // 주변 오브젝트 체크
    private void TargetCheck()
    {
        // 주변 콜라이더 탐색
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, maxRadius, LayerMask.GetMask("Item"));

        for(int i=0; i<hitCollider.Length; i++)
            Debug.Log($"item 발견!! : {hitCollider[i]}");
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            for(int i=0; i<hitCollider.Length; i++)
            {
                Item item = hitCollider[i].GetComponent<ItemPickUp>().item;
                if (item != null)
                    inventory.AcquireItem(item);
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
