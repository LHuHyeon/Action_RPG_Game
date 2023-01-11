using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Button.onclick 을 모를 때 만들어버렸다..
// 나중에 수정하자.
public class Quest_Button : MonoBehaviour
{
    public Quest quest=null;
    public Text title;

    public void ShowChoice()
    {
        QuestManager.instance.questUI.QuestChoice(this);
    }
}
