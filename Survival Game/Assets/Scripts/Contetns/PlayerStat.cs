using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 스탯
public class PlayerStat : Stat
{
    [SerializeField] protected int _statPoint;
    [SerializeField] protected int _exp;
    [SerializeField] protected int _gold;
    [SerializeField] protected int _luk;

    public int totalExp;    // 채워야 할 경험치

// 스탯 포인트 변수
#region StatPoint
    private int _hpPoint;
    private int _spPoint;
    private int _atkPoint;
    private int _lukPoint;
    private int _dpPoint;

    public int HPPoint {
        get { return _hpPoint; } 
        set { 
            _hpPoint = value;
            if (_hpPoint > 0)
            {
                MaxHp += 20;
                Hp += 20;
            }
        }
    }
    public int SPPoint {
        get { return _spPoint; } 
        set { 
            _spPoint = value;
            if (_spPoint > 0)
            {
                MaxSp += 20;
                Sp += 20;
            }
        }
    }
    public int ATKPoint {
        get { return _atkPoint; } 
        set { 
            _atkPoint = value;
            if (_atkPoint > 0)
            {
                Attack += 5;
            }
        }
    }
    public int LUKPoint {
        get { return _lukPoint; } 
        set { 
            _lukPoint = value;
            if (_lukPoint > 0)
            {
                Luk += 1;
            }
        }
    }
    public int DPPoint {
        get { return _dpPoint; } 
        set { 
            _dpPoint = value;
            if (_dpPoint > 0)
            {
                Defense += 5;
            }
        }
    }
#endregion
    
    int basePoint;
    public int StatPoint { get { return _statPoint; } set { _statPoint = value; } }
    public int Gold { 
        get { return _gold; }
        set {
            _gold = value;
            Managers.Game.baseInventory.Gold = _gold;
        }
    }
    public int Exp
    { 
        get { return _exp; }
        set
        { 
            _exp = value;

            int level = Level;

            while (true){
                Data.Stat stat;
                
                // 해당 Key에 Value가 존재 하는지 여부
                if (Managers.Data.StatDict.TryGetValue(level + 1, out stat) == false)
                    break;
                else
                    totalExp = stat.totalExp;

                // 경험치가 다음 레벨 경험치보다 작은지 확인
                if (_exp < stat.totalExp)
                    break;
                
                level++;
            }

            if (level != Level){
                Level = level;
                SetStat(Level);
                _exp = 0;
                Debug.Log("Level UP!!");
            }
        }
    }
    public int Luk
    {
        get { return _luk; }
        set { _luk = value; }
    }

    void Start()
    {
        _level = 1;

        _defense = 5;
        _movespeed = 3.0f;
        _gold = 0;
        _exp = 0;
        _luk = 0;

        _hpPoint = 0;
        _spPoint = 0;
        _atkPoint = 0;
        _lukPoint = 0;
        _dpPoint = 0;

        SetStat(_level);
    }
    
    // 스텟 새로 설정
    public void SetStat(int level)
    {
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        Data.Stat stat = dict[level];

        StatSetting(stat);
        
        basePoint += stat.statPoint;
        _statPoint += stat.statPoint;
    }

    // 스탯 초기화
    public void StatClear()
    {
        _hpPoint = 0;
        _spPoint = 0;
        _atkPoint = 0;
        _lukPoint = 0;
        _dpPoint = 0;

        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        StatSetting(dict[Level]);

        _luk = 0;
        _defense = 5;

        _statPoint = basePoint;
    }

    // 스탯 세팅
    void StatSetting(Data.Stat _stat)
    {
        // TODO : 하드코딩이 아닌 다른 방식으로 하기
        _maxHp = _stat.maxHp + (_hpPoint*20);
        _hp = _stat.maxHp;
        _maxSp = _stat.maxSp + (_spPoint*20);
        _sp = _stat.maxSp;
        _attack = _stat.attack + (_atkPoint*5);
    }

    protected override void OnDead(Stat attacker)
    {
        Debug.Log("Player Dead");
    }
}