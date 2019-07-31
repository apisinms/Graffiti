/*
 프로토콜 32, 64비트 관여 파일
 NetworkManager.cs
 NetworkManager.Packet.cs
 */

#define __64BIT__
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;


/// <summary>
/// NetworkManager_Main.cs파일
/// 주로 상수, 프로토콜, 멤버변수, 프로퍼티 같은 선언이 있다.
/// </summary>
public partial class NetworkManager : MonoBehaviour
{
	const int IDSIZE = 255;
	const int PWSIZE = 255;
	const int NICKNAMESIZE = 255;

#if __64BIT__
	// 63 ~ 59 
	enum STATE_PROTOCOL : Int64
	{
		// 상위 5비트 스테이트를 표현해주는 프로토콜
		LOGIN_STATE = ((Int64)0x1 << 63),
		LOBBY_STATE = ((Int64)0x1 << 62),
		CHAT_STATE = ((Int64)0x1 << 61),
		INGAME_STATE = ((Int64)0x1 << 60),
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

	    // InGameState
		MATCH_PROTOCOL = ((Int64)0x1 << 58),


	    // LobbyState
        START_PROTOCOL = ((Int64)0x1 << 57),      // 게임시작 프로토콜
		LOGOUT_PROTOCOL = ((Int64)0x1 << 56),

		// ChatState
		LEAVE_ROOM_PROTOCOL = ((Int64)0x1 << 58),
		CHAT_PROTOCOL = ((Int64)0x1 << 57),

		// InGameState
		ITEMSELECT_PROTOCOL = ((Int64)0x1 << 58),
        MOVE_PROTOCOL = ((Int64)0x1 << 57),
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
		MATCH_SUCCESS = ((Int64)0x1 << 53),		// 매칭 성공
		MATCH_FAIL = ((Int64)0x1 << 52),        // 매칭 성공

		// ChatState
		LEAVE_ROOM_SUCCESS = ((Int64)0x1 << 53),
		LEAVE_ROOM_FAIL = ((Int64)0x1 << 52),

		// InGameState
		INGAME_SUCCESS = ((Int64)0x1 << 53),
		INGAME_FAIL = ((Int64)0x1 << 52),


		NODATA = ((Int64)0x1 << 49)
	};
#endif

#if __32BIT__
	// 31~27
	enum STATE_PROTOCOL : int
	{
		// 상위 5비트 스테이트를 표현해주는 프로토콜
		LOGIN_STATE = ((int)0x1 << 31),
		LOBBY_STATE = ((int)0x1 << 30),
		CHAT_STATE = ((int)0x1 << 29),
		INGAME_STATE = ((int)0x1 << 28),
		//27
	};

	// 26 ~ 22
	enum PROTOCOL : int
	{
		// LoginState
		JOIN_PROTOCOL = ((int)0x1 << 26),
		LOGIN_PROTOCOL = ((int)0x1 << 25),
		// 56
		// 55
		// 54

		// LobbyState
		MATCH_PROTOCOL = ((int)0x1 << 26),
		START_PROTOCOL = ((int)0x1 << 25),      // 게임시작 프로토콜
		LOGOUT_PROTOCOL = ((int)0x1 << 24),

		// ChatState
		LEAVE_ROOM_PROTOCOL = ((int)0x1 << 26),
		CHAT_PROTOCOL = ((int)0x1 << 25),

		// InGameState
		ITEMSELECT_PROTOCOL = ((int)0x1 << 26),
		// 56
		// 55
		// 54
	};

	// 21 ~ 17
	enum RESULT : int
	{
		//LoginState
		JOIN_SUCCESS   = ((int)0x1 << 21),
		LOGIN_SUCCESS  = ((int)0x1 << 21),
		LOGOUT_SUCCESS = ((int)0x1 << 21),
		LOGOUT_FAIL    = ((int)0x1 << 20),

		// Join & Login result
		ID_EXIST = ((int)0x1 << 20),
		ID_ERROR = ((int)0x1 << 19),
		PW_ERROR = ((int)0x1 << 18),

		// LobbyState
		MATCH_SUCCESS = ((int)0x1 << 21),     // 매칭 성공
		MATCH_FAIL    = ((int)0x1 << 20),        // 매칭 성공

		// ChatState
		LEAVE_ROOM_SUCCESS = ((int)0x1 << 21),
		LEAVE_ROOM_FAIL    = ((int)0x1 << 20),

		// InGameState
		INGAME_SUCCESS = ((int)0x1 << 21),
		INGAME_FAIL    = ((int)0x1 << 20),


		NODATA = ((int)0x1 << 17)
	};
#endif

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
        [MarshalAs(UnmanagedType.R4)]
        public float posX;

        [MarshalAs(UnmanagedType.R4)]
        public float posZ;

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
    } PositionPacket position;


    STATE_PROTOCOL state;   // 클라 상태
	PROTOCOL protocol;      // 프로토콜
	RESULT result;          // 결과
	bool isInGame = false;	// 인게임에 들어갔는지

	// 서버 IP와 포트
	private static IPAddress serverIP = IPAddress.Parse("127.0.0.1");
	private static int serverPort = 9000;

	// 버퍼
	private byte[] sendBuf = new byte[C_Global.BUFSIZE];				// 송신 버퍼
	//private byte[] recvBuf = new byte[C_Global.BUFSIZE];				// 수신 버퍼

	// TCP 클라, 바이너리 리더, 라이터
	private static TcpClient tcpClient = new TcpClient();
	private BinaryReader br = null;
	private BinaryWriter bw = null;

	private object key = new object();      // 동기화에 사용할 key이다.
	private string sysMsg = string.Empty;	// 서버로부터 전달되는 메시지를 저장할 변수

	private Queue<C_Global.QueueInfo> queue;	// recv에 관한 패킷이 저장될 큐

	public Queue<C_Global.QueueInfo> Queue { get { return queue; } }
	public BinaryReader	BinaryReader { get { return br; } }
	public BinaryWriter BinaryWriter { get { return bw; } }

	public string SysMsg
	{
		get { return sysMsg; }
		set { sysMsg = value; }
	}

    public float GetPosX
    {
        get { return position.posX; }
    }
    public float GetPosZ
    {
        get { return position.posZ; }
    }

    public static NetworkManager instance = null;

}