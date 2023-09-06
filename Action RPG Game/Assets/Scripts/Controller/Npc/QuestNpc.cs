using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPC_TalkState
{
    public Vector2 basicsLine;  // 기본 대화
    public Vector2 questLine;   // 퀘스트 시작 대화
    public Vector2 acceptLine;  // 퀘스트 수락 대화
    public Vector2 refusalLine; // 퀘스트 거절 대화
    public Vector2 procLine;    // 퀘스트 진행중인 대화
    public Vector2 clearLine;   // 퀘스트 성공 대화
}

public class QuestNpc : NpcController
{
    public NPC_TalkState[] talkStates;

    public Quest[] quest;           // 퀘스트 목록
    public int nextNumber=0;        // 현재 퀘스트 및 대화 (숫자가 올라가면 다음 퀘스트1,2,3,..)
    public bool isReward = false;   // 보상 수령 여부

    [SerializeField]
    private int startTalkNumber;    // 엑셀 대화 시작 번호
    
    protected override void Init()
    {
        QuestManager.instance.questNpc -= ClearCheck;
        QuestManager.instance.questNpc += ClearCheck;

        // 대화 위치 값 넣기 (1~6, 7~12,..)
        talkStates = new NPC_TalkState[quest.Length];
        for(int i=0; i<quest.Length; i++)
        {
            int num = startTalkNumber + (i*6);

            talkStates[i]               = new NPC_TalkState();
            talkStates[i].basicsLine    = new Vector2(num, num);
            talkStates[i].questLine     = new Vector2(num+1, num+1);
            talkStates[i].acceptLine    = new Vector2(num+2, num+2);
            talkStates[i].refusalLine   = new Vector2(num+3, num+3);
            talkStates[i].procLine      = new Vector2(num+4, num+4);
            talkStates[i].clearLine     = new Vector2(num+5, num+5);
        }
        
        // 시작 시 NPC가 가지고 있는 퀘스트 모두 초기화
        for(int i=0; i<quest.Length; i++)
            quest[i].Clear();
    }

    // 퀘스트 성공 확인 및 보상 수령
    void ClearCheck(Define.NPCAction evt)
    {
        // 다음 퀘스트가 없다면 종료
        if (quest.Length-1 < nextNumber)
            return;

        // 퀘스트 성공 확인
        if (evt == Define.NPCAction.Notify)
        {
            // 퀘스트 중이라면
            if (quest[nextNumber].isAccept)
            {
                // 클리어 했는가?
                if (quest[nextNumber].isClear)
                {
                    // TODO : npc 머리 위 물음표 표시 생성
                    Debug.Log($"{gameObject.name}의 미션 보상을 받으세요.");
                    isReward = true;
                }
                else
                {
                    Debug.Log("아직 퀘스트 중입니다.");
                }
            }
        }
        
        // 보상 수령
        if (evt == Define.NPCAction.Reward)
        {
            // 현재 대화 하는 npc가 맞는지
            if (TalkManager.instance.currentNpc == this)
            {
                if (isReward)
                {
                    // 보상 아이템 인벤토리에 지급
                    Quest.Reward[] reward = quest[nextNumber].itemReward;
                    for(int i=0; i<reward.Length; i++)
                        Managers.Game.baseInventory.AcquireItem(reward[i].item, reward[i].itemCount);

                    Managers.Game.playerStat.Gold += quest[nextNumber].gold;    // 골드 지급
                    Managers.Game.playerStat.Exp += quest[nextNumber].exp;      // 경험치 지급

                    nextNumber++;
                    isReward = false;
                }
            }
        }
    }

    // 플레이어가 상호작용할 시
    public override void Interaction()
    {
        // 다음 퀘스트가 없다면 일반 대화 후 종료
        if (quest.Length-1 < nextNumber)
        {
            Talk();
            return;
        }

        // 퀘스트 레벨 조건 확인
        if (Managers.Game.playerStat.Level >= quest[nextNumber].minLevel)
            QuestTalk();
        else
            Talk();
    }

    // 퀘스트 대화
    public void QuestTalk()
    {
        // 퀘스트 중인지 확인
        if (quest[nextNumber].isAccept)
        {
            // 퀘스트 목표 달성 시
            if (quest[nextNumber].achieveValue >= quest[nextNumber].maxTargetValue)
            {
                StartTalk(talkStates[nextNumber].clearLine, quest[nextNumber]);
                TalkManager.instance.currentNpc = this;
            }
            else
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
        // 다음 대화가 없다면 그전 대화 진행
        if (talkStates.Length-1 < nextNumber)
            StartTalk(talkStates[nextNumber-1].basicsLine);
        else
            StartTalk(talkStates[nextNumber].basicsLine);
    }

    // 대화 시작
    public void StartTalk(Vector2 _talkLine, Quest _quest=null)
    {
        Dialogue[] dialogues = TalkManager.instance.GetDialogue(_talkLine.x, _talkLine.y);
        TalkManager.instance.StartDialogue(dialogues, _quest);
    }
}
