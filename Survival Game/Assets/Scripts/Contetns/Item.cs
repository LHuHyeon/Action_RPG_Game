using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        Equipment,      // 장비
        Used,           // 소비
        Ingredient,     // 재료
        ETC,            // 기타
    }

    public string itemName;         // 이름
    public int maxCount=99;         // 한칸에 들고 있을 수 있는 최대 개수
    public ItemType itemType;       // 아이템 타입
    public int itemCoin;            // 코인
    public int damage=0;            // 데미지
    public Sprite itemImage;        // 이미지
    public GameObject itemPrefab;   // 프리팹
    public Gun gun;                 // 총기

    [TextArea]
    public string itemDesc;
}
