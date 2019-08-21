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
    public readonly string[] playersTag = new string[4];

    public int myNetworkNum { get; set; }
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

	    playersTag[0] = "Player1"; playersTag[1] = "Player2";
	    playersTag[2] = "Player3"; playersTag[3] = "Player4";
        
        // 서버에서 받은 번호를 부여 
        myNetworkNum = NetworkManager.instance.MyPlayerNum;

        switch (myNetworkNum) //서버로부터 받은 번호에따라서 
        {
            case 1:
                myTag = "Player1";
                break;
            case 2:
                myTag = "Player2";
                break;
            case 3:
                myTag = "Player3";
                break;
            case 4:
                myTag = "Player4";
                break;
        }
        //GameObject.FindGameObjectWithTag(MyPlayerManager.instance.myTag).AddComponent<MyPlayerManager>(); //내가 부여받은 번호의 로빈과 합체.
    }



}
