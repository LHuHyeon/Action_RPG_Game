using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "New Quest/quest")]
public class Quest : ScriptableObject
{
    [System.Serializable]
    public class Reward
    {
        public Item item;
        public int itemCount;
    }

    public string title;        // 퀘스트 제목
    public int minLevel;        // 최소 레벨

    public int gold;            // 골드 보상
    public int exp;             // 경험치 보상
    public Reward[] itemReward;  // 아이템 보상

    [TextArea]
    public string description;  // 설명
}
