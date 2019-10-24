using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어의 기본속성이 들어있다.
 * 분할된 클래스 내 필드들의 Awake는 여기서 한번에 이루어진다.
 */

public enum _ATTRIBUTE_STATE
{
    //속성상태 
    DEAD = 0,
    ALIVE = 1
}

public enum _ACTION_STATE //액션(움직임)의 상태
{
    // 단일 STATE
    IDLE = 0,
    DEATH,
    CIR,
    AIM, 
    SHOT,

    // 복합 STATE
    CIR_AIM,
    CIR_AIM_SHOT,
}
public partial class PlayersManager : MonoBehaviour
{ 
    public static PlayersManager instance;
    private NetworkManager networkManager;
    public int myIndex { get; set; } //가독성을위해 하나 더만들어줌

    #region PLAYERS_ROBIN
    public GameObject[] obj_players { get; set; }
    public Transform[] tf_players { get; set; } 
    #endregion

    #region PLAYERS_STATE 
    public _ACTION_STATE[] actionState { get; set; } //행동상태
    public _ATTRIBUTE_STATE[] attributeState { get; set; }
    #endregion

    #region PLAYERS_ATTRIBUTE
    public string[] nickname { get; set; }
    public float[] speed { get; set; }
    public float[] hp { get; set; }
    public float maxSpeed { get; set; }
    public float maxHp { get; set; }
    public Vector3[] direction { get; set; } //플레이어의 방향
    public Vector3[] direction2 { get; set; }
    #endregion

    void Awake()
    {
        if (instance == null)
            instance = this;

   #if NETWORK
        networkManager = NetworkManager.instance;
   #endif

        myIndex = GameManager.instance.myIndex; //게임매니저에서 받은 인덱스를 다시등록
        Initialization(C_Global.MAX_PLAYER);

        //내 인덱스번호에 맞는 로빈오브젝트와 합체.
        obj_players[myIndex] = GameObject.FindGameObjectWithTag(GameManager.instance.myTag);
        tf_players[myIndex] = obj_players[myIndex].GetComponent<Transform>().transform; //최적화를위해 트랜스폼만 따로저장.
        am_animePlayer[myIndex] = obj_players[myIndex].GetComponent<Animator>();

        // 나를 제외한 플레이어 오브젝트에 나머지 3명을 순서대로 등록. 애니메이터도.
        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            if (myIndex != i)
            {
                obj_players[i] = GameObject.FindGameObjectWithTag(GameManager.instance.playersTag[i]);
                tf_players[i] = obj_players[i].GetComponent<Transform>().transform;
                am_animePlayer[i] = obj_players[i].GetComponent<Animator>(); 
            }
        }
    }

	void Start()
	{
		Initialization_GameInfo();
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            StateManager.instance.Idle(true);
    }
    void Initialization(int _num) //클라이언트 자체에서 그냥 초기화. 어차피 서버에서 정보가 있으니까 이렇게함.
    {
        obj_players    = new GameObject[C_Global.MAX_PLAYER];
        tf_players     = new Transform[C_Global.MAX_PLAYER];
        am_animePlayer = new Animator[C_Global.MAX_PLAYER];
        nickname       = new string[C_Global.MAX_PLAYER];
        speed          = new float[C_Global.MAX_PLAYER];
        hp             = new float[C_Global.MAX_PLAYER];
        direction      = new Vector3[C_Global.MAX_PLAYER];
        direction2     = new Vector3[C_Global.MAX_PLAYER];
        actionState    = new _ACTION_STATE[C_Global.MAX_PLAYER];
        attributeState = new _ATTRIBUTE_STATE[C_Global.MAX_PLAYER];
        lastPosX       = new float[C_Global.MAX_PLAYER];
        lastPosZ       = new float[C_Global.MAX_PLAYER];

        //재생중이였던 이전 코루틴.
        curCor = null;
	}

	void Initialization_GameInfo()
	{
#if NETWORK
		maxSpeed = GameManager.instance.gameInfo.maxSpeed;
		maxHp = GameManager.instance.gameInfo.maxHealth;

		for (int i = 0; i < C_Global.MAX_PLAYER; i++)
		{
			attributeState[i] = _ATTRIBUTE_STATE.ALIVE;
			actionState[i] = _ACTION_STATE.IDLE;
			speed[i] = maxSpeed;
			hp[i] = maxHp;
		}
#else
		maxSpeed = 4.0f;
		maxHp = 100.0f;

		for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
             attributeState[i] = _ATTRIBUTE_STATE.ALIVE;
             actionState[i] = _ACTION_STATE.IDLE;
             speed[i] = maxSpeed;
             hp[i] = maxHp;
        }
#endif
	}
}

