using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField] protected int _level;
    [SerializeField] protected int _hp;
    [SerializeField] protected int _sp;
    [SerializeField] protected int _maxHp;
    [SerializeField] protected int _maxSp;
    [SerializeField] protected int _attack;
    [SerializeField] protected int _defense;
    [SerializeField] protected float _movespeed;
    [SerializeField] protected float _addspeed;

    public int Level { get { return _level; } set { _level = value; } }
    public int Hp {
        get { return _hp; }
        set {
            _hp = value;
            if (_hp > _maxHp)
                _hp = _maxHp;
        }
    }
    public int Sp {
        get { return _sp; }
        set {
            _sp = value;
            if (_sp > _maxSp)
                _sp = _maxSp;
        }
    }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int MaxSp { get { return _maxSp; } set { _maxSp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _movespeed; } set { _movespeed = value; } }
    
    float tempSpeed;
    public float AddSpeed { // 추가 속도 증가
        get { return _addspeed; }
        set {
            _addspeed = value;

            if (_addspeed == 0)
                _movespeed = tempSpeed;
            else
            {
                tempSpeed = _movespeed;
                _movespeed = _movespeed + _addspeed;
            }
        }
    }

    void Start()
    {
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _attack = 10;
        _defense = 5;
        _movespeed = 2.0f;
        tempSpeed = _movespeed;
    }

    // 공격을 받았을 때
    public virtual void OnAttacked(Stat attacker, int addDamage=0)
    {
        int damage = Mathf.Max(0, (attacker.Attack + addDamage) - Defense);
        Hp -= damage;

        if (Hp <= 0){
            Hp = 0;
            OnDead(attacker);
        }
    }

    // 죽었을 때
    protected virtual void OnDead(Stat attacker)
    {
        // attacker가 Player라면 경험치 흭득.
        PlayerStat playerStat = attacker as PlayerStat;
        if (playerStat != null){
            playerStat.Exp += 5;
        }
        Managers.Game.Despawn(gameObject);
    }
}
