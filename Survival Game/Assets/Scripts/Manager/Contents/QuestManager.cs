using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    /*
    1. 모든 퀘스트는 여기서 관리
    2. 보상 지급
    3. 수락, 포기
    4. 퀘스트 창이 사용할 수 있도록
    */

    public static QuestManager instance;

    public UI_Quest questUI;             // 퀘스트 UI
    public List<Quest> procQuests;       // 진행중인 퀘스트

    public Action<Quest> questNpc=null;  // 퀘스트를 가지고 있는 npc 저장

    public bool isQuest=false;           // 퀘스트 진행중 체크
    public bool isQuestList=false;       // 퀘스트 목록 활성화/비활성화 여부

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            procQuests = new List<Quest>();
        }
    }

    // 퀘스트 등록
    public void ConnectionQuest(Quest _quest)
    {
        if (procQuests.Count < 10)
        {
            procQuests.Add(_quest);
            _quest.isAccept = true;
        }
        else
            Debug.Log("퀘스트 목록이 가득찼습니다!");
    }

    // 보상 지급
    public void Reward()
    {
        
    }
    
    // 몬스터 처치 시
    public void KillCheck(int id)
    {
        if (isQuest)
        {
            for(int i=0; i<procQuests.Count; i++)
            {
                // 죽은 id가 퀘스트 몬스터 id와 같다면
                if (id == procQuests[i].monsterID)
                {
                    procQuests[i].procKillNumber++;
                    if (procQuests[i].procKillNumber >= procQuests[i].maxKillNumber)
                    {
                        procQuests[i].procKillNumber = procQuests[i].maxKillNumber;
                        procQuests[i].isClear = true;

                        questNpc.Invoke(procQuests[i]);
                    }
                }
            }
        }
    }
}
