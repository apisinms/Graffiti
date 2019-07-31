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
}