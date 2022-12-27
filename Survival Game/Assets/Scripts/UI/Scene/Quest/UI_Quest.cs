using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Quest : UI_Scene
{
    List<Quest_Button> questList;
    GameObject questUI;

    public GameObject[] rewardItems;

    enum GameObjects
    {
        BackGround,
        List_Grid,
    }

    enum Texts
    {
        Quest_Title,
        Quest_Content,
        Gold_Text,
        Exp_Text,
    }

    public override void Init()
    {
        questList = new List<Quest_Button>();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));

        questUI = GetObject((int)GameObjects.BackGround);

        // 마우스 클릭 후 이동 가능
        questUI.BindEvent((PointerEventData eventData)=>{
            questUI.transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
        }, Define.UIEvent.Drag);

        QuestManager.instance.questUI = this;

        QuestReSet();

        questUI.SetActive(false);
    }

    // 퀘스트 목록 초기화
    void QuestReSet()
    {
        GameObject gridPanel = GetObject((int)GameObjects.List_Grid);

        foreach(Transform child in gridPanel.transform) 
            Managers.Resource.Destroy(child.gameObject);

        for(int i=0; i<10; i++)
            questList.Add(Managers.Resource.Instantiate("UI/Scene/Quest/Quest_Button", gridPanel.transform).GetComponent<Quest_Button>());

        for(int i=0; i<10; i++)
            questList[i].gameObject.SetActive(false);
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
        Quest[] quests = QuestManager.instance.procQuests.ToArray();

        if (quests.Length > 0)
        {
            for(int i=0; i<quests.Length; i++)
            {
                questList[i].gameObject.SetActive(true);
                questList[i].quest = quests[i];
                questList[i].title.text = quests[i].title;
            }

            if (questList[0].quest != null)
                QuestChoice(questList[0].quest);
        }
    }

    // 퀘스트 선택 시 퀘스트 정보 호출
    public void QuestChoice(Quest _quest)
    {
        for(int i=0; i<_quest.itemReward.Length; i++)
        {
            rewardItems[i].SetActive(true);
            Util.FindChild<Image>(rewardItems[i]).sprite = _quest.itemReward[i].item.itemImage;
            Util.FindChild<Text>(rewardItems[i]).text = _quest.itemReward[i].itemCount.ToString();
        }

        GetText((int)Texts.Quest_Title).text = _quest.title;
        GetText((int)Texts.Quest_Content).text = _quest.description;
        GetText((int)Texts.Gold_Text).text = _quest.gold.ToString();
        GetText((int)Texts.Exp_Text).text = _quest.exp.ToString();
    }

    // 퀘스트 포기 (Button)
    public void QuestGiveUp()
    {

    }
}
