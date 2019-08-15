using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어의 기본속성이 들어있다.
 * 분할된 클래스 내 필드들의 Awake는 여기서 한번에 이루어진다.
 */

public partial class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public readonly string[] playerTag = new string[4];
    public int myLocalNum { get; set; }
    public int myNetworkNum { get; set; }
    public float speed { get; set; }

    public Vector3 myDirection { get; set; } //플레이어의 방향
    public Vector3 tmp { get; set; }

    void Awake()
    {
        if (instance == null)
            instance = this;

        playerTag[0] = "Player1"; playerTag[1] = "Player2";
        playerTag[2] = "Player3"; playerTag[3] = "Player3";

        am_playerMovement = gameObject.GetComponent<Animator>();
        moveFlag = false;
        speed = 4.0f;
    }

    void Start()
    {

    }

}
