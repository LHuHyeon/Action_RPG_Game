using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    UI_BaseSlot currentSlot;
    public UI_BaseSlot CurrentSlot{
        get { return currentSlot; }
        set{
            if (currentSlot != null)
                currentSlot.currentEffect.SetActive(false);

            currentSlot = value;

            currentSlot.currentEffect.SetActive(true);
        }
    }
    
    List<UI_BaseSlot> slots;

    [SerializeField]
    private float maxRadius = 2f;   // 오브젝트 체크 반경

    void Start()
    {
        Invoke("DelayInit", 0.000001f);
    }

    // Start 보다 늦게 Start 되는 오브젝트를 위해 딜레이를 준다.
    void DelayInit()
    {
        slots = Managers.Game.playerInfo.GetSlot();
        CurrentSlot = slots[0];
    }

    void Update()
    {
        TargetCheck();
        SlotKeyInput();
    }

    void SlotKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            CurrentSlot = slots[0];
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            CurrentSlot = slots[1];
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            CurrentSlot = slots[2];
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            CurrentSlot = slots[3];
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            CurrentSlot = slots[4];
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            CurrentSlot = slots[5];
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            CurrentSlot = slots[6];
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            CurrentSlot = slots[7];
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            CurrentSlot = slots[8];
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
}
