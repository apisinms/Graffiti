using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어의 기본속성이 들어있다.
 * 분할된 클래스 내 필드들의 Awake는 여기서 한번에 이루어진다.
 */
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

        myIndex = GameManager.instance.myIndex; //게임매니저에서 받은 인덱스를 다시등록

#if NETWORK
        networkManager = NetworkManager.instance;
#endif

        Initialization(GameManager.instance.gameInfo.maxPlayer);

        //내 인덱스번호에 맞는 로빈오브젝트와 합체.
        obj_players[myIndex] = GameObject.FindGameObjectWithTag(GameManager.instance.myTag);
        tf_players[myIndex] = obj_players[myIndex].GetComponent<Transform>().transform; //최적화를위해 트랜스폼만 따로저장.
        am_animePlayer[myIndex] = obj_players[myIndex].GetComponent<Animator>();

        // 나를 제외한 플레이어 오브젝트에 나머지 3명을 순서대로 등록. 애니메이터도.
        for (int i = 0; i < GameManager.instance.gameInfo.maxPlayer; i++)
        {
            if (myIndex != i)
            {
                obj_players[i] = GameObject.FindGameObjectWithTag(GameManager.instance.playersTag[i]);
                tf_players[i] = obj_players[i].GetComponent<Transform>().transform;
                am_animePlayer[i] = obj_players[i].GetComponent<Animator>();
            }
        }
    }

    private void Start()
    {
#if NETWORK
        /*
      // 일단 전부 꺼주고 나중에 닉네임 받을 때 다시 켜준다.
      GameObject robin;
      for (int i = 0; i < C_Global.MAX_CHARACTER; i++)
      {
         robin = GameObject.FindGameObjectWithTag("Player" + (i + 1).ToString());

         if(robin != null)
         {
            UIManager.instance.OffPlayerUI(UIManager.instance.PlayerIndexToAbsoluteIndex(i));
            robin.SetActive(false);
         }
      }
      */
#else
        Initialization_GameInfo();
#endif
    }

    void Initialization(int _num) //클라이언트 자체에서 그냥 초기화. 어차피 서버에서 정보가 있으니까 이렇게함.
    {
        obj_players = new GameObject[_num];
        tf_players = new Transform[_num];
        am_animePlayer = new Animator[_num];
        nickname = new string[_num];
        speed = new float[_num];
        hp = new float[_num];
        direction = new Vector3[_num];
        direction2 = new Vector3[_num];
        actionState = new _ACTION_STATE[_num];
        attributeState = new _ATTRIBUTE_STATE[_num];
        lastPosX = new float[_num];
        lastPosZ = new float[_num];

        //재생중이였던 이전 코루틴.
        curCor = null;
    }

    public void Initialization_GameInfo()
    {
#if NETWORK
      maxSpeed = GameManager.instance.gameInfo.maxSpeed;
      maxHp = GameManager.instance.gameInfo.maxHealth;

      for (int i = 0; i < GameManager.instance.gameInfo.maxPlayer; i++)
      {
         attributeState[i] = _ATTRIBUTE_STATE.ALIVE;
         actionState[i] = _ACTION_STATE.IDLE;
         speed[i] = maxSpeed;
         hp[i] = maxHp;
      }
#else
        maxSpeed = 4.0f;
        maxHp = 100.0f;

        for (int i = 0; i < GameManager.instance.gameInfo.maxPlayer; i++)
        {
            attributeState[i] = _ATTRIBUTE_STATE.ALIVE;
            actionState[i] = _ACTION_STATE.IDLE;
            speed[i] = maxSpeed;
            hp[i] = maxHp;
        }
#endif
    }
}
