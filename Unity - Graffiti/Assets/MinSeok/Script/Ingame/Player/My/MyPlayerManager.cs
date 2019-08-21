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

    private bool moveFlag;
    public _ATTRIBUTE_STATE myAttributeState { get; set; }
    public float mySpeed { get; set; }
    public float myHp { get; set; }
    public Vector3 myDirection { get; set; } //플레이어의 방향
    public Vector3 myDirection2 { get; set; }

    void Awake()
    {
        moveFlag = false;

        if (instance == null)
            instance = this;

        obj_myPlayer = GameObject.FindGameObjectWithTag(GameManager.instance.myTag); //내 태그번호에맞는 로빈과합체
        animePlayer = obj_myPlayer.GetComponent<Animator>();

        //전부 서버에서 받아온 데이터로
        myAttributeState = _ATTRIBUTE_STATE.ALIVE;
        myActionState = _ACTION_STATE.IDLE;
        mySpeed = 4.0f; 
        myHp = 100.0f;

        StartCoroutine(MovePlayer());
    }

    IEnumerator MovePlayer()
    {
        while (true)
        {
            if (moveFlag == true)
            {
                NetworkManager.instance.MayIMove(obj_myPlayer.transform.position.x, obj_myPlayer.transform.position.z, obj_myPlayer.transform.localEulerAngles.y);
            }
            yield return YieldInstructionCache.WaitForSeconds(0.16f);
        }
    }

}
