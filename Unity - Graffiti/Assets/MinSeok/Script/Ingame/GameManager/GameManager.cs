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
    public GameObject[] obj_players { get; set; }
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

        obj_players = new GameObject[4];
	    playersTag[0] = "Player1"; playersTag[1] = "Player2";
	    playersTag[2] = "Player3"; playersTag[3] = "Player4";

        //myNetworkNum = NetworkManager.instance.MyPlayerNum; //예시로 번호부여함.  서버에서 샌드된번호로 해야함.
        myNetworkNum = 1; //예시로 번호부여함.  서버에서 샌드된번호로 해야함.
        myIndex = myNetworkNum - 1;

        switch (myNetworkNum) //서버로부터 받은 번호에따라서 
        {
            case 1:
                myTag = "Player1";
                obj_players[0] = GameObject.FindGameObjectWithTag(myTag);
                break;
            case 2:
                myTag = "Player2";
                obj_players[1] = GameObject.FindGameObjectWithTag(myTag);
                break;
            case 3:
                myTag = "Player3";
                obj_players[2] = GameObject.FindGameObjectWithTag(myTag);
                break;
            case 4:
                myTag = "Player4";
                obj_players[3] = GameObject.FindGameObjectWithTag(myTag);
                break;
        }

        // 자기번호를 제외한 플레이어 오브젝트에 나머지 3명을 등록.
        for (int i = 0; i < obj_players.Length; i++)
        {
            if (myIndex != i)
                obj_players[i] = GameObject.FindGameObjectWithTag(playersTag[i]);
        }
    
    }

    private void Start()
    {
        /*
        for (int i = 0; i < obj_players.Length; i++)
        {
            if(i != myIndex)
                Debug.Log(obj_players[i]);
            else
                Debug.Log(obj_players[i] + "는 나의 플레이어이다.");
        }*/
    }


}
