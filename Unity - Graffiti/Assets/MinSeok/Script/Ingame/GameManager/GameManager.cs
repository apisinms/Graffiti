using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public readonly string[] playersTag = new string[C_Global.MAX_PLAYER];
    public int myNetworkNum { get; set; }
    public int myIndex { get; set; }
    public string myTag { get; set; }

    void Awake()
    {
		if (instance == null)
		{
			instance = this;

			// 스크린 가로모드 고정
			Screen.orientation = ScreenOrientation.AutoRotation;
			Screen.autorotateToPortrait = false;
			Screen.autorotateToPortraitUpsideDown = false;
			Screen.autorotateToLandscapeLeft = true;
			Screen.autorotateToLandscapeRight = true;
		}

        //obj_players = new GameObject[4];
	    playersTag[0] = "Player1"; playersTag[1] = "Player2";
	    playersTag[2] = "Player3"; playersTag[3] = "Player4";

        //myNetworkNum = NetworkManager.instance.MyPlayerNum;
        myNetworkNum = 1; //예시로 번호부여함.  서버에서 샌드된번호로 해야함.
        myIndex = myNetworkNum - 1;

        myTag = playersTag[myIndex]; //내 태그등록
    }
}
