using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Talk : UI_Scene
{
    Text nameText;
    Text talkText;

    Dialogue[] dialouges;
    Quest quest;
    List<Text> texts;   // Text 클래스 리스트

    int nextNum=0;      // 다음 대화 확인용
    int talkNum=0;      // 다음 대사 확인용

    float delayTime = 0.1f;    // 타이핑 모션 딜레이

    [SerializeField]
    GameObject[] rewardItems;

    bool nextTalk = false;  // 다음 대화로 넘어갈 수 있는지
    bool isQuest = false;   // 퀘스트 호출 중인지

    enum GameObjects
    {
        UI_Quest,           // 퀘스트 UI 
        
        // ↓ SetActive 용도
        Refusal_Button,     // 거절 버튼
        Accept_Button,      // 수락 버튼
        Next_Button,        // 다음 대화 버튼
    }

    enum Texts
    {
        Quest_Title_Text,   // 퀘스트 제목
        Objective_Content,  // 목표 내용
        Quest_Content,      // 퀘스트 내용
        Gold_Text,          // 골드 보상
        Exp_Text,           // 경험치 보상
        Name_Text,          // NPC 이름
        Talk_Text,          // 대화
    }

    public override void Init()
    {
        texts = new List<Text>();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));

        nameText = GetText((int)Texts.Name_Text);
        talkText = GetText((int)Texts.Talk_Text);

        TalkManager.instance.talkUI = this;

        // 퀘스트 text UI 리스트 넣기
        for(int i=0; i<5; i++)
            texts.Add(GetText(i));

        UIClear();
        gameObject.SetActive(false);
    }

    void Update()
    {
        // 현재 대화 중인지
        if (TalkManager.instance.isDialouge)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                // 다음 대화로 넘어가도 되는지, 퀘스트 확인 중이 아닐 때
                if (nextTalk && !isQuest)
                {
                    NextTalkButton();
                    Debug.Log("다음 대화!");
                }
                else
                {
                    // 대화 딜레이 줄이기
                    delayTime = delayTime / 4;
                }
            }
        }
    }

    // 대화 세팅 후 시작
    public void SetDialogue(Dialogue[] _dialogues, Quest _quest=null)
    {
        // 퀘스트
        if (_quest != null)
            quest = _quest;
        
        Managers.UI.CloseAllUI();   // 활성화된 모든 UI 비활성화
        
        dialouges = _dialogues;
        TalkManager.instance.isDialouge = true;
        Cursor.visible = true;

        // 플레이어 멈추기
        Managers.Game._player.GetComponent<PlayerController>().state = Define.PlayerState.Idle;

        // 다음 대화
        NextTalkButton();
    }

    // 다음 대화
    public void NextTalkButton()
    {
        // 대화의 기본값으로 초기화
        delayTime = 0.1f;
        talkText.text = string.Empty;
        nextTalk = false;
        GetObject((int)GameObjects.Next_Button).SetActive(false);

        // 이름, 대화 내용 실행
        if (dialouges.Length-1 >= nextNum)
        {
            // 다음 대화가 마지막이라면 퀘스트 UI On 
            if (quest != null)
            {
                if (dialouges.Length <= nextNum+1 && dialouges[nextNum].contexts.Length <= talkNum+1)
                    OnQuest();
            }

            nameText.text = dialouges[nextNum].name;

            StopCoroutine(TypeSentence(null));
            StartCoroutine(TypeSentence(dialouges[nextNum].contexts[talkNum]));
        }
        else
        {
            // 퀘스트 대화가 아니라면 초기화 진행
            if (!isQuest)
            {
                // 클리어 했다면
                if (quest != null)
                {
                    if (quest.isClear)
                        QuestManager.instance.Reward(quest);
                }

                Clear();
                gameObject.SetActive(false);
            }
        }
    }

    // 퀘스트 UI ON
    void OnQuest()
    {
        if (!quest.isAccept)
            isQuest = true;

        GetObject((int)GameObjects.UI_Quest).SetActive(true);

        QuestManager.instance.QuestUISetting(texts.ToArray(), rewardItems, true, quest);
    }

    // 타이핑 모션 코루틴
    IEnumerator TypeSentence(string sentence)
    {
        // 대화 타이밍 모션 실행
        foreach(var letter in sentence)
        {
            talkText.text += letter;
            yield return new WaitForSeconds(delayTime);
        }
        
        // 다음 대사 확인
        if (dialouges[nextNum].contexts.Length <= talkNum+1)
        {
            nextNum++;
            talkNum = 0;
        }
        else
            talkNum++;

        // 퀘스트 UI 활성화된 경우
        if (isQuest)
        {
            // 수락, 거절 버튼 활성화
            GetObject((int)GameObjects.Refusal_Button).SetActive(true);
            GetObject((int)GameObjects.Accept_Button).SetActive(true);
        }
        else
        {
            GetObject((int)GameObjects.Next_Button).SetActive(true);
            nextTalk = true;
        }
    }

    // 수락 버튼
    public void AcceptButton()
    {
        Debug.Log("수락!");

        // 퀘스트 저장 및 수락 대화 진행
        QuestManager.instance.ConnectionQuest(quest);

        // 대화 초기화 후 수락 대화 진행
        Clear();
        QuestNpc npc = TalkManager.instance.currentNpc;
        npc.StartTalk(npc.talkStates[npc.nextNumber].acceptLine);
    }

    // 거절 버튼
    public void RefusalButton()
    {
        Debug.Log("거절!");

        // 대화 초기화 후 거절 대화 진행
        Clear();
        QuestNpc npc = TalkManager.instance.currentNpc;
        npc.StartTalk(npc.talkStates[npc.nextNumber].refusalLine);
    }

    // 전체 초기화
    public void Clear()
    {
        UIClear();
        TalkManager.instance.isDialouge = false;
        Cursor.visible = false;
        
        isQuest = false;
        nextTalk = false;

        dialouges = null;
        quest = null;

        nextNum = 0;
        talkNum = 0;
    }

    // Quest 관련 UI 초기화
    void UIClear()
    {
        GetObject((int)GameObjects.Refusal_Button).SetActive(false);
        GetObject((int)GameObjects.Accept_Button).SetActive(false);

        for(int i=0; i<rewardItems.Length; i++)
            rewardItems[i].SetActive(false);

        GetObject((int)GameObjects.UI_Quest).SetActive(false);
    }
}
