using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define : MonoBehaviour
{
    // NPC 상태
    public enum NPCState
    {
        None,
        Shop,
        Quest,
    }

    public enum NPCAction
    {
        Notify,
        Reward,
    }

    // 캐릭터 상태
    public enum State
    {
        Moving,
        Idle,
        Die,
        Skill,
    }

    // 무기 상태
    public enum WeaponState
    {
        Hand,
        Sword,
        Gun,
    }

    public enum WorldObject
    {
        Unknown,
        Player,
        Monster,
    }

    public enum Layer
    {
        Monster = 6,
        Ground = 7,
        Block = 9,
    }

    public enum Scene
    {
        Unknown,
        Login,
        Loby,
        Game,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,   // MaxCount를 마지막 자리에 둠으로 써 해당 enum의 최대 개수(int)가 저장됨.
    }

    public enum UIEvent
    {
        Enter,
        Exit,
        Click,
        BeginDrag,
        Drag,
        EndDrag,
        Drop,
    }
    
    public enum MouseEvent
    {
        Press,      // 꾹 누를 때 상태
        LeftDown,
        RightDown,
        PointDown,
        PointUp,
        Click,      // 클릭 상태 (Press 상태가 끝난 상태)
    }

    public enum CameraMode
    {
        QuarterView,    // 디아블로 게임 같은 시점
    }
}
