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
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

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
						case PROTOCOL.START_PROTOCOL:
							{
								switch (result)
								{
									case RESULT.MATCH_SUCCESS:

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
										isInGame = true;
										break;

									case RESULT.MATCH_FAIL:
										Debug.Log("매칭 실패!!");
										break;
								}
							}
							break;
					}
				}
				break;
		}
	}

	public void Disconnect()
	{
		bw.Close();
		br.Close();
		tcpClient.Close();
	}
}