using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    public UI_Quest questUI;             // 퀘스트 UI
    public List<Quest> procQuests;       // 진행중인 퀘스트

    public Action<Define.NPCAction> questNpc=null;  // 퀘스트를 가지고 있는 npc 저장

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
    public void Reward(Quest _quest)
    {
        questNpc.Invoke(Define.NPCAction.Reward);
        procQuests.Remove(_quest);
        questUI.QuestClear(_quest);
    }
    
    // 몬스터 처치 시
    public void KillCheck(int id)
    {
        for(int i=0; i<procQuests.Count; i++)
        {
            // 죽은 id가 퀘스트 몬스터 id와 같고 퀘스트 중이라면
            if (id == procQuests[i].objectID && procQuests[i].isAccept)
            {
                procQuests[i].achieveValue++;
                if (procQuests[i].achieveValue >= procQuests[i].maxTargetValue)
                {
                    procQuests[i].achieveValue = procQuests[i].maxTargetValue;
                    procQuests[i].isClear = true;

                    questNpc.Invoke(Define.NPCAction.Notify);
                }
            }
        }
    }

    // UI 퀘스트 셋팅 메소드
    public void QuestUISetting(Text[] texts, GameObject[] objs, bool isactive, Quest _quest=null)
    {
        if (isactive && _quest != null)
        {
            // 아이템 보상 UI 활성화
            for(int i=0; i<_quest.itemReward.Length; i++)
            {
                objs[i].SetActive(true);
                Util.FindChild<Image>(objs[i]).sprite = _quest.itemReward[i].item.itemImage;
                Util.FindChild<Text>(objs[i]).text = _quest.itemReward[i].itemCount.ToString();
            }

            // text 호출
            texts[0].text = _quest.title;
            texts[1].text = _quest.achieveDesc + "\n" + $"{_quest.achieveValue} / {_quest.maxTargetValue}";
            texts[2].text = _quest.description;
            texts[3].text = _quest.gold.ToString();
            texts[4].text = _quest.exp.ToString();
        }
        else
        {
            // 아이템 보상칸 초기화 
            for(int i=0; i<objs.Length; i++)
            {
                Util.FindChild<Image>(objs[i]).sprite = null;
                Util.FindChild<Text>(objs[i]).text = string.Empty;
                objs[i].SetActive(false);
            }

            // text 초기화
            for(int i=0; i<5; i++)
                texts[i].text = string.Empty;
        }
    }
}
