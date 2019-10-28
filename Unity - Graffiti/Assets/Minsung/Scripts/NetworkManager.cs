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
    // 서버 IP와 포트
    //private static IPAddress serverIP = IPAddress.Parse("127.0.0.1");
    private static IPAddress serverIP = IPAddress.Parse("211.227.82.184");


    private static int serverPort = 10823;

	readonly static int IDSIZE = 255;
	readonly static int PWSIZE = 255;
	readonly static int NICKNAMESIZE = 255;


	readonly static int STATE_PROTOCOL_OFFSET = 10;                     // 10
	readonly static int PROTOCOL_OFFSET = STATE_PROTOCOL_OFFSET + 20;   // 30
	readonly static int RESULT_OFFSET = PROTOCOL_OFFSET + 24;           // 54

	readonly static int STATE_PROTOCOL_MASK = 0x3FF;
	readonly static int PROTOCOL_MASK = 0xFFFFF;
	readonly static int RESULT_MASK = 0xFFFFFF;

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
		TIMER_PROTOCOL = ((Int64)0x1 << 53),      // 타이머 프로토콜(1초씩 받음)
		WEAPON_PROTOCOL = ((Int64)0x1 << 52),      // 무기 전송 프로토콜
		NICKNAME_PROTOCOL = ((Int64)0x1 << 51),   // 본인의 닉네임을 받음
		START_PROTOCOL = ((Int64)0x1 << 50),      // 게임 시작 프로토콜
		LOADING_PROTOCOL = ((Int64)0x1 << 49),       // 로딩 여부 프로토콜
		UPDATE_PROTOCOL = ((Int64)0x1 << 48),      // 패킷 업데이트 프로토콜
		FOCUS_PROTOCOL = ((Int64)0x1 << 47),      // 포커스 프로토콜

		DISCONNECT_PROTOCOL = ((Int64)0x1 << 34), // 접속 끊김 프로토콜

		// 48 ~ 34
	};

    // 33 ~ 10
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

        // InGameState(공통)
        INGAME_SUCCESS = ((Int64)0x1 << 33),
        INGAME_FAIL = ((Int64)0x1 << 32),

        // WEAPON_PROTOCOL 개별
        NOTIFY_WEAPON = ((Int64)0x1 << 31),   // 무기를 알려줌

        // UPATE_PROTOCOL 개별
        ENTER_SECTOR = ((Int64)0x1 << 31),         // 섹터 진입
        EXIT_SECTOR = ((Int64)0x1 << 30),         // 섹터 퇴장
        UPDATE_PLAYER = ((Int64)0x1 << 29),         // 플레이어 목록 최신화
        FORCE_MOVE = ((Int64)0x1 << 28),         // 강제 이동
        GET_OTHERPLAYER_STATUS = ((Int64)0x1 << 27),         // 다른 플레이어 상태 얻기
        BULLET_HIT = ((Int64)0x1 << 26),         // 총알 맞음
        RESPAWN = ((Int64)0x1 << 25),         // 리스폰 요청 및 상대방 리스폰 수신
        CAR_SPAWN = ((Int64)0x1 << 24),         // 자동차 스폰
        CAR_HIT = ((Int64)0x1 << 23),         // 자동차에 치여 뒤짐
        KILL = ((Int64)0x1 << 22),         // 플레이어한테 뒤짐


        // ~ 11
        NODATA = ((Int64)0x1 << 10)
    };

    struct _User_Info
	{
		public string id;
		public string pw;
		public string nickname;
	};

    string nickName;
    public string NickName { get { return nickName; } set { nickName = value; } }

    // 총알 충돌 검사 구조체
    [StructLayout(LayoutKind.Sequential)]
	public struct BulletCollisionChecker
	{
		[MarshalAs(UnmanagedType.I1)]
		public byte playerBit;

		[MarshalAs(UnmanagedType.I4)]
		public int playerHitCountBit;

		private static BulletCollisionChecker dummy = new BulletCollisionChecker();
		public static BulletCollisionChecker GetDummy()
		{
			return dummy;
		}

		public byte[] Serialize()
		{
			// allocate a byte array for the struct data
			var buffer = new byte[Marshal.SizeOf(typeof(BulletCollisionChecker))];

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
			this = (BulletCollisionChecker)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(BulletCollisionChecker));
			gch.Free();
		}
	}

	// 마샬링을 위한 WeaponPac
	[StructLayout(LayoutKind.Sequential)]
	public struct WeaponPacket
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
	public struct IngamePacket
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

		[MarshalAs(UnmanagedType.R4)]
		public float health;

		[MarshalAs(UnmanagedType.I1)]
		public bool isReloading;

		// 총알 맞았는지 판정 구조체
		public BulletCollisionChecker collisionChecker;

		public byte[] Serialize()
		{
			// allocate a byte array for the struct data
			var buffer = new byte[Marshal.SizeOf(typeof(IngamePacket))];

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
			this = (IngamePacket)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(IngamePacket));
			gch.Free();
		}
	}

    IngamePacket[] ingamePackets; 

	STATE_PROTOCOL state;   // 클라 상태
	PROTOCOL protocol;      // 프로토콜
	RESULT result;          // 결과

	// 버퍼
	private byte[] sendBuf = new byte[C_Global.BUFSIZE];                // 송신 버퍼
																		//private byte[] recvBuf = new byte[C_Global.BUFSIZE];            // 수신 버퍼

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

	public IngamePacket GetIngamePacket(int _idx)
	{
		return ingamePackets[_idx];
	}

	public void SetIngamePacket(int _idx, ref IngamePacket _packet)
	{
		ingamePackets[_idx] = _packet;
	}

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
		return ingamePackets[_idx].posX;
	}
	public float GetPosZ(int _idx)
	{
		return ingamePackets[_idx].posZ;
	}
	public float GetRotY(int _idx)
	{
		return ingamePackets[_idx].rotY;
	}
	public float GetSpeed(int _idx)
	{
		return ingamePackets[_idx].speed;
	}
	public float GetActionState(int _idx)
	{
		return ingamePackets[_idx].action;
	}
	public int GetPlayerNum(int _idx)
	{
		return ingamePackets[_idx].playerNum;
	}

	public float GetHealth(int _idx)
	{
		return ingamePackets[_idx].health;
	}

	public bool GetReloadState(int _idx)
	{
		return ingamePackets[_idx].isReloading;
	}

	public void SetPosX(int _idx, float _posX)
	{
		this.ingamePackets[_idx].posX = _posX;
	}
	public void SetPosZ(int _idx, float _posZ)
	{
		this.ingamePackets[_idx].posZ = _posZ;
	}
	public void SetRotY(int _idx, float _rotY)
	{
		this.ingamePackets[_idx].rotY = _rotY;
	}
	public void SetSpeed(int _idx, float _speed)
	{
		this.ingamePackets[_idx].speed = _speed;
	}
	public void SetActionState(int _idx, int _action)
	{
		this.ingamePackets[_idx].action = _action;
	}
	public void SetPlayerNum(int _idx, int _num)
	{
		this.ingamePackets[_idx].playerNum = _num;
	}
	public void SetHelath(int _idx, float _hp)
	{
		this.ingamePackets[_idx].health = _hp;
	}

	public void SetReloadState(int _idx, bool _reloadState)
	{
		ingamePackets[_idx].isReloading = _reloadState;
	}

	public static NetworkManager instance = null;

}