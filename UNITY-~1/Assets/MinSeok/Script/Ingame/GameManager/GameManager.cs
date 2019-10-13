using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public readonly string[] playersTag = new string[C_Global.MAX_PLAYER];
    public int myNetworkNum { get; set; }
    public int myIndex { get; set; }
    public string myTag { get; set; }
    public bool myFocus { get; set; }

    public struct _Game_Info
    {
        public int gameMode { get; set; }
        public int responeTime { get; set; }
        public int gameTime { get; set; }
    }
    public _Game_Info gameInfo;

    public CameraControl mainCamera;    // 메인 카메라

    //	private static bool isLeftDrag;
    //public static bool LeftTouch { get { return isLeftDrag; } }

    private void Start()
    {     
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>();
		BridgeClientToServer.instance.Initialization_PlayerViewer();	// 게임 시작시 플레이어 뷰어 셋팅
	}

    void Awake()
    {
        if (instance == null)
            instance = this;

        //obj_players = new GameObject[4];
        playersTag[0] = "Player1"; playersTag[1] = "Player2";
        playersTag[2] = "Player3"; playersTag[3] = "Player4";

        gameInfo.gameMode = 0;
        gameInfo.gameTime = 120;
        gameInfo.responeTime = 5;

#if NETWORK
        myNetworkNum = NetworkManager.instance.MyPlayerNum;
        myIndex = myNetworkNum - 1;
#else
        myNetworkNum = 1; //예시로 번호부여함.  서버에서 샌드된번호로 해야함.
        myIndex = myNetworkNum - 1;
#endif
        myTag = playersTag[myIndex]; //내 태그등록
        myFocus = true;             // 포커스 On

        myTag = playersTag[myIndex]; //내 태그등록
    }



    // focus말고 pause로 해야 조금 더 정확한 상황을 만들 수 있음
    private void OnApplicationPause(bool pause)
    {
        // 포커스 갖고 있던 상태에서 정지된 상태라면   패킷을 받을 수 없는 정지된 상태인 것
        if (pause == true)
        {
#if NETWORK
         // 포커스 갖고 있음
         if (myFocus == true)
         {
            NetworkManager.instance.MayIChangeFocus(false); // 포커스 잃음!(정지)
            myFocus = false;   // 포커스 off
         }
#endif
        }

        // 포커스 잃은 상태에서 정지가 풀렸다면 패킷을 받을 수 있는 정지 풀린 상태인 것
        else
        {
#if NETWORK
         // 포커스 잃었던 상태라면
         if (myFocus == false)
         {
            NetworkManager.instance.MayIChangeFocus(true); // 포커스 얻었다!(정지 풀림)
            myFocus = true;
         }
#endif
        }
    }

}
