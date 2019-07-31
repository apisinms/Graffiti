using System;
using UnityEngine;

/// <summary>
/// NetworkManager_Login.cs파일
/// 로그인, 회원가입에 관련된 내용이 있다.
/// </summary>
public partial class NetworkManager : MonoBehaviour
{
	public void MayILogin(string _id, string _pw)
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
	}

	public bool CheckLoginSuccess()
	{
		if (state == STATE_PROTOCOL.LOGIN_STATE &&
			protocol == PROTOCOL.LOGIN_PROTOCOL &&
			result == RESULT.LOGIN_SUCCESS)
			return true;

		else
			return false;
	}
	public bool CheckLogin_IDError()
	{
		if (state == STATE_PROTOCOL.LOGIN_STATE &&
			protocol == PROTOCOL.LOGIN_PROTOCOL &&
			result == RESULT.ID_ERROR)
			return true;

		else
			return false;
	}

	public bool CheckLogin_IDExist()
	{
		if (state == STATE_PROTOCOL.LOGIN_STATE &&
			protocol == PROTOCOL.LOGIN_PROTOCOL &&
			result == RESULT.ID_EXIST)
			return true;

		else
			return false;
	}

	public bool CheckLogin_PWError()
	{
		if (state == STATE_PROTOCOL.LOGIN_STATE &&
			protocol == PROTOCOL.LOGIN_PROTOCOL &&
			result == RESULT.PW_ERROR)
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

	public bool CheckJoin_Success()
	{
		if (state == STATE_PROTOCOL.LOGIN_STATE &&
			protocol == PROTOCOL.JOIN_PROTOCOL &&
			result == RESULT.JOIN_SUCCESS)
			return true;

		else
			return false;
	}
	public bool CheckJoin_IDExist()
	{
		if (state == STATE_PROTOCOL.LOGIN_STATE &&
			protocol == PROTOCOL.JOIN_PROTOCOL &&
			result == RESULT.ID_EXIST)
			return true;

		else
			return false;
	}
}