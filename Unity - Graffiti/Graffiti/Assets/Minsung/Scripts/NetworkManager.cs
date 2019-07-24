﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using UnityEngine;


/// <summary>
/// 싱글톤 클래스 NetworkManager
/// </summary>
public class NetworkManager : MonoBehaviour
{
	const int IDSIZE = 255;
	const int PWSIZE = 255;
	const int NICKNAMESIZE = 255;
	const int BUFSIZE = 4096;   // 버퍼 사이즈

	// 63 ~ 59 
	enum STATE_PROTOCOL : Int64
	{
		// 상위 5비트 스테이트를 표현해주는 프로토콜
		LOGIN_STATE = ((Int64)0x1 << 63),
		LOBBY_STATE = ((Int64)0x1 << 62),
		CHAT_STATE = ((Int64)0x1 << 61),
		//60
		//59
	};

	//58 ~ 54
	enum PROTOCOL : Int64
	{
		// LoginState
		JOIN_PROTOCOL = ((Int64)0x1 << 58),
		LOGIN_PROTOCOL = ((Int64)0x1 << 57),
		// 56
		// 55
		// 54

		// LobbyState
		MATCH_PROTOCOL = ((Int64)0x1 << 58),
		START_PROTOCOL = ((Int64)0x1 << 57),      // 게임시작 프로토콜
		LOGOUT_PROTOCOL = ((Int64)0x1 << 56),

		// ChatState
		LEAVE_ROOM_PROTOCOL = ((Int64)0x1 << 58),
		CHAT_PROTOCOL = ((Int64)0x1 << 57),
		// 56
		// 55
		// 54
	};

	// 53 ~ 49
	enum RESULT : Int64
	{
		//LoginState
		JOIN_SUCCESS = ((Int64)0x1 << 53),
		LOGIN_SUCCESS = ((Int64)0x1 << 53),
		LOGOUT_SUCCESS = ((Int64)0x1 << 53),
		LOGOUT_FAIL = ((Int64)0x1 << 52),

		// Join & Login result
		ID_EXIST = ((Int64)0x1 << 52),
		ID_ERROR = ((Int64)0x1 << 51),
		PW_ERROR = ((Int64)0x1 << 50),

		// LobbyState
		MATCH_SUCCESS = ((Int64)0x1 << 53), // 매칭 성공
		MATCH_FAIL = ((Int64)0x1 << 52),        // 매칭 성공

		// ChatState
		LEAVE_ROOM_SUCCESS = ((Int64)0x1 << 53),
		LEAVE_ROOM_FAIL = ((Int64)0x1 << 52),


		NODATA = ((Int64)0x1 << 49)
	};

	// 큐에 들어갈 정보
	struct Info
	{
		public STATE_PROTOCOL	state;
		public PROTOCOL			protocol;
		public RESULT			result;

		public Info(STATE_PROTOCOL _state, PROTOCOL _protocol, RESULT _result)
		{
			state = _state;
			protocol = _protocol;
			result = _result;
		}
	}

	struct _User_Info
	{
		public string id;
		public string pw;
		public string nickname;
	};

	STATE_PROTOCOL state;   // 클라 상태
	PROTOCOL protocol;      // 프로토콜
	RESULT result;          // 결과

	// 서버 IP와 포트
	private static IPAddress serverIP = IPAddress.Parse("127.0.0.1");
	private static int serverPort = 9000;

	// 버퍼
	private byte[] sendBuf = new byte[BUFSIZE];				// 송신 버퍼
	private byte[] recvBuf = new byte[BUFSIZE];				// 수신 버퍼
	private byte[] recvDecryptBuf = new byte[BUFSIZE];		// 수신 복호화 버퍼

	// TCP 클라, 바이너리 리더, 라이터
	private static TcpClient tcpClient = new TcpClient();
	private BinaryReader br = null;
	private BinaryWriter bw = null;

	private object key = new object();      // 동기화에 사용할 key이다.
	private bool recvFlag = false;          // 모두 recv했는지 나타내는 플래그
	private string msg = string.Empty;  // 서버로부터 전달되는 메시지를 저장할 변수

	private Thread thread = null;


	private Queue<Info> queue = new Queue<Info>();

	public static NetworkManager instance = null;

	private void Start()
	{
		if (instance == null)
		{
			//instance = new NetworkManager();
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

				// 수신전용 쓰레드 생성 및 실행
				//thread = new Thread(new ThreadStart(instance.RecvThread));
				//thread.Start();
				thread = new Thread(RecvThread);
				thread.Start();     //thread 시작 

				// 코루틴 쓰레드 생성 및 실행
				//instance.StartCoroutine(instance.RecvCoroutine());
			}
		}

		else
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	private string ByteToString(byte[] _byte)
	{
		string str = Encoding.Unicode.GetString(_byte);
		return str;
	}

	private byte[] StringToByte(string _str)
	{
		byte[] strByte = Encoding.Unicode.GetBytes(_str);
		return strByte;
	}

	private void GetProtocol(byte[] _buf, out STATE_PROTOCOL _state, out PROTOCOL _protocol, out RESULT _result)
	{
		_state = 0;
		_protocol = 0;
		_result = 0;

		int offset = 0;

		/// BlockCopy를 해서 얻어올때 BitConverter.GetBytes로 얻어오면 byte형식으로 올바르게 못얻어옴, 따라서 byte배열을 선언한 뒤에 거기에다가 저장을 시켜야 함!
		// 통짜로 프로토콜 다 받아와서
		byte[] byteProtocol = new byte[sizeof(PROTOCOL)];   // 1. byte형식으로 얻어올 배열
		PROTOCOL wholeProtocol = 0;                         // 2. 64비트로 가지고있을 Protocol 자체
		Buffer.BlockCopy(_buf, offset, byteProtocol, 0, sizeof(PROTOCOL));  // 1. 우선 byte형식으로 얻음
		wholeProtocol = (PROTOCOL)BitConverter.ToInt64(byteProtocol, 0);    // 2. 위에서 얻은 배열로 Int64를 만들어 Protocol에 저장

		offset += sizeof(PROTOCOL);         // 오프셋 증가

		// 상위 5비트를 걸러줄 마스크를 생성하고
		Int64 mask = ((Int64)0x1f << (64 - 5));

		// 마스크를 통해 스테이트를 얻는다.
		_state = (STATE_PROTOCOL)((Int64)wholeProtocol & mask);

		// state를 읽어서 protocol과 resul를 마스킹을 통해 구한다.
		switch (_state)
		{
			case STATE_PROTOCOL.LOGIN_STATE:
				mask = ((Int64)0x1f << (64 - 10));
				_protocol = (PROTOCOL)((Int64)wholeProtocol & mask);  // 이제 state를 얻었으니 protocol을 얻어보자

				mask = ((Int64)0x1f << (64 - 15));
				_result = (RESULT)((Int64)wholeProtocol & mask);     // 이제 마지막으로 result
				break;

			case STATE_PROTOCOL.LOBBY_STATE:
				mask = ((Int64)0x1f << (64 - 10));
				_protocol = (PROTOCOL)((Int64)wholeProtocol & mask);  // 이제 state를 얻었으니 protocol을 얻어보자

				mask = ((Int64)0x1f << (64 - 15));
				_result = (RESULT)((Int64)wholeProtocol & mask);       // 이제 마지막으로 result
				break;

			case STATE_PROTOCOL.CHAT_STATE:
				break;
		}

	}

	private PROTOCOL SetProtocol(STATE_PROTOCOL _state, PROTOCOL _protocol, RESULT _result)
	{
		PROTOCOL protocol = (PROTOCOL)0;
		protocol = (PROTOCOL)((Int64)_state | (Int64)_protocol | (Int64)_result);
		return protocol;
	}

	private void PackPacket(ref byte[] _buf, PROTOCOL _protocol, out int _size)
	{
		// 암호화된 내용을 저장할 버퍼이다.
		byte[] encryptBuf = new byte[BUFSIZE];
		byte[] buf = new byte[BUFSIZE];

		_size = 0;
		int offset = 0;

		// 프로토콜
		Buffer.BlockCopy(BitConverter.GetBytes((Int64)_protocol), 0, buf, offset, sizeof(PROTOCOL));
		offset += sizeof(PROTOCOL);
		_size += sizeof(PROTOCOL);

		// 암호화된 내용을 encryptBuf에 저장
		C_Encrypt.GetInstance.Encrypt(buf, encryptBuf, _size);

		// 가장 앞에 size를 넣고, 그 뒤에 암호화했던 버퍼를 붙임.
		offset = 0;
		Buffer.BlockCopy(BitConverter.GetBytes(_size), 0, _buf, offset, sizeof(int));
		offset += sizeof(int);
		Buffer.BlockCopy(encryptBuf, 0, _buf, offset, _size);
		offset += _size;

		_size += sizeof(int);   // 총 보내야 할 바이트 수 저장한다.
	}

	private void PackPacket(ref byte[] _buf, PROTOCOL _protocol, string _str1, string _str2, out int _size)
	{
		int strsize1 = _str1.Length * sizeof(char);
		int strsize2 = _str2.Length * sizeof(char);

		// 암호화된 내용을 저장할 버퍼이다.
		byte[] encryptBuf = new byte[BUFSIZE];
		byte[] buf = new byte[BUFSIZE];

		_size = 0;
		int offset = 0;

		// 프로토콜
		Buffer.BlockCopy(BitConverter.GetBytes((Int64)_protocol), 0, buf, offset, sizeof(PROTOCOL));
		offset += sizeof(PROTOCOL);
		_size += sizeof(PROTOCOL);

		// 문자열1 길이
		Buffer.BlockCopy(BitConverter.GetBytes(strsize1), 0, buf, offset, sizeof(int));
		offset += sizeof(int);
		_size += sizeof(int);

		// 문자열1
		Buffer.BlockCopy(StringToByte(_str1), 0, buf, offset, strsize1);
		offset += strsize1;
		_size += strsize1;

		// 문자열2 길이
		Buffer.BlockCopy(BitConverter.GetBytes(strsize2), 0, buf, offset, sizeof(int));
		offset += sizeof(int);
		_size += sizeof(int);

		// 문자열2
		Buffer.BlockCopy(StringToByte(_str2), 0, buf, offset, strsize2);
		offset += strsize2;
		_size += strsize2;

		// 암호화된 내용을 encryptBuf에 저장
		C_Encrypt.GetInstance.Encrypt(buf, encryptBuf, _size);

		// 가장 앞에 size를 넣고, 그 뒤에 암호화했던 버퍼를 붙임.
		offset = 0;
		Buffer.BlockCopy(BitConverter.GetBytes(_size), 0, _buf, offset, sizeof(int));
		offset += sizeof(int);
		Buffer.BlockCopy(encryptBuf, 0, _buf, offset, _size);
		offset += _size;

		_size += sizeof(int);   // 총 보내야 할 바이트 수 저장한다.
	}
	private void PackPacket(ref byte[] _buf, PROTOCOL _protocol, string _str1, string _str2, string _str3, out int _size)
	{
		int strsize1 = _str1.Length * sizeof(char);
		int strsize2 = _str2.Length * sizeof(char);
		int strsize3 = _str3.Length * sizeof(char);

		// 암호화된 내용을 저장할 버퍼이다.
		byte[] encryptBuf = new byte[BUFSIZE];
		byte[] buf = new byte[BUFSIZE];

		_size = 0;
		int offset = 0;

		// 프로토콜
		Buffer.BlockCopy(BitConverter.GetBytes((Int64)_protocol), 0, buf, offset, sizeof(PROTOCOL));
		offset += sizeof(PROTOCOL);
		_size += sizeof(PROTOCOL);

		// 문자열1 길이
		Buffer.BlockCopy(BitConverter.GetBytes(strsize1), 0, buf, offset, sizeof(int));
		offset += sizeof(int);
		_size += sizeof(int);

		// 문자열1
		Buffer.BlockCopy(StringToByte(_str1), 0, buf, offset, strsize1);
		offset += strsize1;
		_size += strsize1;

		// 문자열2 길이
		Buffer.BlockCopy(BitConverter.GetBytes(strsize2), 0, buf, offset, sizeof(int));
		offset += sizeof(int);
		_size += sizeof(int);

		// 문자열2
		Buffer.BlockCopy(StringToByte(_str2), 0, buf, offset, strsize2);
		offset += strsize2;
		_size += strsize2;

		// 문자열3 길이
		Buffer.BlockCopy(BitConverter.GetBytes(strsize3), 0, buf, offset, sizeof(int));
		offset += sizeof(int);
		_size += sizeof(int);

		// 문자열3
		Buffer.BlockCopy(StringToByte(_str3), 0, buf, offset, strsize3);
		offset += strsize3;
		_size += strsize3;

		// 암호화된 내용을 encryptBuf에 저장
		C_Encrypt.GetInstance.Encrypt(buf, encryptBuf, _size);

		// 가장 앞에 size를 넣고, 그 뒤에 암호화했던 버퍼를 붙임.
		offset = 0;
		Buffer.BlockCopy(BitConverter.GetBytes(_size), 0, _buf, offset, sizeof(int));
		offset += sizeof(int);
		Buffer.BlockCopy(encryptBuf, 0, _buf, offset, _size);
		offset += _size;

		_size += sizeof(int);   // 총 보내야 할 바이트 수 저장한다.
	}
	private void UnPackPacket(byte[] _buf, out string _str1)
	{
		byte[] arrStrsize1 = new byte[sizeof(int)];
		int strsize1 = 0;

		int offset = sizeof(PROTOCOL);

		// 문자열 길이
		Buffer.BlockCopy(_buf, offset, arrStrsize1, 0, sizeof(int));
		offset += sizeof(int);
		strsize1 = BitConverter.ToInt32(arrStrsize1, 0);

		// 문자열 길이 만큼 생성해서 문자열을 저장함
		byte[] arrStrByte = new byte[strsize1];
		Buffer.BlockCopy(_buf, offset, arrStrByte, 0, strsize1);
		offset += strsize1;

		// 이제 다시 string으로 변환
		_str1 = ByteToString(arrStrByte);
	}



	public string MayILogin(string _id, string _pw)
	{
		// 프로토콜 셋팅
		protocol = SetProtocol(
				STATE_PROTOCOL.LOGIN_STATE,
				PROTOCOL.LOGIN_PROTOCOL,
				RESULT.NODATA);
		Console.WriteLine((Int64)protocol);

		// 패킹 및 전송
		int packetSize;
		PackPacket(ref sendBuf, protocol, _id, _pw, out packetSize);
		bw.Write(sendBuf, 0, packetSize);

		// 코루틴을 통해서 Recv를 하고, 다 받으면 다시 플래그를 초기화 해준다.
		while (recvFlag == false) ;
		recvFlag = false;

		// 받은 메시지 리턴
		return msg;
	}

	public bool CheckLoginSuccess()
	{
		if (result == RESULT.LOGIN_SUCCESS)
			return true;
		else
			return false;
	}
	public bool CheckIDError()
	{
		if (result == RESULT.ID_ERROR)
			return true;
		else
			return false;
	}
	public bool CheckPWError()
	{
		if (result == RESULT.PW_ERROR)
			return true;
		else
			return false;
	}


	public string MayIJoin(string _id, string _pw, string _nickname)
	{
		// 프로토콜 셋팅
		protocol = SetProtocol(
				STATE_PROTOCOL.LOGIN_STATE,
				PROTOCOL.JOIN_PROTOCOL,
				RESULT.NODATA);
		Console.WriteLine((Int64)protocol);

		// 패킹 및 전송
		int packetSize;
		PackPacket(ref sendBuf, protocol, _id, _pw, _nickname, out packetSize);
		bw.Write(sendBuf, 0, packetSize);


		// 코루틴을 통해서 Recv를 하고, 다 받으면 다시 플래그를 초기화 해준다.
		while (recvFlag == false) ;
		recvFlag = false;

		// 받은 메시지 리턴
		return msg;
	}

	public bool CheckJoinSuccess()
	{
		if (result == RESULT.JOIN_SUCCESS)
			return true;
		else
			return false;
	}
	public bool CheckIDExit()
	{
		if (result == RESULT.ID_EXIST)
			return true;
		else
			return false;
	}

	public bool MayIMatch()
	{
		// 프로토콜 셋팅
		protocol = SetProtocol(
				STATE_PROTOCOL.LOBBY_STATE,
				PROTOCOL.MATCH_PROTOCOL,
				RESULT.NODATA);
		Console.WriteLine((Int64)protocol);

		// 패킹 및 전송
		int packetSize;
		PackPacket(ref sendBuf, protocol, out packetSize);
		bw.Write(sendBuf, 0, packetSize);


		// 서버로부터 결과 얻기
		int readSize = br.ReadInt32();      // 먼저 총 size를 얻어온다.
		recvBuf = br.ReadBytes(readSize);   // 그리고 받아야할 size만큼 byte[]를 얻어온다.


		// 얻은 버퍼를 복호화 진행
		byte[] decryptBuf = new byte[BUFSIZE];
		C_Encrypt.GetInstance.Decrypt(recvBuf, decryptBuf, readSize);

		GetProtocol(decryptBuf, out state, out protocol, out result);

		if (result == RESULT.MATCH_SUCCESS)
			return true;

		else
			return false;
	}

	private IEnumerator RecvCoroutine()
	{
		int readSize = 0;
		byte[] decryptBuf = new byte[BUFSIZE];
		while (true)
		{
			// 서버로부터 결과 얻기
			readSize = br.ReadInt32();      // 먼저 총 size를 얻어온다.
			recvBuf = br.ReadBytes(readSize);   // 그리고 받아야할 size만큼 byte[]를 얻어온다.

			yield return null;

			// 얻은 버퍼를 복호화 진행
			C_Encrypt.GetInstance.Decrypt(recvBuf, decryptBuf, readSize);

			GetProtocol(decryptBuf, out state, out protocol, out result);
			switch (state)
			{
				// Login 상태일 때
				case STATE_PROTOCOL.LOGIN_STATE:
					{
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
									Monitor.Enter(key);

									string msg = string.Empty;
									UnPackPacket(decryptBuf, out msg);
									Debug.Log(msg);
									recvFlag = true;    // 수신 후 모든 처리 완료

									Monitor.Exit(key);
								}
								break;
						}
					}
					break;

				// Lobby 상태일 때
				case STATE_PROTOCOL.LOBBY_STATE:
					{

					}
					break;

			}
		}
	}


	private void RecvThread()
	{
		int readSize = 0;
		//while (true)
		//{
		// 서버로부터 결과 얻기
		readSize = br.ReadInt32();      // 먼저 총 size를 얻어온다.
		recvBuf = br.ReadBytes(readSize);   // 그리고 받아야할 size만큼 byte[]를 얻어온다.

		// 얻은 버퍼를 복호화 진행
		C_Encrypt.GetInstance.Decrypt(recvBuf, recvDecryptBuf, readSize);

		// 상태, 프로토콜, 결과를 얻음
		GetProtocol(recvDecryptBuf, out state, out protocol, out result);

		// 이를 큐에 저장
		Info info = new Info(state, protocol, result);
		queue.Enqueue(info);

		Debug.Log(Thread.CurrentThread);

		/// 만약 종료 프로토콜같은게 들어왔다면 break한다.
		//}
	}
	private void Update()
	{
		Debug.Log(Thread.CurrentThread);

		if (queue.Count > 0)
		{
			Info info = queue.Dequeue();

			switch (info.state)
			{
				// Login 상태일 때
				case STATE_PROTOCOL.LOGIN_STATE:
					{
						// 프로토콜은 제외했다.


						switch (info.result)
						{
							case RESULT.LOGIN_SUCCESS:
							//case RESULT.JOIN_SUCCESS:
							//case RESULT.LOGOUT_SUCCESS:
							case RESULT.ID_EXIST:
							//case RESULT.LOGOUT_FAIL:
							case RESULT.ID_ERROR:
							case RESULT.PW_ERROR:
								{
									Monitor.Enter(key);


									string msg = string.Empty;
									UnPackPacket(recvDecryptBuf, out msg);
									Debug.Log(msg);
									recvFlag = true;    // 수신 후 모든 처리 완료

									Monitor.Exit(key);
								}
								break;
						}
					}
					break;

				// Lobby 상태일 때
				case STATE_PROTOCOL.LOBBY_STATE:
					{

					}
					break;

			}

			thread.Start();
		}
	}


	public void Disconnect()
	{
		bw.Close();
		br.Close();
		tcpClient.Close();
	}
}