using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어의 기본속성이 들어있다.
 * 분할된 클래스 내 필드들의 Awake는 여기서 한번에 이루어진다.
 */

public partial class MyPlayerManager : MonoBehaviour
{
    public static MyPlayerManager instance;

    public GameObject obj_myPlayer { get; set; }

    public _ATTRIBUTE_STATE myAttributeState { get; set; }
    public float mySpeed { get; set; }
    public float myHp { get; set; }
    public Vector3 myDirection { get; set; } //플레이어의 방향
    public Vector3 myDirection2 { get; set; }

    void Awake()
    {
        if (instance == null)
            instance = this;

        obj_myPlayer = GameManager.instance.obj_players[GameManager.instance.myIndex];
        animePlayer = obj_myPlayer.GetComponent<Animator>();

        //전부 서버에서 받아온 데이터로
        myAttributeState = _ATTRIBUTE_STATE.ALIVE;
        myActionState = _ACTION_STATE.IDLE;
        mySpeed = 4.0f; 
        myHp = 100.0f; 
    }

    private void Start()
    {
        //Debug.Log(obj_myPlayer + "은또다른 내플레이어다.");
    }
}
