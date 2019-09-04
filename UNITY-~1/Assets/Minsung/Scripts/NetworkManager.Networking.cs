using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using UnityEngine;


/// <summary>
/// NetworkManager_Networking.cs파일
/// 주로 네트워킹에 관련된 내용이 있다.
/// </summary>
public partial class NetworkManager : MonoBehaviour
{
    private PositionPacket tmpPosPacket = new PositionPacket();

    private void Awake()
	{
		if (instance == null)
		{
			instance = this;

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
			// 처음 서버와 연결하는 부분
			IPEndPoint serverEndPoint = new IPEndPoint(serverIP, serverPort);
			tcpClient.Connect(serverEndPoint);

			// 서버와 연결 성공시 이진 쓰기, 이진 읽기용 스트림 생성
			if (tcpClient.Connected)
			{
				Debug.Log("서버 접속 성공");
				instance.br = new BinaryReader(tcpClient.GetStream());
				instance.bw = new BinaryWriter(tcpClient.GetStream());

				queue = new Queue<C_Global.QueueInfo>();    // 처리되야할 작업들을 담을 큐 생성

				ThreadManager.GetInstance.Init();
			}
		}

		else
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		// 클라이언트가 연결되었고, 큐에 내용이 있다면 RecvProcess를 호출하여 recv에 관한 처리를 한다.
		if (tcpClient.Connected && queue.Count > 0)
			RecvProcess();
	}

    //// 프로그램이 종료될 때
    //private void OnApplicationQuit()
    //{
    //	Disconnect();
    //}
    private void RecvProcess()
	{
		// 큐에 저장된 패킷을 꺼내온다.
		C_Global.QueueInfo info = queue.Dequeue();

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

												// 패킹 및 전송
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
                                    Debug.Log(sysMsg);
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
                                                posPacket[tmpPosPacket.playerNum - 1] = tmpPosPacket;   // 해당 플레이어 위치에 저장
                                                                                                        //// 마스크 만들어서 어떤 플레이어가 같은 섹터에 있는지 확인하고, 오브젝트를 켜고 끔
                                                                                                        //byte bitMask = (byte)PLAYER_BIT.PLAYER_1;
                                                                                                        //for (int i = 0; i < C_Global.MAX_PLAYER; i++, bitMask >>= 1)
                                                                                                        //{
                                                                                                        //   // 오브젝트를 켜준다.
                                                                                                        //   if((playerBit & bitMask) > 0)
                                                                                                        //      playerObjects[i].SetActive(true);

                                                //   // 오브젝트를 꺼준다.
                                                //   else
                                                //      playerObjects[i].SetActive(false);
                                                //}
                                            }
                                        }
                                        break;

                                    // 섹터 입장 시
                                    case RESULT.ENTER_SECTOR:
                                        {
                                            lock (key)
                                            {
                                                UnPackPacket(info.packet, ref tmpPosPacket);
                                                PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].SetActive(true);   // 켜고
                                                posPacket[tmpPosPacket.playerNum - 1] = tmpPosPacket;   // 해당 플레이어 위치에 저장
                                            }
                                        }
                                        break;

                                    // 섹터 퇴장 시
                                    case RESULT.EXIT_SECTOR:
                                        {
                                            lock (key)
                                            {
                                                UnPackPacket(info.packet, ref tmpPosPacket);
                                                PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].SetActive(false);   // 끄고
                                                posPacket[tmpPosPacket.playerNum - 1] = tmpPosPacket;   // 저장해주는게 좋음
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

                                                // 마스크 만들어서 어떤 플레이어가 같은 섹터에 있는지 확인하고, 오브젝트를 켜고 끔
                                                byte bitMask = (byte)PLAYER_BIT.PLAYER_1;
                                                for (int i = 0; i < C_Global.MAX_PLAYER; i++, bitMask >>= 1)
                                                {
                                                    // 본인은 걍 건너 뜀
                                                    if ((myPlayerNum - 1) == i)
                                                        continue;

                                                    // 오브젝트를 켜준다.
                                                    if ((playerBit & bitMask) > 0)
                                                        PlayersManager.instance.obj_players[i].SetActive(true);

                                                    // 오브젝트를 꺼준다.
                                                    else
                                                        PlayersManager.instance.obj_players[i].SetActive(false);
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                            break;

                        // 끊김 프로토콜
                        case PROTOCOL.DISCONNECT_PROTOCOL:
                            {
                                lock (key)
                                {
                                    UnPackPacket(info.packet, out quitPlayerNum);
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
}