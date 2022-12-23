using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "New Quest/quest")]
public class Quest : ScriptableObject
{
    public string title;        // 퀘스트 제목

    public int gold;            // 골드 보상
    public int exp;             // 경험치 보상
    public ItemPickUp[] items;  // 아이템 보상

    [TextArea]
    public string description;  // 설명
}
