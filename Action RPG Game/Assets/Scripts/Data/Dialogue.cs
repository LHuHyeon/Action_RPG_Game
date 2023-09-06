using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 한 캐릭터의 대사 내용
[System.Serializable]
public class Dialogue
{
    [Tooltip("대사 캐릭터 이름")]
    public string name;

    [Tooltip("대사 내용")]
    public string[] contexts;
}

// 엑셀파일에서 데이터를 받아 저장할 클래스
[System.Serializable]
public class DialogueEvent
{
    public string name;

    public Vector2 line;
    public Dialogue[] dialogue;
}
