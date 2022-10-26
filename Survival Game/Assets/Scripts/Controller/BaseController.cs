using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    protected RaycastHit hit;
    
    [SerializeField]
    public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;

    [SerializeField]
    protected Define.State _state = Define.State.Idle;
    public virtual Define.State State
    {
        get { return _state; }
        set { _state = value; }
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        // State 패턴
        switch (State){
            case Define.State.Moving:    // 움직임
                UpdateMoving();
                break;
            case Define.State.Idle:      // 가만히 있기
                UpdateIdle();
                break;
            case Define.State.Skill:     // 스킬
                UpdateSkill();
                break;
            case Define.State.Die:       // 죽음
                UpdateDie();
                break;
        }
    }

    public abstract void Init();

    protected virtual void UpdateMoving() {}
    protected virtual void UpdateIdle() {}
    protected virtual void UpdateSkill() {}
    protected virtual void UpdateDie() {}
}
