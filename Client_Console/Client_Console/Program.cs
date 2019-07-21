using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// 1. 일단 string만 전송해본다.
// 2. 구조체 만들어서 전송해본다.

namespace Client_Console
{
	[Serializable]
	class Program
	{
		const int IDSIZE = 255;
		const int PWSIZE = 255;
		const int NICKNAMESIZE = 255;
		// 53 ~ 49
		enum RESULT : Int64
		{
			//LoginState
			JOIN_SUCCESS = ((Int64)0x1 << 53),
			LOGIN_SUCCESS = ((Int64)0x1 << 53),
			LOGOUT_SUCCESS = ((Int64)0x1 << 53),

			// LobbyState
			ENTER_ROOM_SUCCESS = ((Int64)0x1 << 53),
			CREATE_ROOM_SUCCESS = ((Int64)0x1 << 53),

			// ChatState
			LEAVE_ROOM_SUCCESS = ((Int64)0x1 << 53),
			LEAVE_ROOM_FAIL = ((Int64)0x1 << 52),

			// LoginState
			ID_EXIST = ((Int64)0x1 << 52),
			ID_ERROR = ((Int64)0x1 << 51),
			PW_ERROR = ((Int64)0x1 << 50),

			// LobbyState
			LOGOUT_FAIL = ((Int64)0x1 << 49),
			ENTER_ROOM_FAIL = ((Int64)0x1 << 49),
			CREATE_ROOM_FAIL = ((Int64)0x1 << 49),


			NODATA = ((Int64)0x1 << 49)
		};

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

			// LobbyState
			ENTER_ROOM_PROTOCOL = ((Int64)0x1 << 58),
			CREATE_ROOM_PROTOCOL = ((Int64)0x1 << 57),
			LOGOUT_PROTOCOL = ((Int64)0x1 << 56),
			ROOMLIST_PROTOCOL = ((Int64)0x1 << 55),
			// 54

			// ChatState
			LEAVE_ROOM_PROTOCOL = ((Int64)0x1 << 58),
			CHAT_PROTOCOL = ((Int64)0x1 << 57),
		};

		enum MENU : int
		{
			JOIN_MENU = 1,
			LOGIN_MENU, // 2
			LOGOUT_MENU = 1,
			ENTER_ROOM, // 2
			CREATE_ROOM, // 3
			ROOM_LIST,  // 4 방 목록
			EXIT_MENU = 3,
			LEAVE_MENU = -1
		};

		enum STATE
		{
			CLOGIN_STATE = 1,
			CLOBBY_STATE,
			CCHAT_STATE
		};

		struct _User_Info
		{
			//public char []id;
			//public char []pw;
			//public char []nickname;

			public string id;
			public string pw;
			public string nickname;
		};



		// 서버 IP와 포트
		static IPAddress serverIP = IPAddress.Parse("127.0.0.1");
		static int serverPort     = 9000;

		const int BUFSIZE = 4096;	// 버퍼 사이즈

		static byte[] buf          = new byte[BUFSIZE];  // 버퍼

		static TcpClient tcpClient = new TcpClient();
		static NetworkStream ns;


		static int m_size                   = 0;
		static STATE_PROTOCOL m_serverState = 0;
		static PROTOCOL m_protocol          = 0;
		static RESULT m_result              = 0;
		static STATE m_state                = 0;

		static void Main(string[] args)
		{
			IPEndPoint serverEndPoint = new IPEndPoint(serverIP, serverPort);

			try
			{
				tcpClient.Connect(serverEndPoint);
				if (tcpClient.Connected == true)
					Console.WriteLine("서버와 연결 성공");
			}
			catch (SocketException)
			{
				Console.WriteLine("서버와 연결 실패");
				throw new Exception();
			}

			BinaryWriter bw = null;
			if (ns == null)
			{
				ns = tcpClient.GetStream();
				bw = new BinaryWriter(ns);
			}

			bool endFlag = false;

			m_state = STATE.CLOGIN_STATE;
			MENU select = 0;
			Console.WriteLine("<<<C# 로그인 클라이언트>>>");
			while (true)
			{
				endFlag = false;

				switch (m_state)
				{
					case STATE.CLOGIN_STATE:
						{
							Console.Write("<<메뉴>>\n");
							Console.Write("1. 회원가입\n");
							Console.Write("2. 로그인\n");
							Console.Write("3. 종료\n");
							Console.Write("선택:");

							try
							{
								select = (MENU)int.Parse(Console.ReadLine());
							}
							catch (Exception)
							{
								Console.WriteLine("메뉴 선택 도중 예외 발생!");
							}

							switch (select)
							{
								case MENU.JOIN_MENU:
									{
										_User_Info info;
										Console.Write("ID:");
										info.id = Console.ReadLine();
										Console.Write("PW:");
										info.pw = Console.ReadLine();
										Console.Write("NICKNAME:");
										info.nickname = Console.ReadLine();

										//PROTOCOL protocol = SetProtocol(
										//	STATE_PROTOCOL.LOGIN_STATE,
										//	PROTOCOL.JOIN_PROTOCOL,
										//	RESULT.NODATA);

										PROTOCOL protocol = SetProtocol(
											(STATE_PROTOCOL)0,
											PROTOCOL.JOIN_PROTOCOL,
											(RESULT)0);
										Console.WriteLine((Int64)protocol);
										
										PackPacket(ref buf, protocol, info.id, info.pw, info.nickname, ref m_size);
										//BinaryFormatter bf = new BinaryFormatter();
										//bf.Serialize(ns, buf);

										/// 도대체 어떻게 해야 서버로 전송했을 때 온전하게 갈까?
										/// 근데 또 생각해보면 문자열은 그런대로 갔단 말이지..
										/// 64비트 전송하는게 마샬링이 필요한건가?

										//bw.Write(buf, 0, m_size);
										ns.Write(buf, 0, m_size);
									}
									break;

								default:
									Console.WriteLine("잘못된 입력입니다!");
									continue;
							}


							int readSize = 0;
							byte[] packetSize = new byte[sizeof(int)];
							Array.Clear(buf, 0, buf.Length);
							if (ns.CanRead == true)
							{
								ns.Read(packetSize, 0, sizeof(int));
								//ns.Read(BitConverter.GetBytes(readSize), 0, sizeof(int));
								do
								{
									readSize = ns.Read(buf, 0, BitConverter.ToInt32(packetSize, 0));
									//readSize = ns.Read(buf, 0, sizeof(int));
								}
								while (ns.DataAvailable == true);
							
							}


							byte[] decryptBuf = new byte[BUFSIZE];
							C_Encrypt.Decrypt(buf, decryptBuf, readSize);

							GetProtocol(decryptBuf, ref m_serverState, ref m_protocol, ref m_result);

							string msg = string.Empty;
							UnPackPacket(decryptBuf, ref msg);
							Console.WriteLine(msg);

							switch (m_protocol)
							{
								case PROTOCOL.JOIN_PROTOCOL:
									break;
								case PROTOCOL.LOGIN_PROTOCOL:
									switch (m_result)
									{
										case RESULT.ID_ERROR:
										case RESULT.PW_ERROR:
											break;
										case RESULT.LOGIN_SUCCESS:
											// LOBBY STATE
											m_state = STATE.CLOBBY_STATE;
											break;
									}
									break;
							}
						}
					break;
				}

				if (endFlag == true)
					break;
			}

			ns.Close();
			tcpClient.Close();
		}

		static string ByteToString(byte[] _byte)
		{
			string str = Encoding.Unicode.GetString(_byte);
			return str;
		}

		static byte[] StringToByte(string _str)
		{
			byte[] strByte = Encoding.Unicode.GetBytes(_str);
			return strByte;
		}

		static void GetProtocol(byte[] _buf, ref STATE_PROTOCOL _state, ref PROTOCOL _protocol, ref RESULT _result)
		{
			// 각각 얻을 state, protocol, result값
			STATE_PROTOCOL state;
			PROTOCOL protocol;
			RESULT result;

			int offset = 0;

			

			/// BlockCopy를 해서 얻어올때 BitConverter.GetBytes로 얻어오면 byte형식으로 올바르게 못얻어옴, 따라서 byte배열을 선언한 뒤에 거기에다가 저장을 시켜야 함!
			// 통짜로 프로토콜 다 받아와서
			byte[] byteProtocol = new byte[sizeof(PROTOCOL)];	// 1. byte형식으로 얻어올 배열
			PROTOCOL wholeProtocol = 0;							// 64비트로 가지고있을 Protocol 자체
			Buffer.BlockCopy(_buf, offset, byteProtocol, 0, sizeof(PROTOCOL));	// 우선 byte형식으로 얻음
			wholeProtocol = (PROTOCOL)BitConverter.ToInt64(byteProtocol, 0);	// 위에서 얻은 배열로 Int64를 만들어 Protocol에 저장
			offset += sizeof(PROTOCOL);

			// 상위 5비트를 걸러줄 마스크를 생성하고
			Int64 mask = ((Int64)0x1f << (64 - 5));

			// 마스크를 통해 스테이트를 얻는다.
			state = (STATE_PROTOCOL)((Int64)wholeProtocol & mask);

			// 이제 state를 얻었으니 protocol을 얻어보자
			protocol = (PROTOCOL)wholeProtocol;
			switch (state)
			{
				case STATE_PROTOCOL.LOGIN_STATE:
					mask = ((Int64)0x1f << (64 - 10));
					_protocol = (PROTOCOL)(protocol & (PROTOCOL)mask);

					result = (RESULT)wholeProtocol; // 이제 protocol을 얻었으니 result를 얻어보자

					mask = ((Int64)0x1f << (64 - 15));
					_result = (RESULT)(result & (RESULT)mask);
					break;

				case STATE_PROTOCOL.LOBBY_STATE:
					mask = ((Int64)0x1f << (64 - 10));
					_protocol = (PROTOCOL)(protocol & (PROTOCOL)mask);

					result = (RESULT)wholeProtocol; // 이제 protocol을 얻었으니 result를 얻어보자

					mask = ((Int64)0x1f << (64 - 15));
					_result = (RESULT)(result & (RESULT)mask);
					break;
				case STATE_PROTOCOL.CHAT_STATE:
					break;
			}

		}

		static PROTOCOL SetProtocol(STATE_PROTOCOL _state, PROTOCOL _protocol, RESULT _result)
		{
			PROTOCOL protocol = (PROTOCOL)0;
			protocol = (PROTOCOL)((Int64)_state | (Int64)_protocol | (Int64)_result);
			return protocol;
		}


		static void PackPacket(ref byte[] _buf, PROTOCOL _protocol, string _str1, string _str2, string _str3, ref int _size)
		{
			int strsize1 = _str1.Length * sizeof(char);
			int strsize2 = _str2.Length * sizeof(char);
			int strsize3 = _str3.Length * sizeof(char);

			// 암호화된 내용을 저장할 버퍼이다.
			byte[] encryptBuf = new byte[BUFSIZE];
			byte[] buf        = new byte[BUFSIZE];

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
			C_Encrypt.Encrypt(buf, encryptBuf, _size);

			// 가장 앞에 size를 넣고, 그 뒤에 암호화했던 버퍼를 붙임.
			offset = 0;
			Buffer.BlockCopy(BitConverter.GetBytes(_size), 0, _buf, offset, sizeof(int));
			offset += sizeof(int);
			Buffer.BlockCopy(encryptBuf, 0, _buf, offset, _size);
			offset += _size;

			_size += sizeof(int);	// 총 보내야 할 바이트 수 저장한다.
		}
		static void UnPackPacket(byte[] _buf, ref string _str1)
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
	}

	class C_Encrypt
	{
		const int C1 = 52845;
		const int C2 = 22719;
		const int KEY = 78695;

		public static bool Encrypt(byte[] _src, byte[] _dest, int _size)
		{
			int key = KEY;

			if (_src == null || _dest == null || _size <= 0)
				return false;


			for (int i = 0; i < _size; i++)
			{
				_dest[i] = (byte)(_src[i] ^ key >> 8);
				key = ((sbyte)_dest[i] + key) * C1 + C2;	// key값을 다시 연산할때에는 sbyte로 변환한 뒤에 연산해야 음수값이 제대로 들어갈 수 있다.
			}

			return true;
		}

		public static bool Decrypt(byte[] _src, byte[] _dest, int _size)
		{
			byte prevBlock;
			int key = KEY;


			if (_src == null || _dest == null || _size <= 0)
				return false;

			for (int i = 0; i < _size; i++)
			{
				prevBlock = _src[i];
				_dest[i] = (byte)(_src[i] ^ key >> 8);
				key = ((sbyte)prevBlock + key) * C1 + C2;
			}

			return true;
		}
	}
}
