using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어의 기본속성이 들어있다.
 * 분할된 클래스 내 필드들의 Awake는 여기서 한번에 이루어진다.
 */

public partial class PlayersManager : MonoBehaviour
{
    public const int NUM = 4;

    public static PlayersManager instance;

    public GameObject[] obj_players { get; set; }
    public GameObject obj_myPlayer { get; set; }

    public _ACTION_STATE[] actionState { get; set; }
    public _ATTRIBUTE_STATE[] attributeState { get; set; }
    public float[] speed { get; set; }
    public float[] hp { get; set; }
    public Vector3[] direction { get; set; } //플레이어의 방향
    public Vector3[] direction2 { get; set; }

    public int myIndex { get; set; } //가독성을위해 하나 더만들어줌

    void Awake()
    {
        if (instance == null)
            instance = this;

        myIndex = GameManager.instance.myIndex; //게임매니저에서 받은 인덱스를 다시등록
        Initialization(NUM); //기타 초기화

        //내 인덱스번호에 맞는 로빈오브젝트와 합체. 태그도 함께등록
        obj_players[myIndex] = GameObject.FindGameObjectWithTag(GameManager.instance.myTag);
        am_animePlayer[myIndex] = obj_players[myIndex].GetComponent<Animator>();

        // 나를를 제외한 플레이어 오브젝트에 나머지 3명을 순서대로 등록. 애니메이터도.
        for (int i = 0; i < NUM; i++)
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
        obj_players = new GameObject[NUM];
        am_animePlayer = new Animator[NUM];
        attributeState = new _ATTRIBUTE_STATE[NUM];
        actionState = new _ACTION_STATE[NUM];
        speed = new float[NUM];
        hp = new float[NUM];
        direction = new Vector3[NUM];
        direction2 = new Vector3[NUM];

        // !!!!!!!!!!! 서버에서 받은데이터로 초기화해야함  임의로 속성값부여해둠. !!!!!!!!!!
        for (int i=0; i<NUM; i++)
        {
            if(myIndex == i) //내인덱스들의 초기화
            {
                attributeState[i] = _ATTRIBUTE_STATE.ALIVE;
                actionState[i] = _ACTION_STATE.IDLE;
                speed[i] = 4.0f;
                hp[i] = 100.0f;
            }
            else //나머지 3명 인덱스의 초기화
            {
                attributeState[i] = _ATTRIBUTE_STATE.ALIVE;
                actionState[i] = _ACTION_STATE.IDLE;
                speed[i] = 3.0f;
                hp[i] = 80.0f;
            }
        }
    }
}
