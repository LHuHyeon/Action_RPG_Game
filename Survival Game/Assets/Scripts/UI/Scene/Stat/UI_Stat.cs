using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StatList
{
    public Text baseText;
    public Text addText;
}

public class UI_Stat : UI_Scene
{
    // 0 = HP | 1 = SP | 2 = ATK | 3 = LUK | 4 = DP
    public StatList[] statList;
    public GameObject statUI;

    [SerializeField]
    private Slider expGauge;

    enum Texts
    {
        Level_Text,     // 레벨 Text
        Exp_Text,       // 경험치 Text
        Point_Text,     // 스탯 포인트 Text 
    }

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
    }

    // 스탯 On/Off
    public void OnStatUI()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {

        }
    }

    // 스탯 올리기 (Button)
    public void AddStat()
    {

    }

    // 스탯 초기화 (Button)
    public void StatClear()
    {

    }
}
