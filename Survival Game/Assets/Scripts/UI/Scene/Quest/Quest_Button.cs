using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest_Button : MonoBehaviour
{
    public Quest quest=null;
    public Text title;

    public void ShowChoice()
    {
        QuestManager.instance.questUI.QuestChoice(this);
    }
}
