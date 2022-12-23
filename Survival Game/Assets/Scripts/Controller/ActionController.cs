using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    UI_BaseSlot currentSlot;            // 선택 슬롯
    public UI_BaseSlot CurrentSlot{     // 선택 슬롯 프로퍼티
        get { return currentSlot; }
        set{
            if (currentSlot != null)
            {
                currentSlot.currentEffect.SetActive(false);
            }

            currentSlot = value;
            currentSlot.currentEffect.SetActive(true);

            if (currentSlot.item != null)
            {
                // 장비 체크
                if (currentSlot.item.itemType == Item.ItemType.Equipment)
                    playerAnim.State = Managers.Weapon.EquipWeapon(currentSlot.item);
                else
                    playerAnim.State = Managers.Weapon.NoneWeapon();
            }
            else
                playerAnim.State = Managers.Weapon.NoneWeapon();

            // 조준점 활성화 여부
            if (Managers.Weapon.weaponState == Define.WeaponState.Gun)
            {
                Managers.Weapon.crossHair.gameObject.SetActive(true);
                Managers.Weapon.weaponActive.GetComponent<GunController>().SetGun(currentSlot.item.gun);
            }
            else
                Managers.Weapon.crossHair.gameObject.SetActive(false);
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

        Managers.Input.MouseAction -= UsingSlot;
        Managers.Input.MouseAction += UsingSlot;

        playerAnim = GetComponent<PlayerAnimator>();
    }

    // Start 보다 늦게 Start 되는 오브젝트를 위해 딜레이를 준다.
    void DelayInit()
    {
        slots = Managers.Game.playerInfo.GetSlot(); // 슬롯 UI 가져오기
        CurrentSlot = slots[0];     // 현재 선택한 슬롯
    }

    // 현재 슬롯 다시 들기 (더블 체크)
    public void TakeUpSlot()
    {
        CurrentSlot = currentSlot;
    }

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
                    if (TalkManager.instance.talkUI.isDialouge == false)
                        hitCollider[i].GetComponent<NpcController>().Interaction();
                }
            }
        }
    }

    // 슬롯의 아이템 사용
    void UsingSlot(Define.MouseEvent evt)
    {
        if (currentSlot.item != null && evt == Define.MouseEvent.RightDown)
        {
            if (currentSlot.item.itemType == Item.ItemType.Used)
                Managers.Game.baseInventory.UsingItem(currentSlot, null);
        }
    }

    // 슬롯 선택
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
                    
                    TakeUpSlot(); // 현재 슬롯 선택

                    return;
                }
            }
        }
    }
}
