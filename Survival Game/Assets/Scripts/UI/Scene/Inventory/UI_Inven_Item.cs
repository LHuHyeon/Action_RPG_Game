using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 인벤토리 슬롯
public class UI_Inven_Item : UI_Base
{
    public Item item;           // 인벤이 들고 있는 Item 정보
    public Image itemImage;     // UI 이미지
    public Text itemCountText;  // UI 개수
    public int itemCount;       // 아이템 개수

    // 클릭 가능 여부
    private bool isClick;
    public bool IsClick{
        get { return isClick; }
        set {
            isClick = value;

            if (isClick)
                clickBan.SetActive(false);
            else
                clickBan.SetActive(true);
        }
    }

    public GameObject clickBan;         // 클릭 금지일 경우 회색 슬롯

    public UI_BaseSlot havebaseSlot;    // 메인 슬롯에 있는 슬롯과 연결

    bool isKeyConnection = false;

    public override void Init()
    {
        IsClick = true;

        #region EventSystems Code

        // 커서가 닿을 시 아이템 정보 UI 활성화
        gameObject.BindEvent((PointerEventData)=>
        {
            if (isClick == false)
                return;

            // 아이템 정보 호출
            Managers.Game.baseInventory.ShowItemTip(item);

            // 키 입력으로 메인슬롯과 연결
            if (item != null)
                isKeyConnection = true;
        }, Define.UIEvent.Enter);
        
        // 커서가 때어지면 아이템 정보 UI 비활성화
        gameObject.BindEvent((PointerEventData)=>
        {
            Managers.Game.baseInventory.HideItemTip();
            isKeyConnection = false;
        }, Define.UIEvent.Exit);

        // 아이템에 커서를 클릭 시
        gameObject.BindEvent((PointerEventData)=>
        {
            // 상점 이용중일 땐 아이템 사용 금지
            if (Managers.Game.isShop)
                return;

            // TODO : 다른 scene UI 들도 클릭 시 Sorting Order 수정 시키기
            // 클릭 시 ui 우선순위
            Managers.UI.OnUI(Managers.Game.baseInventory);
                
            if (Input.GetMouseButtonUp(1))
            {
                if (item.itemType == Item.ItemType.Used)
                    Managers.Game.baseInventory.UsingItem(null, this);    // 아이템 사용
                else if (item.itemType == Item.ItemType.Equipment)
                {
                    if (Managers.Weapon.eqSlotUI != null)
                        Managers.Weapon.eqSlotUI.ConnectionSlot(this);
                }
            }
        }, Define.UIEvent.Click);
        
        // 아이템이 존재할 시 마우스로 들 수 있다.
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (isClick == false)
                return;
                
            if (item != null)
            {
                UI_DragSlot.instance.dragSlot = this;
                UI_DragSlot.instance.DragSetImage(itemImage);

                UI_DragSlot.instance.transform.position = eventData.position;

                // 상점일 경우 Drop 받을 Obj 활성화
                if (Managers.Game.isShop)
                    UI_Shop.go_RayDrop.SetActive(true);
            }
        }, Define.UIEvent.BeginDrag);

        // 마우스를 드래그할 시 아이템이 이동됨.
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (isClick == false)
                return;

            if (item != null)
                UI_DragSlot.instance.transform.position = eventData.position;

        }, Define.UIEvent.Drag);

        // 드래그가 끝났을 때
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (isClick == false)
                return;

            // 아이템을 버린 위치가 UI가 아니라면
            if (item != null && !EventSystem.current.IsPointerOverGameObject())
            {
                if (itemCount > 1)  // 버릴 아이템 개수 설정
                {
                    Managers.Game.baseInventory.ItemDump(this, itemCount);
                }
                else                // 1개라면 바로 버리기 
                {
                    GameObject _item = Managers.Resource.Instantiate($"Item/{item.itemType}/{item.itemName}");
                    _item.transform.position = Managers.Game._player.transform.position;
                    ClearSlot();
                }
            }

            // 들고 있는 임시 아이템 초기화
            UI_DragSlot.instance.SetColor(0);
            UI_DragSlot.instance.dragSlot = null;

        }, Define.UIEvent.EndDrag);

        // 이 슬롯에 마우스 클릭이 끝나면 아이템 받기
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (isClick == false || UI_DragSlot.instance.dragSlot == this)
                return;

            if (UI_DragSlot.instance.dragSlot != null)      // 인벤 슬롯을 받을 때
            {
                // 두 슬롯의 아이템이 같은 아이템일 경우 개수 체크
                if (item == UI_DragSlot.instance.dragSlot.item)
                {
                    int addValue = itemCount + UI_DragSlot.instance.dragSlot.itemCount;
                    if (addValue > item.maxCount)
                    {
                        UI_DragSlot.instance.dragSlot.SetCount(-(item.maxCount-itemCount));
                        SetCount(item.maxCount - itemCount);
                    }
                    else
                    {
                        SetCount(UI_DragSlot.instance.dragSlot.itemCount);
                        UI_DragSlot.instance.dragSlot.ClearSlot();  // 들고 있었던 슬롯은 초기화
                    }
                }
                else
                    ChangeSlot();
            }
            else if (UI_DragSlot.instance.baseSlot != null) // 메인 슬롯을 받을 때
                UI_DragSlot.instance.baseSlot.ClearSlot();
            else if (UI_DragSlot.instance.eqSlot != null)   // 장비 슬롯을 받을 때
            {
                if (item == null)
                {
                    AddItem(UI_DragSlot.instance.eqSlot.item);
                    UI_DragSlot.instance.eqSlot.ClearSlot();
                }
            }

        }, Define.UIEvent.Drop);

        #endregion
    }

    void Update()
    {
        // 숫자 키를 눌러 인벤->메인 슬롯으로 바로 이동 시키는 메소드
        if (isKeyConnection)
            KeyConnection();
    }

    // 현재 슬롯을 다른 슬롯과 바꿀 때 사용하는 메소드
    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;
        UI_BaseSlot _tempSlot = havebaseSlot;

        // 드래그된 슬롯을 현재 슬롯에 Add
        UI_Inven_Item dragSlot = UI_DragSlot.instance.dragSlot;
        AddItem(dragSlot.item, dragSlot.itemCount, dragSlot.havebaseSlot);

        // 현재 슬롯 아이템을 드래그된 슬롯에 Add
        if (_tempItem != null)
            UI_DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount, _tempSlot);
        else
            UI_DragSlot.instance.dragSlot.ClearSlot(false);
    }

    // 투명도 설정 (0 ~ 255)
    void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
        itemCountText.color = color;
    }

    // 아이템 등록
    public void AddItem(Item _item, int count = 1, UI_BaseSlot _baseSlot = null)
    {
        item = _item;
        itemImage.sprite = item.itemImage;

        // 메인 슬롯과 연결
        if (_baseSlot != null)
        {
            havebaseSlot = _baseSlot;
            havebaseSlot.haveinvenSlot = this;
        }

        // 장비가 아니라면 개수 설정
        if (Item.ItemType.Equipment != _item.itemType)
        {
            itemCount = count;
            itemCountText.text = itemCount.ToString();
        }
        else
        {
            itemCount = 0;
            itemCountText.text = "";
        }

        // 색 활성화
        SetColor(255);
    }

    // 아이템 개수 업데이트
    public void SetCount(int count = 1)
    {
        itemCount += count;
        itemCountText.text = itemCount.ToString();

        // 메인 슬롯을 들고 있으면 똑같이 수정
        if (havebaseSlot != null)
            havebaseSlot.SetCount(itemCount);

        // 개수가 없다면
        if (itemCount <= 0)
            ClearSlot();
    }

    // 슬롯 초기화
    public void ClearSlot(bool isRemove=true)
    {
        item = null;
        itemImage.sprite = null;
        itemCount = 0;
        itemCountText.text = "0";
        IsClick = true;

        // 메인 슬롯도 초기화
        if (isRemove)
        {
            if (havebaseSlot != null)
            {
                havebaseSlot.ClearSlot();
                havebaseSlot = null;
            }
        }
        
        SetColor(0);
        Managers.Game.baseInventory.HideItemTip();  // 아이템 팁 비활성화
    }

    // TODO : 슬롯 개수 수정
    // 숫자 키(1~9)로 메인슬롯과 연결
    void KeyConnection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Managers.Game.playerInfo.slots[0].ConnectionSlot(this);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            Managers.Game.playerInfo.slots[1].ConnectionSlot(this);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            Managers.Game.playerInfo.slots[2].ConnectionSlot(this);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            Managers.Game.playerInfo.slots[3].ConnectionSlot(this);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            Managers.Game.playerInfo.slots[4].ConnectionSlot(this);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            Managers.Game.playerInfo.slots[5].ConnectionSlot(this);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            Managers.Game.playerInfo.slots[6].ConnectionSlot(this);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            Managers.Game.playerInfo.slots[7].ConnectionSlot(this);
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            Managers.Game.playerInfo.slots[8].ConnectionSlot(this);
    }
}
