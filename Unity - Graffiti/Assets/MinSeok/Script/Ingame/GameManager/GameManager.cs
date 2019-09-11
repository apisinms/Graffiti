using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
	public CameraControl mainCamera;	// 메인 카메라
    public readonly string[] playersTag = new string[C_Global.MAX_PLAYER];
    public int myNetworkNum { get; set; }
    public int myIndex { get; set; }
    public string myTag { get; set; }

	void Awake()
	{
		if (instance == null)
			instance = this;

		//obj_players = new GameObject[4];
		playersTag[0] = "Player1"; playersTag[1] = "Player2";
		playersTag[2] = "Player3"; playersTag[3] = "Player4";

#if NETWORK
		myNetworkNum = NetworkManager.instance.MyPlayerNum;
		myIndex = myNetworkNum - 1;
#else
		//myNetworkNum = 1; //예시로 번호부여함.  서버에서 샌드된번호로 해야함.
#endif
		myTag = playersTag[myIndex]; //내 태그등록
	}

	private void Start()
	{
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>();
	}
}
