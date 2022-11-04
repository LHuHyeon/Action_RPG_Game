using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemEffect
{
    public string itemName;  // 아이템의 이름
    [Tooltip("HP, ATTACK, DEFENSE, SPEED 만 가능합니다.")]
    public string[] part;  // 효과.
    public int[] num;  // 수치.
}

public class ItemEffectDatabase : MonoBehaviour
{
    [SerializeField]
    private ItemEffect[] itemEffects;

    private const string HP = "HP", ATTACK = "ATTACK", DEFENSE = "DEFENSE", SPEED = "SPEED";

    [SerializeField]
    private PlayerStat playerStat;

    // 아이템 사용
    public void UseItem(Item _item)
    {
        if (_item.itemType == Item.ItemType.Equipment)
        {
            // 장비 장착
        }
        if (_item.itemType == Item.ItemType.Used)
        {
            for (int i = 0; i < itemEffects.Length; i++)
            {
                if (itemEffects[i].itemName == _item.itemName)
                {
                    for (int j = 0; j < itemEffects[i].part.Length; j++)
                    {
                        switch(itemEffects[i].part[j])
                        {
                            case HP:
                                playerStat.Hp += itemEffects[i].num[j];
                                break;
                            case ATTACK:    // TODO : ATTACK, DEFENSE, SPEED 는 쿨타임 구현
                                playerStat.Attack += itemEffects[i].num[j];
                                break;
                            case DEFENSE:
                                playerStat.Defense += itemEffects[i].num[j];
                                break;
                            case SPEED:
                                playerStat.MoveSpeed += itemEffects[i].num[j];
                                break;
                            default:
                                Debug.Log("잘못된 Status 부위. HP, ATTACK, DEFENSE, SPEED 만 가능합니다.");
                                break;
                        }
                        Debug.Log(_item.itemName + " 을 사용했습니다.");
                    }
                    return;
                }
            }
            Debug.Log("itemEffectDatabase에 일치하는 itemName이 없습니다.");
        }
    }
}