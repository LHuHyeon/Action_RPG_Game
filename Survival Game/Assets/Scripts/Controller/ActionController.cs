using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    UI_BaseSlot currentSlot;            // 선택 슬롯
    public UI_BaseSlot CurrentSlot{     // 선택 슬롯 프로퍼티
        get { return currentSlot; }
        set{
            currentSlot = value;

            // 아이템 사용
            if (currentSlot.item != null && !Managers.Game.isInventory)
            {
                if (currentSlot.item.itemType == Item.ItemType.Used)
                    Managers.Game.baseInventory.UsingItem(currentSlot, null);
            }
        }
    }
    
    PlayerAnimator playerAnim;
    List<UI_BaseSlot> slots;

    [SerializeField]
    private float maxRadius = 2f;   // 오브젝트 체크 반경

    void Start()
    {
        Invoke("DelayInit", 0.000001f);

        Managers.Input.KeyAction -= () => {
            Interaction();
            SlotKeyInput();
            TargetCheck();
        };
        Managers.Input.KeyAction += () => {
            Interaction();
            SlotKeyInput();
            TargetCheck();
        };

        playerAnim = GetComponent<PlayerAnimator>();
    }

    // Start 보다 늦게 Start 되는 오브젝트를 위해 딜레이를 준다.
    void DelayInit()
    {
        slots = Managers.Game.playerInfo.slots; // 슬롯 UI 가져오기
    }

    // TODO Remove
    // 현재 슬롯 다시 들기 (더블 체크)
    // public void TakeUpSlot()
    // {
    //     CurrentSlot = currentSlot;
    // }

    // 주변 상호작용
    void Interaction()
    {
        // 11 Layer : NPC
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, maxRadius, (1 << 11));

        if (Input.GetKeyDown(KeyCode.G))
        {
            // NPC와 대화
            if (hitCollider != null)
            {
                for(int i=0; i<hitCollider.Length; i++)
                {
                    Debug.Log($"NPC[{i}] {hitCollider[i].name} 발견!");
                    if (TalkManager.instance.isDialouge == false)
                        hitCollider[i].GetComponent<NpcController>().Interaction();
                }
            }
        }
    }

    // TODO Remove
    // void UsingSlot(Define.MouseEvent evt)
    // {
    //     if (currentSlot.item != null && evt == Define.MouseEvent.RightDown)
    //     {
    //         if (currentSlot.item.itemType == Item.ItemType.Used)
    //             Managers.Game.baseInventory.UsingItem(currentSlot, null);
    //     }
    // }

    // TODO : 슬롯 줄이기
    // 슬롯 사용하기
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
        // 주변 아이템 탐색
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, maxRadius, LayerMask.GetMask("Item"));
        
        // F 키를 누르면 줍기
        if (Input.GetKeyDown(KeyCode.F))
        {
            for(int i=0; i<hitCollider.Length; i++)
            {
                ItemPickUp _item = hitCollider[i].GetComponent<ItemPickUp>();
                if (_item != null)
                {
                    // 인벤에 넣기
                    Managers.Game.baseInventory.AcquireItem(_item.item, _item.itemCount);
                    Destroy(hitCollider[i].gameObject);

                    return;
                }
            }
        }
    }
}
