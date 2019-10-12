using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using UnityEngine;
using static C_Global;


/// <summary>
/// NetworkManager_Networking.cs파일
/// 주로 네트워킹에 관련된 내용이 있다.
/// </summary>
public partial class NetworkManager : MonoBehaviour
{
	private BridgeClientToServer bridge;
	private PositionPacket tmpPosPacket = new PositionPacket();
	private Vector3 tmpVec = new Vector3();
	private Vector3 tmpAngle = new Vector3();


	private void Awake()
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

			// 60프레임 설정
			Application.targetFrameRate = 60;

			// 절전모드 안되게
			Screen.sleepTimeout = SleepTimeout.NeverSleep;

			//// 백그라운드에서도 실행되게
			//Application.runInBackground = true;

			// 처음 서버와 연결하는 부분
			IPEndPoint serverEndPoint = new IPEndPoint(serverIP, serverPort);
			tcpClient.Connect(serverEndPoint);

			// 서버와 연결 성공시 이진 쓰기, 이진 읽기용 스트림 생성
			if (tcpClient.Connected)
			{
				Debug.Log("서버 접속 성공");
				instance.br = new BinaryReader(tcpClient.GetStream());
				instance.bw = new BinaryWriter(tcpClient.GetStream());

				queue = new Queue<QueueInfo>();    // 처리되야할 작업들을 담을 큐 생성

				ThreadManager.GetInstance.Init();
			}
		}

		else
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		if (bridge == null)
			bridge = BridgeClientToServer.instance;
	}

	private void Update()
	{
		// 클라이언트가 연결되었고, 큐에 내용이 있다면 RecvProcess를 호출하여 recv에 관한 처리를 한다.
		if (tcpClient.Connected && queue.Count > 0)
			RecvProcess();

		// 인터넷이 끊기면 어플을 종료함(나중에 메시지박스 같은거 띄우고 확인버튼 누르면 종료시키면 될듯)
		if (Application.internetReachability == NetworkReachability.NotReachable)
			Application.Quit();
	}

	private void RecvProcess()
	{
		// 큐에 저장된 패킷을 꺼내온다.
		QueueInfo info = queue.Dequeue();

		// 얻어온 패킷으로 state, protocol, result를 각각 추출한다.
		GetProtocol(info.packet, out state, out protocol, out result);

		// 얻어온 정보를 switch하여 처리한다.
		switch (state)
		{
			// Login 상태일 때
			case STATE_PROTOCOL.LOGIN_STATE:
				{
					// Login 상태에서 프로토콜은 제외했다.(같은 값이 존재해서)


					switch (result)
					{
						case RESULT.LOGIN_SUCCESS:
						//case RESULT.JOIN_SUCCESS:
						//case RESULT.LOGOUT_SUCCESS:
						case RESULT.ID_EXIST:
						//case RESULT.LOGOUT_FAIL:
						case RESULT.ID_ERROR:
						case RESULT.PW_ERROR:
							{
								lock (key)
								{
									sysMsg = string.Empty;
									UnPackPacket(info.packet, out sysMsg);
									Debug.Log(sysMsg);
								}
							}
							break;
					}
				}
				break;

			// Lobby 상태일 때
			case STATE_PROTOCOL.LOBBY_STATE:
				{
					switch (protocol)
					{
						case PROTOCOL.GOTO_INGAME_PROTOCOL:
							{
								switch (result)
								{
									case RESULT.LOBBY_SUCCESS:
										{
											lock (key)
											{
												UnPackPacket(info.packet, out myPlayerNum);

												Debug.Log(myPlayerNum);

												// 클라가 매칭 성공을 수신했다라는 프로토콜 셋팅
												PROTOCOL startProtocol = SetProtocol(
													  STATE_PROTOCOL.LOBBY_STATE,
													  PROTOCOL.START_PROTOCOL,
													  RESULT.NODATA);

												// 시작 프로토콜 + 플레이어 번호 패킹 및 전송
												int packetSize;
												PackPacket(ref sendBuf, startProtocol, out packetSize);
												bw.Write(sendBuf, 0, packetSize);

												Debug.Log("매칭 성공!!");
											}
										}
										break;

									case RESULT.LOBBY_FAIL:
										Debug.Log("매칭 실패!!");
										break;
								}
							}
							break;

						case PROTOCOL.MATCH_CANCEL_PROTOCOL:
							{
								switch (result)
								{
									case RESULT.LOBBY_SUCCESS:
										Debug.Log("매칭 취소 성공!");
										break;

									case RESULT.LOBBY_FAIL:
										Debug.Log("매칭 취소 실패!");
										break;
								}
							}
							break;
					}
				}
				break;

			case STATE_PROTOCOL.INGAME_STATE:
				{
					switch (protocol)
					{
						// 타이머 프로토콜이 넘겨져오면
						case PROTOCOL.TIMER_PROTOCOL:
							{
								// result 생략

								// 넘겨온 초를 string으로 변환해서 sysMsg에 저장한다.
								lock (key)
								{
									sysMsg = string.Empty;

									int sec = 0;
									UnPackPacket(info.packet, out sec);
									sysMsg = sec.ToString() + "초";
								}
							}
							break;

						// (자신포함)상대방이 자신의 총 정보를 넘겨주면 그걸로 셋팅함
						case PROTOCOL.WEAPON_PROTOCOL:
							{
								switch (result)
								{
									case RESULT.NOTIFY_WEAPON:
										{
											lock (key)
											{
												int playerNum;
												WeaponPacket weapon = new WeaponPacket();

												UnPackPacket(info.packet, out playerNum, ref weapon);

												bridge.SetWeapon(playerNum, ref weapon);
											}
										}
										break;
								}
							}
							break;

						// 게임 시작 프로토콜
						case PROTOCOL.START_PROTOCOL:
							break;

						// 움직임 프로토콜
						case PROTOCOL.MOVE_PROTOCOL:
							{
								switch (result)
								{
									// 정상 이동 시
									case RESULT.INGAME_SUCCESS:
										{
											lock (key)
											{
												UnPackPacket(info.packet, ref tmpPosPacket);

												bridge.OnMoveSuccess(ref tmpPosPacket);
											}
										}
										break;

									// 다른 플레이어가 섹터 입장 시
									case RESULT.ENTER_SECTOR:
										{
											lock (key)
											{
												UnPackPacket(info.packet, ref tmpPosPacket);

												bridge.EnterSectorProcess(ref tmpPosPacket);
											}
										}
										break;

									// 다른 플레이어가 섹터 퇴장 시
									case RESULT.EXIT_SECTOR:
										{
											lock (key)
											{
												UnPackPacket(info.packet, ref tmpPosPacket);    // 사실 이 부분도 int 하나만 갖고도 될 일

												bridge.ExitSectorProcess(ref tmpPosPacket);
											}
										}
										break;

									// 섹터 입장 시 새로 입장한 인접섹터에 있는 플레이어 리스트 갱신
									case RESULT.UPDATE_PLAYER:
										{
											lock (key)
											{
												byte playerBit = 0;
												UnPackPacket(info.packet, out playerBit);

												bridge.UpdatePlayerProcess(playerBit);
											}
										}
										break;

									case RESULT.FORCE_MOVE:
										{
											lock (key)
											{
												UnPackPacket(info.packet, ref tmpPosPacket);    // 포지션 패킷 가져옴

												bridge.ForceMoveProcess(ref tmpPosPacket);
											}
										}
										break;

									case RESULT.GET_OTHERPLAYER_POS:
										{
											lock (key)
											{
												UnPackPacket(info.packet, ref tmpPosPacket);            // 패킷을 받고

												bridge.GetOtherPlayerPos(ref tmpPosPacket);
											}
										}
										break;
								}
							}
							break;

						// 포커스 프로토콜(추후에 게임 정보가 추가된다면, 위치정보 뿐만 아니라 체력이나 기타 정보도 업데이트 해줘야한다!)
						case PROTOCOL.FOCUS_PROTOCOL:
							{
								// result 생략

								lock (key)
								{
									UnPackPacket(info.packet, ref tmpPosPacket);         // 포지션 패킷 가져옴

									bridge.ForceMoveProcess(ref tmpPosPacket);
								}
							}
							break;

						// 다른사람 끊김 프로토콜
						case PROTOCOL.DISCONNECT_PROTOCOL:
							{
								lock (key)
								{
									// 끊긴 놈을 꺼준다.
									UnPackPacket(info.packet, out quitPlayerNum);

									bridge.OnOtherPlayerDisconnected(quitPlayerNum);
								}
							}
							break;
					}
				}
				break;
		}
	}

	private void OnApplicationQuit()
	{
		Disconnect();
	}

	public void Disconnect()
	{
		// 뒷정리
		bw.Close();
		br.Close();
		tcpClient.Close();
	}

	// 다른 플레이어 위치 얻기 요청
	public void RequestOtherPlayerPos(int _playerNumber)
	{
		int packetSize = 0;

		// 다른 클라의 위치 요청 프로토콜
		PROTOCOL reqProtocol = SetProtocol(
			  STATE_PROTOCOL.INGAME_STATE,
			  PROTOCOL.MOVE_PROTOCOL,
			  RESULT.GET_OTHERPLAYER_POS);

		// 플레이어라면 위치를 요청한다.
		PackPacket(ref sendBuf, reqProtocol, _playerNumber, out packetSize);
		bw.Write(sendBuf, 0, packetSize);
	}
}