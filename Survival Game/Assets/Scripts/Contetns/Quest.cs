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

    public int objectID;        // 오브젝트 번호
    public int maxTargetValue;  // 목표 개수
    public int achieveValue;    // 달성한 개수

    public int gold;            // 골드 보상
    public int exp;             // 경험치 보상
    public Reward[] itemReward; // 아이템 보상

    public bool isAccept = false;   // 수락 상태
    public bool isClear = false;    // 클리어 상태

    [TextArea]
    public string achieveDesc;  // 목표 설명

    [TextArea]
    public string description;  // 내용 설명

    // 퀘스트 초기화 
    public void Clear()
    {
        isAccept = false;
        isClear = false;
        achieveValue = 0;
    }
}
