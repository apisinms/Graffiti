using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
//using Unity.Jobs
//using Unity.Collections


/// <summary>
/// NetworkManager_Main.cs파일
/// 주로 상수, 프로토콜, 멤버변수, 프로퍼티 같은 선언이 있다.
/// </summary>
public partial class NetworkManager : MonoBehaviour
{
	readonly static int IDSIZE = 255;
	readonly static int PWSIZE = 255;
	readonly static int NICKNAMESIZE = 255;


	readonly static int STATE_PROTOCOL_OFFSET = 10;                     // 10
	readonly static int PROTOCOL_OFFSET = STATE_PROTOCOL_OFFSET + 20;   // 30
	readonly static int RESULT_OFFSET = PROTOCOL_OFFSET + 10;         // 40

	readonly static int STATE_PROTOCOL_MASK = 0x3FF;
	readonly static int PROTOCOL_MASK = 0xFFFFF;
	readonly static int RESULT_MASK = 0x3FF;

	/// <summary>
	/// 10(STATE_PROTOCOL) + 20(PROTOCOL) + 10(RESULT) + 24(그외)
	/// </summary>
	// 63 ~ 54
	enum STATE_PROTOCOL : Int64
	{
		// 상위 5비트 스테이트를 표현해주는 프로토콜
		LOGIN_STATE = ((Int64)0x1 << 63),
		LOBBY_STATE = ((Int64)0x1 << 62),
		CHAT_STATE = ((Int64)0x1 << 61),
		INGAME_STATE = ((Int64)0x1 << 60),

		//59 ~ 54
	};

	//53 ~ 34
	enum PROTOCOL : Int64
	{
		// LoginState
		JOIN_PROTOCOL = ((Int64)0x1 << 53),
		LOGIN_PROTOCOL = ((Int64)0x1 << 52),

		// LobbyState
		MATCH_PROTOCOL = ((Int64)0x1 << 53),
		MATCH_CANCEL_PROTOCOL = ((Int64)0x1 << 52),
		GOTO_INGAME_PROTOCOL = ((Int64)0x1 << 51),      // 인게임 상태로 진입 프로토콜
		LOGOUT_PROTOCOL = ((Int64)0x1 << 50),

		// ChatState
		LEAVE_ROOM_PROTOCOL = ((Int64)0x1 << 53),
		CHAT_PROTOCOL = ((Int64)0x1 << 52),

		// InGameState
		TIMER_PROTOCOL = ((Int64)0x1 << 53),    // 타이머 프로토콜(1초씩 받음)
		WEAPON_PROTOCOL = ((Int64)0x1 << 52),   // 무기 전송 프로토콜
		START_PROTOCOL = ((Int64)0x1 << 51),    // 게임 시작 프로토콜
		MOVE_PROTOCOL = ((Int64)0x1 << 50), // 이동 프로토콜
		DISCONNECT_PROTOCOL = ((Int64)0x1 << 49), // 접속 끊김 프로토콜

		// 48 ~ 34
	};

    // 33 ~ 24
    enum RESULT : Int64
    {
        //LoginState
        JOIN_SUCCESS = ((Int64)0x1 << 33),
        LOGIN_SUCCESS = ((Int64)0x1 << 33),
        LOGOUT_SUCCESS = ((Int64)0x1 << 33),
        LOGOUT_FAIL = ((Int64)0x1 << 32),

        // Join & Login result
        ID_EXIST = ((Int64)0x1 << 32),
        ID_ERROR = ((Int64)0x1 << 31),
        PW_ERROR = ((Int64)0x1 << 30),

        // LobbyState
        LOBBY_SUCCESS = ((Int64)0x1 << 33),     // 로비에서 성공 처리
        LOBBY_FAIL = ((Int64)0x1 << 32),        // 로비에서 실패 처리

        // ChatState
        LEAVE_ROOM_SUCCESS = ((Int64)0x1 << 33),
        LEAVE_ROOM_FAIL = ((Int64)0x1 << 32),

        // InGameState
        INGAME_SUCCESS = ((Int64)0x1 << 33),
        INGAME_FAIL = ((Int64)0x1 << 32),
        ENTER_SECTOR = ((Int64)0x1 << 31),   // 섹터 진입
        EXIT_SECTOR = ((Int64)0x1 << 30),    // 섹터 퇴장
        UPDATE_PLAYER = ((Int64)0x1 << 29),   // 플레이어 목록 최신화

        // ~ 25
        NODATA = ((Int64)0x1 << 24)
    };

    enum PLAYER_BIT : byte
    {
        PLAYER_1 = (1 << 3),
        PLAYER_2 = (1 << 2),
        PLAYER_3 = (1 << 1),
        PLAYER_4 = (1 << 0),
    }

    struct _User_Info
	{
		public string id;
		public string pw;
		public string nickname;
	};

	// 마샬링을 위한 WeaponPac
	[StructLayout(LayoutKind.Sequential)]
	struct WeaponPacket
	{
		[MarshalAs(UnmanagedType.I1)]
		public sbyte mainW;

		[MarshalAs(UnmanagedType.I1)]
		public sbyte subW;

		public byte[] Serialize()
		{
			// allocate a byte array for the struct data
			var buffer = new byte[Marshal.SizeOf(typeof(WeaponPacket))];

			// Allocate a GCHandle and get the array pointer
			var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			var pBuffer = gch.AddrOfPinnedObject();

			// copy data from struct to array and unpin the gc pointer
			Marshal.StructureToPtr(this, pBuffer, false);
			gch.Free();

			return buffer;
		}
		public void Deserialize(ref byte[] data)
		{
			var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
			this = (WeaponPacket)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(WeaponPacket));
			gch.Free();
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	struct PositionPacket
	{
		[MarshalAs(UnmanagedType.I4)]
		public int playerNum;

		[MarshalAs(UnmanagedType.R4)]
		public float posX;

		[MarshalAs(UnmanagedType.R4)]
		public float posZ;

		[MarshalAs(UnmanagedType.R4)]
		public float rotY;

        [MarshalAs(UnmanagedType.R4)]
        public float speed;

        [MarshalAs(UnmanagedType.I4)]
        public int action;

        public byte[] Serialize()
		{
			// allocate a byte array for the struct data
			var buffer = new byte[Marshal.SizeOf(typeof(PositionPacket))];

			// Allocate a GCHandle and get the array pointer
			var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			var pBuffer = gch.AddrOfPinnedObject();

			// copy data from struct to array and unpin the gc pointer
			Marshal.StructureToPtr(this, pBuffer, false);
			gch.Free();

			return buffer;
		}
		public void Deserialize(ref byte[] data)
		{
			var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
			this = (PositionPacket)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(PositionPacket));
			gch.Free();
		}
	}

	PositionPacket[] posPacket = new PositionPacket[C_Global.MAX_PLAYER];

	STATE_PROTOCOL state;   // 클라 상태
	PROTOCOL protocol;      // 프로토콜
	RESULT result;          // 결과

	// 서버 IP와 포트
	private static IPAddress serverIP = IPAddress.Parse("127.0.0.1");
	private static int serverPort = 10823;

	// 버퍼
	private byte[] sendBuf = new byte[C_Global.BUFSIZE];                // 송신 버퍼
																		//private byte[] recvBuf = new byte[C_Global.BUFSIZE];				// 수신 버퍼

	// TCP 클라, 바이너리 리더, 라이터
	private static TcpClient tcpClient = new TcpClient();
	private BinaryReader br = null;
	private BinaryWriter bw = null;

	private object key = new object();      // 동기화에 사용할 key이다.
	private string sysMsg = string.Empty;   // 서버로부터 전달되는 메시지를 저장할 변수
	private int myPlayerNum;
	private int quitPlayerNum;              // 게임에서 나간 플레이어 번호

	private Queue<C_Global.QueueInfo> queue;    // recv에 관한 패킷이 저장될 큐

	public Queue<C_Global.QueueInfo> Queue { get { return queue; } }
	public BinaryReader BinaryReader { get { return br; } }
	public BinaryWriter BinaryWriter { get { return bw; } }

	public string SysMsg
	{
		get { return sysMsg; }
		set { sysMsg = value; }
	}

	public int MyPlayerNum
	{
		get { return myPlayerNum; }
		set { myPlayerNum = value; }
	}
	public float GetPosX(int _idx)
	{
		return posPacket[_idx].posX;
	}
	public float GetPosZ(int _idx)
	{
		return posPacket[_idx].posZ;
	}
	public float GetRotY(int _idx)
	{
		return posPacket[_idx].rotY;
	}
    public float GetSpeed(int _idx)
    {
        return posPacket[_idx].speed;
    }
    public float GetActionState(int _idx)
    {
        return posPacket[_idx].action;
    }
    public int GetPosPlayerNum(int _idx)
	{
		return posPacket[_idx].playerNum;
	}

	public void SetPosX(int _idx, float _posX)
	{
		this.posPacket[_idx].posX = _posX;
	}
	public void SetPosZ(int _idx, float _posZ)
	{
		this.posPacket[_idx].posZ = _posZ;
	}
	public void SetRotY(int _idx, float _rotY)
	{
		this.posPacket[_idx].rotY = _rotY;
	}
    public void SetSpeed(int _idx, float _speed)
    {
        this.posPacket[_idx].speed = _speed;
    }
    public void SetActionState(int _idx, int _action)
    {
        this.posPacket[_idx].action = _action;
    }
    public void SetPosPlayerNum(int _idx, int _num)
	{
		this.posPacket[_idx].playerNum = _num;
	}

	public static NetworkManager instance = null;

}