using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NPC 부모 클래스
public abstract class NpcController : MonoBehaviour
{
    public float scanRange = 2f;
    
    GameObject obj;

    void Start()
    {
        obj = Managers.UI.MakeWorldSpaceUI<UI_NameBar>(transform).gameObject;
        Init();
    }
    
    void Update()
    {
        NPCUpdate();
        NameBarPos();
    }

    void NameBarPos()
    {
        // 주변 플레이어 확인 후 이름 활성화
        Vector3 playerPos = Managers.Game._player.transform.position;
        float distance = Vector3.Distance(transform.position, playerPos);

        if (distance >= scanRange)
            obj.SetActive(false);
        else
            obj.SetActive(true);
    }

    protected virtual void NPCUpdate() {}

    protected abstract void Init();
    public abstract void Interaction();     // 상호작용
}
