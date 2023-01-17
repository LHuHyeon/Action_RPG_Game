using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "New Item/Equipment")]
public class EqItem : Item
{
    public Define.EqType eqType;    // 장비 타입
    public int defense;             // 방어력
    public int damage;              // 공격력
    public Gun gun;                 // 총 일경우 넣어주기
}
