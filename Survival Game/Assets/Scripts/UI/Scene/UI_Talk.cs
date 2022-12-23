using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Talk : UI_Scene
{
    /*
    1. 대화 내용
    2. 수락, 거절 버튼
    3. 퀘스트라면 퀘스트 ui도 같이 호출
    */
    Text nameText;
    Text talkText;

    Dialogue[] dialouges;

    int nextNum=0;
    int talkNum=0;

    public bool isDialouge = false; // 현재 대화 중인지
    bool nextTalk = false;  // 다음 대화로 넘어갈 수 있는지

    enum Texts
    {
        Name_Text,
        Talk_Text,
    }

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        nameText = GetText((int)Texts.Name_Text);
        talkText = GetText((int)Texts.Talk_Text);

        TalkManager.instance.talkUI = this;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (isDialouge)
        {
            if (nextTalk)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    NextTalkButton();
                }
            }
        }
    }

    // 대화 세팅 후 시작
    public void SetDialogue(Dialogue[] _dialogues)
    {
        dialouges = _dialogues;
        isDialouge = true;

        NextTalkButton();
    }

    // 다음 대화
    public void NextTalkButton()
    {
        talkText.text = string.Empty;   // 초기화
        nextTalk = false;

        if (dialouges.Length-1 >= nextNum)
        {
            nameText.text = dialouges[nextNum].name;    // 이름 대입

            StopCoroutine(TypeSentence(null));
            StartCoroutine(TypeSentence(dialouges[nextNum].contexts[talkNum]));
        }
        else
        {
            Clear();
            gameObject.SetActive(false);
        }
    }

    //타이핑 모션 함수
    IEnumerator TypeSentence(string sentence)
    {
        foreach(var letter in sentence)
        {
            talkText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }
        
        if (dialouges[nextNum].contexts.Length-1 <= talkNum)
        {
            nextNum++;
            talkNum = 0;
        }
        else
            talkNum++;

        nextTalk = true;
        Debug.Log($"dialougesNum : {dialouges.Length} , nextNum : {nextNum}");
    }

    // 수락 버튼
    public void AcceptButton()
    {

    }

    // 거절 버튼
    public void RefusalButton()
    {

    }

    public void Clear()
    {
        isDialouge = false;

        dialouges = null;

        nextNum = 0;
        talkNum = 0;
    }
}
