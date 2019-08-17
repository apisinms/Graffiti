using System;
using UnityEngine;

/// <summary>
/// NetworkManager_Login.cs파일
/// 로그인, 회원가입에 관련된 내용이 있다.
/// </summary>
public partial class NetworkManager : UnityEngine.MonoBehaviour
{
	// 로그인 정보 서버로 전송
	public void MayILogin(string _id, string _pw)
	{
		// 프로토콜 셋팅
		protocol = SetProtocol(
				STATE_PROTOCOL.LOGIN_STATE,
				PROTOCOL.LOGIN_PROTOCOL,
				RESULT.NODATA);

		// 패킹 및 전송
		int packetSize;
		PackPacket(ref sendBuf, protocol, _id, _pw, out packetSize);
		bw.Write(sendBuf, 0, packetSize);
	}

	// 로그인 성공 조회
	public bool CheckLoginSuccess()
	{
		if (state == STATE_PROTOCOL.LOGIN_STATE &&
			protocol == PROTOCOL.LOGIN_PROTOCOL &&
			result == RESULT.LOGIN_SUCCESS)
			return true;

		else
			return false;
	}

	// 로그인 아이디 없음 조회
	public bool CheckLogin_IDError()
	{
		if (state == STATE_PROTOCOL.LOGIN_STATE &&
			protocol == PROTOCOL.LOGIN_PROTOCOL &&
			result == RESULT.ID_ERROR)
			return true;

		else
			return false;
	}

	// 로그인 아이디 이미 로그인 조회
	public bool CheckLogin_IDExist()
	{
		if (state == STATE_PROTOCOL.LOGIN_STATE &&
			protocol == PROTOCOL.LOGIN_PROTOCOL &&
			result == RESULT.ID_EXIST)
			return true;

		else
			return false;
	}

	// 로그인 패스워드 에러 조회
	public bool CheckLogin_PWError()
	{
		if (state == STATE_PROTOCOL.LOGIN_STATE &&
			protocol == PROTOCOL.LOGIN_PROTOCOL &&
			result == RESULT.PW_ERROR)
			return true;

		else
			return false;
	}

	// 회원가입 정보(아이디, 패스워드, 닉네임) 전송
	public void MayIJoin(string _id, string _pw, string _nickname)
	{
		// 프로토콜 셋팅
		protocol = SetProtocol(
				STATE_PROTOCOL.LOGIN_STATE,
				PROTOCOL.JOIN_PROTOCOL,
				RESULT.NODATA);

		// 패킹 및 전송
		int packetSize;
		PackPacket(ref sendBuf, protocol, _id, _pw, _nickname, out packetSize);
		bw.Write(sendBuf, 0, packetSize);
	}

	// 회원가입 성공 조회
	public bool CheckJoin_Success()
	{
		if (state == STATE_PROTOCOL.LOGIN_STATE &&
			protocol == PROTOCOL.JOIN_PROTOCOL &&
			result == RESULT.JOIN_SUCCESS)
			return true;

		else
			return false;
	}

	// 이미 있는 아이디 조회
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