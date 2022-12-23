using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    public static TalkManager instance;

    [SerializeField]
    private string csv_FileName;

    Dictionary<int, Dialogue> dialogueDic = new Dictionary<int, Dialogue>();

    public bool isFinish = false;
    public UI_Talk talkUI;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            Dialogue[] dialogues = Parse(csv_FileName);
            for(int i=0; i<dialogues.Length; i++)
            {
                dialogueDic.Add(i+1, dialogues[i]);
                Debug.Log(dialogueDic[i+1].name);
            }

            isFinish = true;
        }
    }

    // 대화 시작
    public void StartDialogue(Dialogue[] dialogues)
    {
        talkUI.gameObject.SetActive(true);
        talkUI.SetDialogue(dialogues);
    }

    // 대본 가져오기
    public Dialogue[] GetDialogue(float _StartNum, float _EndNum)
    {
        List<Dialogue> dialogues = new List<Dialogue>();

        if (_StartNum != _EndNum)
        {
            for(int i=0; i<(int)_EndNum-_StartNum; i++)
            {
                dialogues.Add(dialogueDic[(int)_StartNum + i]);
                Debug.Log("dialogueDic : " + _StartNum + " " + _EndNum);
            }
        }
        else
            dialogues.Add(dialogueDic[(int)_StartNum]);

        return dialogues.ToArray();
    }

    // 엑셀 파일 읽기 쉽게 변환
    public Dialogue[] Parse(string _CSVFileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>();             // 대화 리스트 생성
        TextAsset csvData = Resources.Load<TextAsset>("Data/"+_CSVFileName);    // Csv 파일 가져오기

        string[] data = csvData.text.Split('\n');   // 엔터(\n) 기준으로 쪼개기

        for(int i=1; i<data.Length;)
        {
            // row[1] = 이름, row[2] = 대화 내용
            string[] row = data[i].Split(',');      // 다음 칸(,) 기준으로 쪼개기
            
            Dialogue dialogue = new Dialogue();     // 대사 리스트 생성
            List<string> contextList = new List<string>();

            dialogue.name = row[1];

            // 다음 줄의 이름이 공백인지 확인하고 대사 추가
            do{
                contextList.Add(row[2]);
                if (++i < data.Length)
                    row = data[i].Split(',');
                else
                    break;
            } while(row[0].ToString() == "");

            dialogue.contexts = contextList.ToArray();  // 대사 리스트 반환

            dialogueList.Add(dialogue);
        }

        return dialogueList.ToArray();
    }
}
