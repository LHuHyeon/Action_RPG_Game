using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    // private static = 같은 클래스 안에서만 호출 가능.
    // public static = 다른 클래스에서도 호출 가능.
    private static Managers s_instance; // 유일성이 보장된다.
    private static Managers Instance { get { Init(); return s_instance; } }  // 유일한 매니저를 갖고 온다.

    private GameManager _game = new GameManager();
    private StatManager _stat = new StatManager();
    private WeaponManager _weapon = new WeaponManager();

    public static GameManager Game { get { return Instance._game; } }
    public static StatManager Stat { get { return Instance._stat; } }
    public static WeaponManager Weapon { get { return Instance._weapon; } }

    private DataManager _data = new DataManager();
    private InputManager _input = new InputManager();
    private PoolManager _pool = new PoolManager();
    private ResourceManager _resource = new ResourceManager();
    private SceneManagerEx _scene = new SceneManagerEx();
    // private SoundManager _sound = new SoundManager();
    private UIManager _ui = new UIManager();

    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    // public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }

    void Start()
    {
        Init();     // 싱글톤
    }

    void Update()
    {
        Input.OnUpdate();   // 키입력 메소드 호출
    }

    // 싱글톤 메소드
    static void Init()
    {
        if (s_instance == null){
            GameObject go = GameObject.Find("@Manager");// 오브젝트 찾기

            if (go == null){
                go = new GameObject{name = "@Manager"}; // 오브젝트 이름 설정
                go.AddComponent<Managers>();            // 컴포넌트 추가
                Debug.Log("@Manager 생성.");
            }

            DontDestroyOnLoad(go);                      // 씬변경될 때 삭제 안됨.
            s_instance = go.GetComponent<Managers>();

            s_instance._pool.Init();
            s_instance._data.Init();
        }
    }
    
    public static void Clear()
    {

    }
}
