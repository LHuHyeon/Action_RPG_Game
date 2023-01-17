using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_EqSlot : UI_Base
{
    public Define.EqType eqType;  // 장비 타입

    public EqItem item;
    public Image itemImage;     // UI 이미지
    public GameObject baseImage;     // 기본 이미지
    
    public override void Init()
    {
        #region EventSystems Code

        // 아이템에 커서를 클릭 시
        gameObject.BindEvent((PointerEventData)=>
        {
            if (Input.GetMouseButtonUp(1))
            {
                // 인벤으로 옮기기
                Managers.Game.baseInventory.AcquireItem(item);
                ClearSlot();
            }
        }, Define.UIEvent.Click);

        // 아이템이 존재할 시 마우스로 들 수 있다.
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (item != null)
            {
                UI_DragSlot.instance.eqSlot = this;
                UI_DragSlot.instance.DragSetImage(itemImage);

                UI_DragSlot.instance.transform.position = eventData.position;
            }
        }, Define.UIEvent.BeginDrag);

        // 마우스를 드래그할 시 아이템이 이동됨.
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (item != null)
                UI_DragSlot.instance.transform.position = eventData.position;

        }, Define.UIEvent.Drag);

        // 드래그가 끝났을 때
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            // 아이템을 버린 위치가 UI가 아니라면
            if (item != null && !EventSystem.current.IsPointerOverGameObject())
            {
                // 인벤으로 옮기기
                Managers.Game.baseInventory.AcquireItem(item);
                ClearSlot();
            }

            // 들고 있는 임시 아이템 초기화
            UI_DragSlot.instance.SetColor(0);
            UI_DragSlot.instance.eqSlot = null;

        }, Define.UIEvent.EndDrag);

        // 이 슬롯에 마우스 클릭이 끝나면 아이템 받기
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (Managers.Game.isInventory)
            {
                if (UI_DragSlot.instance.dragSlot != null)
                    DropConnectionSlot();
            }

        }, Define.UIEvent.Drop);

        #endregion

        if (eqType == Define.EqType.Weapon)
            Managers.Weapon.eqSlotUI = this;
    }

    // 아이템을 Drop이 아닌 코드로 넣을 때 사용될 메소드
    public void ConnectionSlot(UI_Inven_Item _invenSlot)
    {
        // 사용 아이템만 적용 가능
        if (_invenSlot.item.itemType != Item.ItemType.Equipment)
        {
            Debug.Log("장비 아이템이 아닙니다!");
            return;
        }
            
        // 장비 부위가 같다면 장착
        EqItem _eqitem = _invenSlot.item as EqItem;
        if (eqType == _eqitem.eqType)
        {
            // 장비가 존재하면 바꾸기
            if (item != null)
            {
                Item _tempItem = item;

                AddItem(_eqitem);
                _invenSlot.AddItem(_tempItem);
            }
            else
            {
                AddItem(_eqitem);
                _invenSlot.ClearSlot();
            }
        }
    }

    // 인벤토리에서 메인 슬롯에 등록할 때
    private void DropConnectionSlot()
    {
        ConnectionSlot(UI_DragSlot.instance.dragSlot);
        UI_DragSlot.instance.dragSlot = null;
    }

    // 아이템 등록
    public void AddItem(EqItem _item)
    {
        item = _item;
        itemImage.sprite = item.itemImage;
        SetColor(255);

        baseImage.SetActive(false);

        if (eqType == Define.EqType.Weapon)
            EqInstall();
    }

    // 투명도 설정 (0 ~ 255)
    void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 슬롯 초기화
    public void ClearSlot()
    {
        item = null;
        itemImage.sprite = null;
        SetColor(0);

        baseImage.SetActive(true);

        if (eqType == Define.EqType.Weapon)
            EqInstall();
    }

    // 장착 or 체인지
    public void EqInstall()
    {
        PlayerAnimator playerAnim = Managers.Game._player.GetComponent<PlayerAnimator>();

        if (item != null)
            playerAnim.State = Managers.Weapon.EquipWeapon(item);
        else
            playerAnim.State = Managers.Weapon.NoneWeapon();

        // 조준점 활성화 여부
        if (Managers.Weapon.weaponState == Define.WeaponState.Gun)
        {
            Managers.Weapon.crossHair.gameObject.SetActive(true);
            Managers.Weapon.weaponActive.GetComponent<GunController>().SetGun(item.gun);
        }
        else
            Managers.Weapon.crossHair.gameObject.SetActive(false);
    }
}
