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

    int nextNum=0;
    int talkNum=0;

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
        Name_Text,          // NPC 이름
        Talk_Text,          // 대화
        Quest_Title_Text,   // 퀘스트 제목
        Quest_Content,      // 퀘스트 내용
        Gold_Text,          // 골드 보상
        Exp_Text,           // 경험치 보상
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));

        nameText = GetText((int)Texts.Name_Text);
        talkText = GetText((int)Texts.Talk_Text);

        TalkManager.instance.talkUI = this;

        UIClear();
        gameObject.SetActive(false);
    }

    void Update()
    {
        // 현재 대화 중인지
        if (TalkManager.instance.isDialouge)
        {
            // 다음 대화로 넘어가도 되는지, 퀘스트 확인 중이 아닐 때
            if (nextTalk && !isQuest)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    NextTalkButton();
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
        
        dialouges = _dialogues;
        TalkManager.instance.isDialouge = true;
        Cursor.visible = true;

        // 플레이어 멈추기
        Managers.Game._player.GetComponent<PlayerController>().State = Define.State.Idle;

        NextTalkButton();
    }

    // 다음 대화
    public void NextTalkButton()
    {
        talkText.text = string.Empty;   // 초기화
        nextTalk = false;
        GetObject((int)GameObjects.Next_Button).SetActive(false);

        // 다음 대화가 마지막이라면 퀘스트 UI On 
        if (dialouges.Length <= nextNum+1 && dialouges[nextNum].contexts.Length <= talkNum+1)
        {
            OnQuest();
        }

        // 이름, 대화 내용 실행
        if (dialouges.Length-1 >= nextNum)
        {
            nameText.text = dialouges[nextNum].name;

            StopCoroutine(TypeSentence(null));
            StartCoroutine(TypeSentence(dialouges[nextNum].contexts[talkNum]));
        }
        else
        {
            // 퀘스트 대화가 아니라면 초기화 진행
            if (!isQuest)
            {
                Clear();
                gameObject.SetActive(false);
            }
        }
    }

    // 퀘스트 UI ON
    void OnQuest()
    {
        isQuest = true;
        GetObject((int)GameObjects.UI_Quest).SetActive(true);

        GetText((int)Texts.Quest_Title_Text).text = quest.title;
        GetText((int)Texts.Quest_Content).text = quest.description;
        GetText((int)Texts.Gold_Text).text = quest.gold.ToString();
        GetText((int)Texts.Exp_Text).text = quest.exp.ToString();

        for(int i=0; i<quest.itemReward.Length; i++)
        {
            rewardItems[i].SetActive(true);
            Util.FindChild<Image>(rewardItems[i]).sprite = quest.itemReward[i].item.itemImage;
            Util.FindChild<Text>(rewardItems[i]).text = quest.itemReward[i].itemCount.ToString();
        }
    }

    //타이핑 모션 코루틴
    IEnumerator TypeSentence(string sentence)
    {
        foreach(var letter in sentence)
        {
            talkText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }
        
        if (dialouges[nextNum].contexts.Length <= talkNum)
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
        // TODO : 현재 퀘스트 저장(QuestManager), 수락 대화 호출 후 초기화
        Debug.Log("수락!");
    }

    // 거절 버튼
    public void RefusalButton()
    {
        // TODO : 거절 시 거절 대화 호출 후 초기화
        Debug.Log("거절!");
    }

    public void Clear()
    {
        UIClear();
        TalkManager.instance.isDialouge = false;
        Cursor.visible = false;

        dialouges = null;

        nextNum = 0;
        talkNum = 0;
    }

    void UIClear()
    {
        GetObject((int)GameObjects.Refusal_Button).SetActive(false);
        GetObject((int)GameObjects.Accept_Button).SetActive(false);

        for(int i=0; i<rewardItems.Length; i++)
            rewardItems[i].SetActive(false);

        GetObject((int)GameObjects.UI_Quest).SetActive(false);
    }
}
