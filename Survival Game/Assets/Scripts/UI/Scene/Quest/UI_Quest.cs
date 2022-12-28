using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Quest : UI_Scene
{
    List<Quest_Button> questList;  // 퀘스트 목록 리스트
   
    Quest[] quests;                // 가지고 있는 퀘스트
    List<Text> texts;              // Text 클래스 리스트

    int currentIndex;              // 현재 보고 있는 퀘스트

    public GameObject[] rewardItems;     // 보상 아이템 오브젝트
    public GameObject questUI;           // ui 오브젝트

    enum GameObjects
    {
        BackGround,
        List_Grid,
        GiveUp_Button,
        Refusal,
    }

    enum Texts
    {
        Quest_Title,
        Objective_Content,
        Quest_Content,
        Gold_Text,
        Exp_Text,
    }

    public override void Init()
    {
        questList = new List<Quest_Button>();
        texts = new List<Text>();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));

        questUI = GetObject((int)GameObjects.BackGround);
        QuestManager.instance.questUI = this;

        ClickStayMove();    // ui 잡고 움직이기 기능
        QuestReSet();       // 퀘스트 목록 변수 등록
        Clear();            // 초기화

        GetObject((int)GameObjects.Refusal).SetActive(false);
        questUI.SetActive(false);
    }

    // 마우스 클릭 후 이동 가능
    void ClickStayMove()
    {
        // 퀘스트 목록 움직이기
        questUI.BindEvent((PointerEventData eventData)=>{
            questUI.transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
        }, Define.UIEvent.Drag);

        // 퀘스트 포기 ui 움직이기
        GetObject((int)GameObjects.Refusal).BindEvent((PointerEventData eventData)=>{
            GetObject((int)GameObjects.Refusal).transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
        }, Define.UIEvent.Drag);
    }

    // 퀘스트 목록 초기화
    void QuestReSet()
    {
        GameObject gridPanel = GetObject((int)GameObjects.List_Grid);

        // 퀘스트 목록 초기화
        foreach(Transform child in gridPanel.transform) 
            Managers.Resource.Destroy(child.gameObject);

        // 초기화된 목록 다시 넣기
        for(int i=0; i<10; i++)
            questList.Add(Managers.Resource.Instantiate("UI/Scene/Quest/Quest_Button", gridPanel.transform).GetComponent<Quest_Button>());

        // 퀘스트 목록 비활성화
        for(int i=0; i<10; i++)
            questList[i].gameObject.SetActive(false);

        // Text 객체 리스트에 넣기
        for(int i=0; i<5; i++)
            texts.Add(GetText(i));
    }

    void Update()
    {
        OnQuest();
    }

    void OnQuest()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuestManager.instance.isQuestList = !QuestManager.instance.isQuestList;

            if (QuestManager.instance.isQuestList)
            {
                questUI.SetActive(true);
                Cursor.visible = true;
                ShowQuest();
            }
            else
            {
                Cursor.visible = false;
                questUI.SetActive(false);
            }
        }
    }

    // 퀘스트 목록 보여주기
    public void ShowQuest()
    {
        quests = QuestManager.instance.procQuests.ToArray();

        if (quests.Length > 0)
        {
            for(int i=0; i<quests.Length; i++)
            {
                questList[i].gameObject.SetActive(true);
                questList[i].quest = quests[i];
                questList[i].title.text = quests[i].title;
            }

            if (questList[0].quest != null)
            {
                QuestChoice(questList[0].quest);
                currentIndex = 0;
            }
        }
    }

    // 퀘스트 선택 시 퀘스트 정보 호출
    public void QuestChoice(Quest _quest)
    {
        // 포기 버튼 UI 활성화
        GetObject((int)GameObjects.GiveUp_Button).SetActive(true);

        // Text 내용 활성화
        QuestManager.instance.QuestUISetting(texts.ToArray(), rewardItems, true, _quest);
    }

    // 퀘스트 포기 UI 호출
    public void RefusalUI()
    {
        GetObject((int)GameObjects.Refusal).SetActive(true);
    }

    // 퀘스트 포기 승인 (Button)
    public void QuestGiveUp()
    {
        QuestManager.instance.procQuests.Remove(questList[currentIndex].quest);

        // 포기할 퀘스트 초기화
        questList[currentIndex].quest.Clear();
        questList[currentIndex].quest = null;
        questList[currentIndex].title.text = string.Empty;

        Clear();
        questList[currentIndex].gameObject.SetActive(false);
    }

    // 퀘스트 포기 취소 (Button)
    public void QuestGiveUpClose()
    {
        GetObject((int)GameObjects.Refusal).SetActive(false);
    }

    // 퀘스트 초기화 
    public void Clear()
    {
        // 포기 버튼 ui 비활성화
        GetObject((int)GameObjects.GiveUp_Button).SetActive(false);
        // 포기 ui 비활성화
        GetObject((int)GameObjects.Refusal).SetActive(false);

        // Text 내용 초기화
        QuestManager.instance.QuestUISetting(texts.ToArray(), rewardItems, true);
    }
}
