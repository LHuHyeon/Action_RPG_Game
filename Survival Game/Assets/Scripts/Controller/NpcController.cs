using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPC_TalkState
{
    public Vector2 basicsLine = new Vector2(1,1);  // 기본 대화
    public Vector2 questLine = new Vector2(2,2);   // 퀘스트 시작 대화
    public Vector2 acceptLine = new Vector2(3,3);  // 퀘스트 수락 대화
    public Vector2 refusalLine = new Vector2(4,4); // 퀘스트 거절 대화
    public Vector2 procLine = new Vector2(5,5);    // 퀘스트 진행중인 대화
    public Vector2 clearLine = new Vector2(6,6);   // 퀘스트 성공 대화
}

public class NpcController : MonoBehaviour
{
    public Define.NPCState state = Define.NPCState.None;

    public NPC_TalkState[] talkStates;

    public Quest[] quest;  // 퀘스트 목록
    public int nextNumber=0;    // 현재 퀘스트 및 대화 (숫자가 올라가면 다음 퀘스트1,2,3,..)
    
    GameObject obj;

    void Start()
    {
        obj = Managers.UI.MakeWorldSpaceUI<UI_NameBar>(transform).gameObject;
        QuestManager.instance.questNpc -= ClearCheck;
        QuestManager.instance.questNpc += ClearCheck;
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

    // 퀘스트 성공 확인
    void ClearCheck(Quest _quest)
    {
        if (quest[nextNumber] == _quest)
        {
            if (quest[nextNumber].isClear)
            {
                // TODO : npc 머리 위 물음표 표시 생성
                Debug.Log($"{gameObject.name}의 미션 보상을 받으세요.");
            }
        }
    }

    // 플레이어가 상호작용할 시
    public void Interaction()
    {
        if (state == Define.NPCState.Quest)
        {
            // 퀘스트 레벨 조건 확인
            if (Managers.Game.playerStat.Level >= quest[nextNumber].minLevel)
            {
                QuestTalk();
            }
            else
            {
                Talk();
            }
        }
    }

    // 퀘스트 대화
    public void QuestTalk()
    {
        // 퀘스트 중인지 확인
        if (quest[nextNumber].isAccept)
        {
            // TODO : 퀘스트 클리어 여부 확인
            StartTalk(talkStates[nextNumber].procLine);
        }
        else
        {
            StartTalk(talkStates[nextNumber].questLine, quest[nextNumber]);
            TalkManager.instance.currentNpc = this;
        }
    }

    // 일반 대화
    public void Talk()
    {
        StartTalk(talkStates[nextNumber].basicsLine);
    }

    // 대화 시작
    public void StartTalk(Vector2 _talkLine, Quest _quest=null)
    {
        Dialogue[] dialogues = TalkManager.instance.GetDialogue(_talkLine.x, _talkLine.y);
        TalkManager.instance.StartDialogue(dialogues, _quest);
    }
}
