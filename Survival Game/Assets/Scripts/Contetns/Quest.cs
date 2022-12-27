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

    public int monsterID;       // 몬스터 번호
    public int maxKillNumber;   // 잡아야할 개수
    public int procKillNumber;  // 잡은 개수

    public int gold;            // 골드 보상
    public int exp;             // 경험치 보상
    public Reward[] itemReward; // 아이템 보상

    public bool isAccept = false;   // 수락 상태
    public bool isClear = false;    // 클리어 상태

    [TextArea]
    public string description;  // 설명
}
