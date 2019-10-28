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

		wholeProtocol = (PROTOCOL)BitConverter.ToInt64(byteProtocol, 0);    // 2. 위에서 얻은 배열로 Int64를 만들어 Protocol에 저장
		offset += sizeof(PROTOCOL);         // 오프셋 증가


		// STATE를 걸러줄 마스크를 생성하고
		Int64 mask = ((Int64)STATE_PROTOCOL_MASK << (64 - STATE_PROTOCOL_OFFSET));

		// 마스크를 통해 스테이트를 얻는다.
		_state = (STATE_PROTOCOL)((Int64)wholeProtocol & mask);

		mask = ((Int64)PROTOCOL_MASK << (64 - PROTOCOL_OFFSET));
		_protocol = (PROTOCOL)((Int64)wholeProtocol & mask);  // 이제 state를 얻었으니 protocol을 얻어보자

		mask = ((Int64)RESULT_MASK << (64 - RESULT_OFFSET));
		_result = (RESULT)((Int64)wholeProtocol & mask);       // 이제 마지막으로 result

		/// 그 뒤는 부가정보
	}

	private PROTOCOL SetProtocol(STATE_PROTOCOL _state, PROTOCOL _protocol, RESULT _result)
	{
		PROTOCOL protocol = (PROTOCOL)0;
		protocol = (PROTOCOL)((Int64)_state | (Int64)_protocol | (Int64)_result);
		return protocol;
	}

	private void PackPacket(ref byte[] _buf, PROTOCOL _protocol, out int _size)
	{
		Array.Clear(_buf, 0, C_Global.BUFSIZE); // 송신 버퍼 초기화

		// 임시 저장 버퍼 + 암호화 시킬 버퍼
		byte[] tmpBuf = new byte[C_Global.BUFSIZE];

		_size = 0;
		int offset = 0;

		// 프로토콜
		Buffer.BlockCopy(BitConverter.GetBytes((Int64)_protocol), 0, tmpBuf, offset, sizeof(PROTOCOL));
		offset += sizeof(PROTOCOL);
		_size += sizeof(PROTOCOL);

		// 암호화된 내용을 encryptBuf에 저장
		C_Encrypt.instance.Encrypt(tmpBuf, tmpBuf, _size);

		// 가장 앞에 size를 넣고, 그 뒤에 암호화했던 버퍼를 붙임.
		offset = 0;
		Buffer.BlockCopy(BitConverter.GetBytes(_size), 0, _buf, offset, sizeof(int));
		offset += sizeof(int);
		Buffer.BlockCopy(tmpBuf, 0, _buf, offset, _size);
		offset += _size;

		_size += sizeof(int);   // 총 보내야 할 바이트 수 저장한다.
	}

	private void PackPacket(ref byte[] _buf, PROTOCOL _protocol, int _num, out int _size)
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

		// num
		Buffer.BlockCopy(BitConverter.GetBytes(_num), 0, buf, offset, sizeof(int));
		offset += sizeof(int);
		_size += sizeof(int);

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

	private void PackPacket(ref byte[] _buf, PROTOCOL _protocol, float _posX, float _posZ, out int _size)
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

		// posX
		Buffer.BlockCopy(BitConverter.GetBytes(_posX), 0, buf, offset, sizeof(float));
		offset += sizeof(float);
		_size += sizeof(float);

		// posZ
		Buffer.BlockCopy(BitConverter.GetBytes(_posZ), 0, buf, offset, sizeof(float));
		offset += sizeof(float);
		_size += sizeof(float);

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

	private void PackPacket(ref byte[] _buf, PROTOCOL _protocol, WeaponPacket _weapon, out int _size)
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
	private void PackPacket(ref byte[] _buf, PROTOCOL _protocol, IngamePacket _packet, out int _size)
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

		// 위치
		Buffer.BlockCopy(_packet.Serialize(), 0, buf, offset, Marshal.SizeOf(_packet));
		offset += Marshal.SizeOf(_packet);
		_size += Marshal.SizeOf(_packet);

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
		C_Encrypt.instance.Encrypt(buf, encryptBuf, _size);

		// 가장 앞에 size를 넣고, 그 뒤에 암호화했던 버퍼를 붙임.
		offset = 0;
		Buffer.BlockCopy(BitConverter.GetBytes(_size), 0, _buf, offset, sizeof(int));
		offset += sizeof(int);
		Buffer.BlockCopy(encryptBuf, 0, _buf, offset, _size);
		offset += _size;

		_size += sizeof(int);   // 총 보내야 할 바이트 수 저장한다.
	}

	private void UnPackPacket(byte[] _buf, ref IngamePacket _struct)
	{
		int offset = sizeof(PROTOCOL);
		// 문자열 길이 만큼 생성해서 문자열을 저장함

		byte[] posByte = new byte[Marshal.SizeOf(_struct)];
		Buffer.BlockCopy(_buf, offset, posByte, 0, Marshal.SizeOf(_struct));

		_struct.Deserialize(ref posByte);
	}

	private void UnPackPacket(byte[] _buf, out byte _playerBit)
	{
		int offset = sizeof(PROTOCOL);

		byte[] bitByte = new byte[sizeof(byte)];
		Buffer.BlockCopy(_buf, offset, bitByte, 0, sizeof(byte));
		offset += sizeof(byte);

		_playerBit = bitByte[0];
	}

	private void UnPackPacket(byte[] _buf, out int _playerNum, ref WeaponPacket _struct)
	{
		byte[] arrNum = new byte[sizeof(int)];

		int offset = sizeof(PROTOCOL);

		// 일단 byte 배열로 받고
		Buffer.BlockCopy(_buf, offset, arrNum, 0, sizeof(int));
		offset += sizeof(int);

		// 이제 다시 int로 변환
		_playerNum = BitConverter.ToInt32(arrNum, 0);

		// weapon을 받음
		byte[] weaponByte = new byte[Marshal.SizeOf(_struct)];
		Buffer.BlockCopy(_buf, offset, weaponByte, 0, Marshal.SizeOf(_struct));

		_struct.Deserialize(ref weaponByte);
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
    private void UnPackPacket(byte[] _buf, out int _num1, out int _num2)
    {
        byte[] arrNum = new byte[sizeof(int)];

        int offset = sizeof(PROTOCOL);

        // 일단 byte 배열로 받고
        Buffer.BlockCopy(_buf, offset, arrNum, 0, sizeof(int));
        offset += sizeof(int);

        // 이제 다시 int로 변환
        _num1 = BitConverter.ToInt32(arrNum, 0);



        // 일단 byte 배열로 받고
        Buffer.BlockCopy(_buf, offset, arrNum, 0, sizeof(int));
        offset += sizeof(int);

        // 이제 다시 int로 변환
        _num2 = BitConverter.ToInt32(arrNum, 0);
    }
    private void UnPackPacket(byte[] _buf, out int _playerNum, out float _posX, out float _posZ)
	{
		int offset = sizeof(PROTOCOL);

		// 1. 일단 byte 배열로 받고(playerNum)
		byte[] arr = new byte[sizeof(int)];
		Buffer.BlockCopy(_buf, offset, arr, 0, sizeof(int));
		offset += sizeof(int);

		// 이제 다시 int로 변환
		_playerNum = BitConverter.ToInt32(arr, 0);


		// 1. 일단 byte 배열로 받고(posX)
		arr = new byte[sizeof(float)];
		Buffer.BlockCopy(_buf, offset, arr, 0, sizeof(float));
		offset += sizeof(float);

		// 이제 다시 float로 변환
		_posX = BitConverter.ToSingle(arr, 0);


		// 2. 일단 byte 배열로 받고(posZ)
		Buffer.BlockCopy(_buf, offset, arr, 0, sizeof(float));
		offset += sizeof(float);

		// 이제 다시 float로 변환
		_posZ = BitConverter.ToSingle(arr, 0);
	}

	private void UnPackPacket(byte[] _buf, out int _num, out string _str1)
	{
		// 1. num
		byte[] arrNum = new byte[sizeof(int)];

		int offset = sizeof(PROTOCOL);

		// 일단 byte 배열로 받고
		Buffer.BlockCopy(_buf, offset, arrNum, 0, sizeof(int));
		offset += sizeof(int);

		// 이제 다시 int로 변환
		_num = BitConverter.ToInt32(arrNum, 0);


		// 2. 문자열
		byte[] arrStrsize1 = new byte[sizeof(int)];
		int strsize1 = 0;

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

    private void UnPackPacket(byte[] _buf, out string _str1, out string _str2)
    {
        byte[] arrStrsize = new byte[sizeof(int)];
        int strsize = 0;

        int offset = sizeof(PROTOCOL);

        // 문자열1 길이
        Buffer.BlockCopy(_buf, offset, arrStrsize, 0, sizeof(int));
        offset += sizeof(int);
        strsize = BitConverter.ToInt32(arrStrsize, 0);

        // 문자열1 길이 만큼 생성해서 문자열을 저장함
        byte[] arrStrByte = new byte[strsize];
        Buffer.BlockCopy(_buf, offset, arrStrByte, 0, strsize);
        offset += strsize;

        // 이제 다시 string으로 변환
        _str1 = ByteToString(arrStrByte);


        // 문자열2 길이
        Buffer.BlockCopy(_buf, offset, arrStrsize, 0, sizeof(int));
        offset += sizeof(int);
        strsize = BitConverter.ToInt32(arrStrsize, 0);

        // 문자열2 길이 만큼 생성해서 문자열을 저장함
        arrStrByte = new byte[strsize];
        Buffer.BlockCopy(_buf, offset, arrStrByte, 0, strsize);
        offset += strsize;

        // 이제 다시 string으로 변환
        _str2 = ByteToString(arrStrByte);
    }
    private void UnPackPacket(byte[] _buf, ref GameManager.GameInfo _gameInfo, ref WeaponManager.WeaponInfo[] _weapons)
	{
		int offset = sizeof(PROTOCOL);

		// 1. GameInfo 받음
		byte[] posByte = new byte[Marshal.SizeOf(_gameInfo)];
		Buffer.BlockCopy(_buf, offset, posByte, 0, Marshal.SizeOf(_gameInfo));

		_gameInfo.Deserialize(ref posByte);
		offset += Marshal.SizeOf(_gameInfo);



		// 2. Weapon 갯수 받음
		byte[] arrNum = new byte[sizeof(int)];

		// 일단 byte 배열로 받고
		Buffer.BlockCopy(_buf, offset, arrNum, 0, sizeof(int));

		// 이제 다시 int로 변환
		int numOfWeapon = BitConverter.ToInt32(arrNum, 0);
		offset += sizeof(int);

		// 받은 만큼 배열 할당
		_weapons = new WeaponManager.WeaponInfo[numOfWeapon];


		// 3. WeaponInfo를 위에서 받은 갯수만큼 받아서 _weaponInfo 배열에 저장함
		WeaponManager.WeaponInfo weapon = new WeaponManager.WeaponInfo();
		for (int i = 0; i < numOfWeapon; i++)
		{
			posByte = new byte[Marshal.SizeOf(weapon)];
			Buffer.BlockCopy(_buf, offset, posByte, 0, Marshal.SizeOf(weapon));

			weapon.Deserialize(ref posByte);

			_weapons[i] = weapon;
			offset += Marshal.SizeOf(weapon);
		}
	}
}