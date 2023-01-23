using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager
{
    public UI_Stat statUI;  // 스탯 UI
    public Action addStat;  // 스탯 관련 메소드들 호출이 필요할 때

    // 장비 장착 (장비 장착이 스탯에 있기 때문에 여기에 코드 구현)
    public void EqConnection(UI_Inven_Item _invenSlot)
    {
        EqItem eqItem = _invenSlot.item as EqItem;
        for(int i=0; i<statUI.eqSlotList.Count; i++)
        {
            // 장비 부위가 같으면
            if (eqItem.eqType == statUI.eqSlotList[i].eqType)
            {
                statUI.eqSlotList[i].ConnectionSlot(_invenSlot);
                return;
            }
        }
    }

    // 쿨 지속 효과 
    // 
}
