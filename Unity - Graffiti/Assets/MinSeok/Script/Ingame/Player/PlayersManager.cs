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
    CIRCUIT,
    AIMING,

    // 복합 STATE
    CIRCUIT_AND_AIMING = (CIRCUIT + AIMING),
}

public partial class PlayersManager : MonoBehaviour
{
    //public const int NUM = 4;

    public static PlayersManager instance;
    public int myIndex { get; set; } //가독성을위해 하나 더만들어줌
    public int coroutineFlag { get; set; }
    private IEnumerator coroutine;

    #region PLAYERS_ROBIN
    public GameObject[] obj_players { get; set; }
    #endregion

    #region PLAYERS_ANIMATOR
    public Animator[] am_animePlayer { get; set; }
    #endregion

    #region PLAYERS_STATE 
    // 현재 액션 스테이트 
    public _ACTION_STATE[] actionState { get; set; }
    public _ATTRIBUTE_STATE[] attributeState { get; set; }
    #endregion

    #region PLAYERS_ATTRIBUTE
    public float[] speed { get; set; }
    public float[] hp { get; set; }
    public float[] maxSpeed { get; set; }
    public float[] maxHp { get; set; }
    public Vector3[] direction { get; set; } //플레이어의 방향
    public Vector3[] direction2 { get; set; }
    #endregion


    void Awake()
    {
        if (instance == null)
            instance = this;

       // coroutine = MovePlayer();
        coroutineFlag = 0;
        myIndex = GameManager.instance.myIndex; //게임매니저에서 받은 인덱스를 다시등록
        Initialization(C_Global.MAX_PLAYER); //기타 초기화

        //내 인덱스번호에 맞는 로빈오브젝트와 합체.
        obj_players[myIndex] = GameObject.FindGameObjectWithTag(GameManager.instance.myTag);
        am_animePlayer[myIndex] = obj_players[myIndex].GetComponent<Animator>();

        // 나를 제외한 플레이어 오브젝트에 나머지 3명을 순서대로 등록. 애니메이터도.
        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            if (myIndex != i)
            {
                obj_players[i] = GameObject.FindGameObjectWithTag(GameManager.instance.playersTag[i]);
                am_animePlayer[i] = obj_players[i].GetComponent<Animator>(); 
            }
        }
    }

    void Initialization(int _num)
    {
        obj_players    = new GameObject[C_Global.MAX_PLAYER];
        am_animePlayer = new Animator[C_Global.MAX_PLAYER];
        attributeState = new _ATTRIBUTE_STATE[C_Global.MAX_PLAYER];
        actionState    = new _ACTION_STATE[C_Global.MAX_PLAYER];
        speed          = new float[C_Global.MAX_PLAYER];
        hp             = new float[C_Global.MAX_PLAYER];
        maxSpeed  = new float[C_Global.MAX_PLAYER];
        maxHp       = new float[C_Global.MAX_PLAYER];
        direction      = new Vector3[C_Global.MAX_PLAYER];
        direction2     = new Vector3[C_Global.MAX_PLAYER];

		// !!!!!!!!!!! 서버에서 받은데이터로 초기화해야함  임의로 속성값부여해둠. !!!!!!!!!!
		for (int i = 0; i < C_Global.MAX_PLAYER; i++)
		{
            if(myIndex == i) //내인덱스들의 초기화
            {
                attributeState[i] = _ATTRIBUTE_STATE.ALIVE;
                actionState[i] = _ACTION_STATE.IDLE;
                speed[i] = maxSpeed[i] = 4.0f;
                hp[i] = maxHp[i] = 100.0f;
            }
            else //나머지 3명 인덱스의 초기화
            {
                attributeState[i] = _ATTRIBUTE_STATE.ALIVE;
                actionState[i] = _ACTION_STATE.IDLE;
                speed[i] = maxSpeed[i] = 4.0f;
                hp[i] = maxHp[i] = 100.0f;
            }
        }
    }
    
    IEnumerator MovePlayer()
    {
        while (true)
        {
			NetworkManager.instance.MayIMove(obj_players[myIndex].transform.localPosition.x,
	            obj_players[myIndex].transform.localPosition.z,
	            obj_players[myIndex].transform.localEulerAngles.y, actionState[myIndex]);

            yield return YieldInstructionCache.WaitForSeconds(0.14f);
        }
    }

    public void StartMoveCoroutine()
    {
        Debug.Log(coroutineFlag);
        // 2개의 조이스틱이 있으므로 코루틴이 겹치는걸 방지 
        if (1 <= coroutineFlag)
        {
            coroutineFlag++;
            return;
        }

        StartCoroutine(coroutine);
        coroutineFlag++;
    }

    public void StopMoveCoroutine()
    {
        //Debug.Log(coroutineFlag);

        // 양쪽 조이스틱중 하나만 땠을 경우 플레그 카운트만 1줄여준다.
        if (1 < coroutineFlag)
        {
            coroutineFlag--;
            return;
        }

        // 코루틴 정지시에 마지막 위치를 보내준다.
        NetworkManager.instance.MayIMove(obj_players[myIndex].transform.localPosition.x,
            obj_players[myIndex].transform.localPosition.z,
            obj_players[myIndex].transform.localEulerAngles.y, actionState[myIndex]);

        StopCoroutine(coroutine);
        coroutineFlag--;
    }
}
