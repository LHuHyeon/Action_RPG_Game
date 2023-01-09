using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Stat : UI_Scene
{
    public GameObject statUI;

    [SerializeField]
    private Slider expGauge;

    enum Texts
    {
        Level_Text,     // 레벨 Text
        Exp_Text,       // 경험치 Text
        Point_Text,     // 스탯 포인트 Text 
        HP_Base,        // 스탯 Base
        SP_Base,
        ATK_Base,
        LUK_Base,
        DP_Base,
        HP_Add,         // 스탯 Add
        SP_Add,
        ATK_Add,
        LUK_Add,
        DP_Add,
    }

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
    }

    void Update()
    {
        OnStatUI();
    }

    // 스탯 On/Off
    public void OnStatUI()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StatSetting();
        }
    }

    // Stat 세팅
    public void StatSetting()
    {
        PlayerStat _stat = Managers.Game.playerStat;
        float ratio = (float)_stat.Exp / _stat.totalExp;

        // Slider 값이 NaN이 된다면 값 수정이 불가능하기 때문에 사전에 처리해야 한다.
        if (float.IsNaN(ratio) == false)
        {
            expGauge.value = ratio;
            GetText((int)Texts.Exp_Text).text = (ratio*100).ToString("F1")+"%";
        }
        else
        {
            expGauge.value = 0;
            GetText((int)Texts.Exp_Text).text = "0.0%";
        }

        GetText((int)Texts.Level_Text).text = _stat.Level.ToString();
        GetText((int)Texts.Point_Text).text = _stat.StatPoint.ToString();

        GetText((int)Texts.HP_Base).text = _stat.MaxHp.ToString();
        GetText((int)Texts.SP_Base).text = _stat.MaxSp.ToString();
        GetText((int)Texts.ATK_Base).text = _stat.Attack.ToString();
        GetText((int)Texts.LUK_Base).text = _stat.Luk.ToString();
        GetText((int)Texts.DP_Base).text = _stat.Defense.ToString();

        GetText((int)Texts.HP_Add).text = _stat.hpPoint.ToString();
        GetText((int)Texts.SP_Add).text = _stat.spPoint.ToString();
        GetText((int)Texts.ATK_Add).text = _stat.atkPoint.ToString();
        GetText((int)Texts.LUK_Add).text = _stat.lukPoint.ToString();
        GetText((int)Texts.DP_Add).text = _stat.dpPoint.ToString();
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
