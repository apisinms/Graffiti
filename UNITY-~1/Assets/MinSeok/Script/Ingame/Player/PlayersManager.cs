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
    IDLE   = 0, 
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
    private NetworkManager networkManager;  // 접근용
    public int myIndex { get; set; } //가독성을위해 하나 더만들어줌
    public int coroutineFlag { get; set; }
    private IEnumerator coroutine;

    #region PLAYERS_ROBIN
    public GameObject[] obj_players { get; set; }
    public Transform[] tf_players { get; set; } 
    #endregion

    #region PLAYERS_STATE 
    public _ACTION_STATE[] actionState { get; set; } //행동상태
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

#if NETWORK
      networkManager = NetworkManager.instance;
      coroutine = MovePlayer();
#endif

        //coroutine = MovePlayer();
        coroutineFlag = 0;
        myIndex = GameManager.instance.myIndex; //게임매니저에서 받은 인덱스를 다시등록
        Initialization(C_Global.MAX_PLAYER); //기타 초기화
 
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
                //obj_players[i].GetComponent<CapsuleCollider>().isTrigger = true;
                tf_players[i] = obj_players[i].GetComponent<Transform>();
                am_animePlayer[i] = obj_players[i].GetComponent<Animator>(); 
            }
        }

#if NETWORK
      //////////////// 게임 시작 시 최초로 1회 위치정보를 서버로 전송해야함 /////////////////
      networkManager.SendPosition(obj_players[myIndex].transform.localPosition.x,
           obj_players[myIndex].transform.localPosition.z,
           obj_players[myIndex].transform.localEulerAngles.y, speed[myIndex], actionState[myIndex], true);
#endif

#if NETWORK
      //////////////////////// 테스트용(상대팀 끄기) ////////////////////
      switch (myIndex)
        {
            case 0:
            case 1:
                obj_players[2].SetActive(false);
                obj_players[3].SetActive(false);
                break;

            case 2:
            case 3:
                obj_players[0].SetActive(false);
                obj_players[1].SetActive(false);
                break;
        }
#endif
    }

    void Initialization(int _num)
    {
        obj_players    = new GameObject[C_Global.MAX_PLAYER];
        tf_players      = new Transform[C_Global.MAX_PLAYER];
        am_animePlayer = new Animator[C_Global.MAX_PLAYER];
        speed          = new float[C_Global.MAX_PLAYER];
        hp             = new float[C_Global.MAX_PLAYER];
        maxSpeed  = new float[C_Global.MAX_PLAYER];
        maxHp       = new float[C_Global.MAX_PLAYER];
        direction      = new Vector3[C_Global.MAX_PLAYER];
        direction2     = new Vector3[C_Global.MAX_PLAYER];
        actionState = new _ACTION_STATE [C_Global.MAX_PLAYER];
        attributeState = new _ATTRIBUTE_STATE[C_Global.MAX_PLAYER];
        lastPosX = new float[C_Global.MAX_PLAYER];
        lastPosZ = new float[C_Global.MAX_PLAYER];

        //재생중이였던 이전 코루틴.
        curCor = null;

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
            NetworkManager.instance.SendPosition(tf_players[myIndex].localPosition.x,
                tf_players[myIndex].localPosition.z,
                tf_players[myIndex].localEulerAngles.y, speed[myIndex], actionState[myIndex]);

            yield return YieldInstructionCache.WaitForSeconds(C_Global.packetInterval);
        }
    }

    public void StartMoveCoroutine()
    {
        StartCoroutine(coroutine);
    }

    public void StopMoveCoroutine()
    {
        // 코루틴 정지시에 마지막 위치를 보내준다.
        networkManager.SendPosition(tf_players[myIndex].localPosition.x,
            tf_players[myIndex].localPosition.z,
            tf_players[myIndex].localEulerAngles.y, speed[myIndex], actionState[myIndex]);

        StopCoroutine(coroutine);
    }

    /*
    switch(_value) //트루면+ 펄스면-
    {
        case true:
            if (stateInfo[myIndex].isApply[dn_stateAndIndex[_state]] == false)
            {
                stateInfo[myIndex].actionState += (int)_state - 1;
                stateInfo[myIndex].isApply[dn_stateAndIndex[_state]] = true;
            }
            break;
        case false:
            if (stateInfo[myIndex].isApply[dn_stateAndIndex[_state]] == true)
            {
                stateInfo[myIndex].actionState -= (int)_state - 1;
                stateInfo[myIndex].isApply[dn_stateAndIndex[_state]] = false;
            }
            break;
    }
    */
}

