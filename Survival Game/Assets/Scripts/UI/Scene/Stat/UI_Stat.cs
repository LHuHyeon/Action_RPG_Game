using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Stat : UI_Scene
{
    public GameObject title;    // 타이틀 Obj

    [SerializeField]
    private Slider expGauge;    // 슬라이더

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

    // 스탯 포인트 버튼
    enum Buttons
    {
        HP_Button,
        SP_Button,
        ATK_Button,
        LUK_Button,
        DP_Button,
    }

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        UISetting();

        Managers.Input.KeyAction -= OnStatUI;
        Managers.Input.KeyAction += OnStatUI;

        baseObject.SetActive(false);
    }

    // UI 세팅
    void UISetting()
    {
        // 버튼 이벤트 설정
        for(int i=0; i<5; i++)
        {
            int temp = i; // Closure 문제때문에 복사해서 사용한다.
            GetButton(i).onClick.AddListener(() => AddStatButton(temp));
        }

        // 스탯창 옮기기 EventSystem 등록
        title.BindEvent((PointerEventData eventData)=>{
            baseObject.transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
            Managers.UI.OnUI(this);
        }, Define.UIEvent.Drag);

        // ui를 클릭할 시 order 우선순위
        baseObject.BindEvent((PointerEventData eventData)=>{
            Managers.UI.OnUI(this);
        }, Define.UIEvent.Click);
    }

    // 스탯 On/Off
    public void OnStatUI()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            MissActive();
            Managers.Game.isStat = !Managers.Game.isStat;

            // 활성화/비활성화
            Managers.Game.IsActive(Managers.Game.isStat, this);

            if (Managers.Game.isStat)
                StatSetting();      // 스탯 업데이트
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

        GetText((int)Texts.HP_Add).text = _stat.HPPoint.ToString();
        GetText((int)Texts.SP_Add).text = _stat.SPPoint.ToString();
        GetText((int)Texts.ATK_Add).text = _stat.ATKPoint.ToString();
        GetText((int)Texts.LUK_Add).text = _stat.LUKPoint.ToString();
        GetText((int)Texts.DP_Add).text = _stat.DPPoint.ToString();
    }

    // 스탯 올리기 (Button)
    public void AddStatButton(int _index=10)
    {
        PlayerStat _stat = Managers.Game.playerStat;

        if (_stat.StatPoint > 0)
        {
            switch (_index)
            {
                case 0:                     // 체력
                    _stat.HPPoint++;
                    _stat.StatPoint--;
                    break;
                case 1:                     // 스테미나
                    _stat.SPPoint++;
                    _stat.StatPoint--;
                    break;
                case 2:                     // 공격력
                    _stat.ATKPoint++;
                    _stat.StatPoint--;
                    break;
                case 3:                     // 행운
                    _stat.LUKPoint++;
                    _stat.StatPoint--;
                    break;
                case 4:                     // 방어력
                    _stat.DPPoint++;
                    _stat.StatPoint--;
                    break;
            }
            StatSetting();
        }
        else
            Debug.Log("스탯 포인트가 부족합니다.");
    }

    // 스탯 초기화 (Button)
    public void StatClear()
    {
        Managers.Game.playerStat.StatClear();
        StatSetting();
    }

    // Active가 bool과 엇갈렸는지 확인
    void MissActive()
    {
        // 오브젝트는 true인데 bool 변수는 false일 수 있으니 true로 바꿔주기
        if (baseObject.activeSelf == true)
            Managers.Game.isStat = true;
        else
            Managers.Game.isStat = false;
    }
}
