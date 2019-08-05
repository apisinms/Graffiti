#define __64BIT__
using System;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;


/// <summary>
/// NetworkManager_Packet.cs파일
/// 패킷, 프로토콜에 관한 내용이 있다.
/// </summary>
public partial class NetworkManager : MonoBehaviour
{
	// 바이트 배열을 스트링으로
	private string ByteToString(byte[] _byte)
	{
		string str = Encoding.Unicode.GetString(_byte);
		return str;
	}

	// 스트링을 바이트 배열로
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
#if __64BIT__
		wholeProtocol = (PROTOCOL)BitConverter.ToInt64(byteProtocol, 0);    // 2. 위에서 얻은 배열로 Int64를 만들어 Protocol에 저장
#endif
#if __32BIT__
		wholeProtocol = (PROTOCOL)BitConverter.ToInt32(byteProtocol, 0);
#endif
		offset += sizeof(PROTOCOL);         // 오프셋 증가


#if __64BIT__
		// 상위 5비트를 걸러줄 마스크를 생성하고
		Int64 mask = ((Int64)0x1f << (64 - 5));

		// 마스크를 통해 스테이트를 얻는다.
		_state = (STATE_PROTOCOL)((Int64)wholeProtocol & mask);

		mask = ((Int64)0x1f << (64 - 10));
		_protocol = (PROTOCOL)((Int64)wholeProtocol & mask);  // 이제 state를 얻었으니 protocol을 얻어보자

		mask = ((Int64)0x1f << (64 - 15));
		_result = (RESULT)((Int64)wholeProtocol & mask);       // 이제 마지막으로 result



		// state를 읽어서 protocol과 resul를 마스킹을 통해 구한다.
		//switch (_state)
		//{
		//	case STATE_PROTOCOL.LOGIN_STATE:
		//		{
		//			mask = ((Int64)0x1f << (64 - 10));
		//			_protocol = (PROTOCOL)((Int64)wholeProtocol & mask);  // 이제 state를 얻었으니 protocol을 얻어보자

		//			mask = ((Int64)0x1f << (64 - 15));
		//			_result = (RESULT)((Int64)wholeProtocol & mask);     // 이제 마지막으로 result
		//		}
		//		break;

		//	case STATE_PROTOCOL.LOBBY_STATE:
		//		{
		//			mask = ((Int64)0x1f << (64 - 10));
		//			_protocol = (PROTOCOL)((Int64)wholeProtocol & mask);  // 이제 state를 얻었으니 protocol을 얻어보자

		//			mask = ((Int64)0x1f << (64 - 15));
		//			_result = (RESULT)((Int64)wholeProtocol & mask);       // 이제 마지막으로 result
		//		}
		//		break;

		//	case STATE_PROTOCOL.INGAME_STATE:
		//		{
		//			mask = ((Int64)0x1f << (64 - 10));
		//			_protocol = (PROTOCOL)((Int64)wholeProtocol & mask);  // 이제 state를 얻었으니 protocol을 얻어보자

		//			mask = ((Int64)0x1f << (64 - 15));
		//			_result = (RESULT)((Int64)wholeProtocol & mask);       // 이제 마지막으로 result
		//		}
		//		break;

		//	//case STATE_PROTOCOL.CHAT_STATE:
		//	//	break;
		//}
#endif

#if __32BIT__
		// 상위 5비트를 걸러줄 마스크를 생성하고
		Int32 mask = ((Int32)0x1f << (32 - 5));

		// 마스크를 통해 스테이트를 얻는다.
		_state = (STATE_PROTOCOL)((Int32)wholeProtocol & mask);

		// state를 읽어서 protocol과 resul를 마스킹을 통해 구한다.
		switch (_state)
		{
			case STATE_PROTOCOL.LOGIN_STATE:
				mask = ((Int32)0x1f << (32 - 10));
				_protocol = (PROTOCOL)((Int64)wholeProtocol & mask);  // 이제 state를 얻었으니 protocol을 얻어보자

				mask = ((Int32)0x1f << (32 - 15));
				_result = (RESULT)((Int64)wholeProtocol & mask);     // 이제 마지막으로 result
				break;

			case STATE_PROTOCOL.LOBBY_STATE:
				mask = ((Int32)0x1f << (32 - 10));
				_protocol = (PROTOCOL)((Int64)wholeProtocol & mask);  // 이제 state를 얻었으니 protocol을 얻어보자

				mask = ((Int32)0x1f << (32 - 15));
				_result = (RESULT)((Int64)wholeProtocol & mask);       // 이제 마지막으로 result
				break;

			case STATE_PROTOCOL.CHAT_STATE:
				break;
		}
#endif

	}

	private PROTOCOL SetProtocol(STATE_PROTOCOL _state, PROTOCOL _protocol, RESULT _result)
	{
		PROTOCOL protocol = (PROTOCOL)0;
#if __64BIT__
		protocol = (PROTOCOL)((Int64)_state | (Int64)_protocol | (Int64)_result);
#endif

#if __32BIT__
		protocol = (PROTOCOL)((Int32)_state | (Int32)_protocol | (Int32)_result);
#endif
		return protocol;
	}

	private void PackPacket(ref byte[] _sendBuf, PROTOCOL _protocol, out int _size)
	{
		Array.Clear(_sendBuf, 0, C_Global.BUFSIZE);	// 송신 버퍼 초기화

		// 임시 저장 버퍼 + 암호화 시킬 버퍼
		byte[] tmpBuf = new byte[C_Global.BUFSIZE];

		_size = 0;
		int offset = 0;

		// 프로토콜
#if __64BIT__
		Buffer.BlockCopy(BitConverter.GetBytes((Int64)_protocol), 0, tmpBuf, offset, sizeof(PROTOCOL));
#endif
#if __32BIT__
		Buffer.BlockCopy(BitConverter.GetBytes((Int32)_protocol), 0, buf, offset, sizeof(PROTOCOL));
#endif

		offset += sizeof(PROTOCOL);
		_size += sizeof(PROTOCOL);

		// 암호화된 내용을 encryptBuf에 저장
		C_Encrypt.instance.Encrypt(tmpBuf, tmpBuf, _size);

		// 가장 앞에 size를 넣고, 그 뒤에 암호화했던 버퍼를 붙임.
		offset = 0;
		Buffer.BlockCopy(BitConverter.GetBytes(_size), 0, _sendBuf, offset, sizeof(int));
		offset += sizeof(int);
		Buffer.BlockCopy(tmpBuf, 0, _sendBuf, offset, _size);
		offset += _size;

		_size += sizeof(int);   // 총 보내야 할 바이트 수 저장한다.
	}

	private void PackPacket(ref byte[] _buf, PROTOCOL _protocol, WeaponPacket _weapon, out int _size)
	{

		// 암호화된 내용을 저장할 버퍼이다.
		byte[] encryptBuf = new byte[C_Global.BUFSIZE];
		byte[] buf = new byte[C_Global.BUFSIZE];

		_size = 0;
		int offset = 0;

		// 프로토콜
#if __64BIT__
		Buffer.BlockCopy(BitConverter.GetBytes((Int64)_protocol), 0, buf, offset, sizeof(PROTOCOL));
#endif
#if __32BIT__
		Buffer.BlockCopy(BitConverter.GetBytes((Int32)_protocol), 0, buf, offset, sizeof(PROTOCOL));
#endif
		offset += sizeof(PROTOCOL);
		_size += sizeof(PROTOCOL);

		// 프로토콜
		Buffer.BlockCopy(_weapon.Serialize(), 0, buf, offset, Marshal.SizeOf(_weapon));
		offset += Marshal.SizeOf(_weapon);
		_size += Marshal.SizeOf(_weapon);

		// 암호화된 내용을 encryptBuf에 저장
		C_Encrypt.instance.Encrypt(buf, encryptBuf, _size);

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
		byte[] buf        = new byte[C_Global.BUFSIZE];

		_size = 0;
		int offset = 0;

		// 프로토콜
#if __64BIT__
		Buffer.BlockCopy(BitConverter.GetBytes((Int64)_protocol), 0, buf, offset, sizeof(PROTOCOL));
#endif
#if __32BIT__
		Buffer.BlockCopy(BitConverter.GetBytes((Int32)_protocol), 0, buf, offset, sizeof(PROTOCOL));
#endif
		offset += sizeof(PROTOCOL);
		_size  += sizeof(PROTOCOL);

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
		C_Encrypt.instance.Encrypt(buf, encryptBuf, _size);

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
#if __64BIT__
		Buffer.BlockCopy(BitConverter.GetBytes((Int64)_protocol), 0, buf, offset, sizeof(PROTOCOL));
#endif
#if __32BIT__
		Buffer.BlockCopy(BitConverter.GetBytes((Int32)_protocol), 0, buf, offset, sizeof(PROTOCOL));
#endif
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
		C_Encrypt.instance.Encrypt(buf, encryptBuf, _size);

		// 가장 앞에 size를 넣고, 그 뒤에 암호화했던 버퍼를 붙임.
		offset = 0;
		Buffer.BlockCopy(BitConverter.GetBytes(_size), 0, _buf, offset, sizeof(int));
		offset += sizeof(int);
		Buffer.BlockCopy(encryptBuf, 0, _buf, offset, _size);
		offset += _size;

		_size += sizeof(int);   // 총 보내야 할 바이트 수 저장한다.
	}

	private void UnPackPacket(byte[] _buf, out int _num)
	{
		byte[] arrNum = new byte[sizeof(int)];

		int offset = sizeof(PROTOCOL);

		// 일단 byte 배열로 받고
		Buffer.BlockCopy(_buf, offset, arrNum, 0, sizeof(int));
		offset += sizeof(int);

		// 이제 다시 int로 변환
		_num = BitConverter.ToInt32(arrNum, 0);
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
}


