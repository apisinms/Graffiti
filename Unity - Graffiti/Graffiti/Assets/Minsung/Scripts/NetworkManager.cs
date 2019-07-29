using System;
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
		public STATE_PROTOCOL state;
		public PROTOCOL protocol;
		public RESULT result;

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
	private byte[] sendBuf = new byte[C_Global.BUFSIZE];             // 송신 버퍼
	private byte[] recvBuf = new byte[C_Global.BUFSIZE];             // 수신 버퍼

	// TCP 클라, 바이너리 리더, 라이터
	private TcpClient tcpClient = new TcpClient();
	private BinaryReader br = null;
	private BinaryWriter bw = null;

	private readonly object key = new object();      // 동기화에 사용할 key이다.
	public bool recvFlag = false;
	private string msg = string.Empty;  // 서버로부터 전달되는 메시지를 저장할 변수

	private Queue<C_Global.QueueInfo> queue = null;

	private static NetworkManager instance = null;



	private static EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
	

	public static NetworkManager GetInstance
	{
		get
		{
			return instance;
		}
	}

	public BinaryReader GetBinaryReader { get { return br; } }
	public BinaryWriter GetBinaryWriter { get { return bw; } }


	public Queue<C_Global.QueueInfo> GetQueue { get { return queue; } }

	//public static NetworkManager GetInstance
	//{
	//	get
	//	{
	//		if (instance == null)
	//		{
	//			instance = FindObjectOfType<NetworkManager>();

	//			if (instance == null)
	//				Debug.LogError("네트워크 매니저가 존재하지 않음.");




	//		}

	//		//스트림 생성 안됐을 때
	//		if(	instance != null && 
	//			instance.bw == null || instance.br == null)
	//		{
	//			instance.bw = new BinaryWriter(tcpClient.GetStream());
	//			instance.br = new BinaryReader(tcpClient.GetStream());

	//			instance.thread = new Thread(new ThreadStart(instance.RecvThread));
	//			instance.thread.Start();
	//		}


	//		return instance;
	//	}
	//}

	private void Awake()
	{
		instance = this;

		// 처음 서버와 연결하는 부분
		IPEndPoint serverEndPoint = new IPEndPoint(serverIP, serverPort);
		instance.tcpClient.Connect(serverEndPoint);

		// 서버와 연결 성공시 이진 쓰기, 이진 읽기용 스트림 생성
		if (tcpClient.Connected)
		{
			Debug.Log("서버 접속 성공");
			instance.br = new BinaryReader(instance.tcpClient.GetStream());
			instance.bw = new BinaryWriter(instance.tcpClient.GetStream());

			queue = new Queue<C_Global.QueueInfo>();

			// 쓰레드도 돌린다.
			ThreadManager.GetInstance.Init();
		}


		DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		if (tcpClient.Connected == true &&
			queue != null)
		{
			RecvProcess();
		}
	}

	private void RecvProcess()
	{
		if (queue.Count > 0)
		{
			C_Global.QueueInfo info = queue.Dequeue();
			byte[] packet = info.GetPacket;

			// 상태, 프로토콜, 결과를 얻음
			GetProtocol(packet, out state, out protocol, out result);

			switch (state)
			{
				// Login 상태일 때
				case STATE_PROTOCOL.LOGIN_STATE:
					{
						// 프로토콜은 제외했다.


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
										msg = string.Empty;
										UnPackPacket(packet, out msg);
										Debug.Log(msg);
										waitHandle.Set();
										recvFlag = true;    // 수신 후 모든 처리 완료
									}
								}
								break;
						}
					}
					break;

				// Lobby 상태일 때
				case STATE_PROTOCOL.LOBBY_STATE:
					{
						// 프로토콜 읽고
						switch(protocol)
						{
							// 매칭되서 게임 시작 프로토콜이면
							case PROTOCOL.START_PROTOCOL:
								{
									switch(result)
									{
										// 매칭 성공시(Unpack할 내용은 없다)
										case RESULT.MATCH_SUCCESS:
											{
												Debug.Log("매칭 성공!!");
											}
											break;

											// 매칭 실패시
										case RESULT.MATCH_FAIL:
											{
												Debug.Log("매칭 실패!!");
											}
											break;
									}
								}
								break;
						}
					}
					break;

			}
		}
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
		byte[] encryptBuf = new byte[C_Global.BUFSIZE];
		byte[] buf = new byte[C_Global.BUFSIZE];

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
		byte[] encryptBuf = new byte[C_Global.BUFSIZE];
		byte[] buf = new byte[C_Global.BUFSIZE];

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
		byte[] encryptBuf = new byte[C_Global.BUFSIZE];
		byte[] buf = new byte[C_Global.BUFSIZE];

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


	public string GetResultMsg()
	{
		return msg;
	}

	public void MayILogin(string _id, string _pw)
	{
		// 프로토콜 셋팅
		protocol = SetProtocol(
				STATE_PROTOCOL.LOGIN_STATE,
				PROTOCOL.LOGIN_PROTOCOL,
				RESULT.NODATA);
		Console.WriteLine((Int64)protocol);

		// 패킹 및 서버로 전송
		int packetSize;
		PackPacket(ref sendBuf, protocol, _id, _pw, out packetSize);
		bw.Write(sendBuf, 0, packetSize);

		//// 수신한 로그인 결과에대한 처리를 다 할때까지 대기하고, 받은 메시지를 리턴한다.
		//RecvProcess();
		//waitHandle.WaitOne();

		//while (true)
		//{
			//Debug.Log("모든 처리를 기다리는 중!");
			//yield return new WaitForSeconds(1.0f);
			//yield return new WaitWhile(() => recvFlag == false);
			//Debug.Log("모든 처리가 완료됨!");
		//}
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


	public void MayIJoin(string _id, string _pw, string _nickname)
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

	public void MayIMatch()
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
	   

		//// 서버로부터 결과 얻기
		//int readSize = br.ReadInt32();      // 먼저 총 size를 얻어온다.
		//recvBuf = br.ReadBytes(readSize);   // 그리고 받아야할 size만큼 byte[]를 얻어온다.


		//// 얻은 버퍼를 복호화 진행
		//byte[] decryptBuf = new byte[C_Global.BUFSIZE];
		//C_Encrypt.GetInstance.Decrypt(recvBuf, decryptBuf, readSize);

		//GetProtocol(decryptBuf, out state, out protocol, out result);

		//if (result == RESULT.MATCH_SUCCESS)
		//	return true;

		//else
		//	return false;
	}

	public bool CheckMatched()
	{
		// 아직 시작을 매칭 잡는 중이면 false리턴
		if (protocol == PROTOCOL.START_PROTOCOL)
			return true;

		return false;
	}

	public bool CheckMatchSuccess()
	{
		// 매칭 성공시 true
		if (result == RESULT.MATCH_SUCCESS)
			return true;

		// 실패시 false
		else
			return false;
	}

	public void Disconnect()
	{
		ThreadManager.GetInstance.End();
		bw.Close();
		br.Close();
		tcpClient.Close();
	}

	public IEnumerator WaitRecvAll()
	{
		while(true)
		{
			if(recvFlag == true)
				yield return true;

			yield return null;
		}
	}
}