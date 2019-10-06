using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BridgeClientToServer : MonoBehaviour
{
    public static BridgeClientToServer instance;

    private NetworkManager networkManager;  // 접근용
    private PlayersManager playersManager; // 접근용
    public int myIndex { get; set; }

    private void Awake()
    {
        if (instance == null) // 이전 로그인씬에서 부터 가져옴
        {
            instance = this;
        }
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //파셜클래스 된애들을 생성자처럼 각각 나눠서 한번에 호출. Awake가 하나라서 이렇게함
        Initialization_Networking();
        Initialization_PacketCoroutine();
        //Initialization_PlayerViewer();
    }

	//기존 MoveManager의 업데이트 부분 호출
	private void Update()
    {
		if(playersManager != null)
			PlayerActionViewer();
    }

}
