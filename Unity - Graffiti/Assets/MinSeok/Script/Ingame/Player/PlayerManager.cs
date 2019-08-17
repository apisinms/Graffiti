using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어의 기본속성이 들어있다.
 * 분할된 클래스 내 필드들의 Awake는 여기서 한번에 이루어진다.
 */
public enum _ATTRIBUTE_STATE
{
    DEAD = 0,
    ALIVE = 1
}

public partial class PlayerManager : UnityEngine.MonoBehaviour
{
    public static PlayerManager instance;

    public readonly string[] playerTag = new string[4];
    public _ATTRIBUTE_STATE myAttributeState { get; set; }
    public int myLocalNum { get; set; }
    public int myNetworkNum { get; set; }
    public float speed { get; set; }

    public Vector3 myDirection { get; set; } //플레이어의 방향


    float keyHorizontal;
    float keyVertical;

    void Awake()
    {
        if (instance == null)
            instance = this;

        playerTag[0] = "Player1"; playerTag[1] = "Player2";
        playerTag[2] = "Player3"; playerTag[3] = "Player4";

        animePlayer = gameObject.GetComponent<Animator>();
        myAttributeState = _ATTRIBUTE_STATE.ALIVE;
        myActionState = _ACTION_STATE.IDLE;
        speed = 4.0f;
    }

    private void Start()
    {
        switch (this.gameObject.tag)
        {
            case "Player1":
                PlayerManager.instance.myLocalNum = 1;
                break;
            case "Player2":
                PlayerManager.instance.myLocalNum = 2;
                break;
            case "Player3":
                PlayerManager.instance.myLocalNum = 3;
                break;
            case "Player4":
                PlayerManager.instance.myLocalNum = 4;
                break;
        }
    }

    /*
    private void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
   //         if (NetworkManager.instance.MyPlayerNum == myLocalNum)
  //          {

                keyHorizontal = Input.GetAxis("Horizontal");

                keyVertical = Input.GetAxis("Vertical");

                transform.Translate(Vector3.right * speed * Time.smoothDeltaTime * keyHorizontal, Space.World);
                transform.Translate(Vector3.forward * speed * Time.smoothDeltaTime * keyVertical, Space.World);

  //          }

        }
    }
    */

}
