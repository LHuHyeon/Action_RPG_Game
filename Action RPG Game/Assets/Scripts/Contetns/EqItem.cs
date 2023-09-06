using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentStat
{
    [Tooltip("HP, SP, ATTACK, DEFENSE 만 가능합니다.")]
    public string[] part;  // 효과.
    public int[] num;  // 수치.
}

[CreateAssetMenu(fileName = "New Equipment", menuName = "New Item/Equipment")]
public class EqItem : Item
{
    public Define.EqType eqType;    // 장비 타입
    public int minLevel;            // 장착 레벨
    public EquipmentStat eqStat;    // 장비 스탯

    // 스탯 호출
    public Dictionary<string, int> GetStat()
    {
        Dictionary<string, int> statDic = new Dictionary<string, int>();

        for(int i=0; i<eqStat.part.Length; i++)
            statDic.Add(eqStat.part[i], eqStat.num[i]);

        return statDic;
    }

    // 플레이어에게 스탯 적용
    public void StatImport(bool isTrue)
    {
        PlayerStat _stat = Managers.Game.playerStat;
        Dictionary<string, int> statDic = GetStat();

        if (isTrue)
        {
            _stat.MaxHp += statDic.ContainsKey("HP") ? statDic["HP"] : 0;
            _stat.MaxSp += statDic.ContainsKey("SP") ? statDic["SP"] : 0;
            _stat.Attack += statDic.ContainsKey("ATTACK") ? statDic["ATTACK"] : 0;
            _stat.Defense += statDic.ContainsKey("DEFENSE") ? statDic["DEFENSE"] : 0;
        }
        else
        {
            _stat.MaxHp -= statDic.ContainsKey("HP") ? statDic["HP"] : 0;
            _stat.MaxSp -= statDic.ContainsKey("SP") ? statDic["SP"] : 0;
            _stat.Attack -= statDic.ContainsKey("ATTACK") ? statDic["ATTACK"] : 0;
            _stat.Defense -= statDic.ContainsKey("DEFENSE") ? statDic["DEFENSE"] : 0;
        }
    }
}
