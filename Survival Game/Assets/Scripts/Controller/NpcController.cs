using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestLevel
{
    public Quest quest;     // 퀘스트
    public int minLevel;    // 최소 레벨
    public Vector2 talkLine;// 대화 라인 (csv)
}

[System.Serializable]
public class TalkState
{
    public Vector2 basicsLine;  // 기본 대화
    public Vector2 questLine;   // 퀘스트 시작 대화
    public Vector2 procLine;    // 퀘스트 진행중인 대화
    public Vector2 acceptLine;  // 퀘스트 수락 대화
    public Vector2 refusalLine; // 퀘스트 거절 대화
    public Vector2 clearLine;   // 퀘스트 성공 대화
    public Vector2 failedLine;  // 퀘스트 실패 대화
}

public class NpcController : MonoBehaviour
{
    public Define.NPCState state = Define.NPCState.None;

    public TalkState[] talkStates;

    public QuestLevel[] quest;  // 퀘스트 목록
    public int nextNumber=0;    // 현재 퀘스트

    public bool isQuest = false;    // 현재 퀘스트 중인지
    
    Vector2 talkLine;
    GameObject obj;

    void Start()
    {
        obj = Managers.UI.MakeWorldSpaceUI<UI_NameBar>(transform).gameObject;
    }
    
    void Update()
    {
        // 주변 플레이어 확인 후 이름 활성화
        Vector3 playerPos = Managers.Game._player.transform.position;
        float distance = Vector3.Distance(transform.position, playerPos);
        
        if (distance >= 2f)
            obj.SetActive(false);
        else
            obj.SetActive(true);
    }

    // 플레이어가 상호작용할 시
    public void Interaction()
    {
        if (state == Define.NPCState.Quest)
        {
            // 다음 퀘스트 레벨 조건이 맞다면
            // if (quest != null)
            // {
            //     if (Managers.Game.playerStat.Level >= quest[nextNumber].minLevel)
            //     {
            //         QuestTalk();
            //     }
            //     else
            //     {
            //         Talk();
            //     }
            // }
            // else
            // {
            //     Talk();
            // }
            Talk();
        }
    }

    // 퀘스트 대화
    public void QuestTalk()
    {
        // 퀘스트 중이라면
        if (isQuest)
        {

        }
        else
        {
            
        }
    }

    // 일반 대화창
    public void Talk()
    {
        talkLine = talkStates[nextNumber].basicsLine;
        Dialogue[] dialogues = TalkManager.instance.GetDialogue(talkLine.x, talkLine.y);
        Debug.Log(dialogues[0].contexts[0]);
        TalkManager.instance.StartDialogue(dialogues);
    }
}
