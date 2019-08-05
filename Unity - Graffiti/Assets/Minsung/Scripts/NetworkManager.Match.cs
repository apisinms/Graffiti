using System;
using UnityEngine;

/// <summary>
/// NetworkManager_Match.cs파일
/// 매칭에 관련된 내용이 있다.
/// </summary>
public partial class NetworkManager : MonoBehaviour
{
	// 매칭을 원한다고 서버로 전송
	public void MayIMatch()
	{
		// 프로토콜 셋팅
		protocol = SetProtocol(
				STATE_PROTOCOL.LOBBY_STATE,
				PROTOCOL.MATCH_PROTOCOL,
				RESULT.NODATA);

		// 패킹 및 전송
		int packetSize;
		PackPacket(ref sendBuf, protocol, out packetSize);
		bw.Write(sendBuf, 0, packetSize);
	}

	// 매칭이 잡혔는지 조회
	public bool CheckMatched()
	{
		if (state    == STATE_PROTOCOL.LOBBY_STATE &&
			protocol == PROTOCOL.GOTO_INGAME_PROTOCOL &&
			result   == RESULT.LOBBY_SUCCESS)
			return true;

		else
			return false;
	}

	// 매칭 취소를 원한다고 서버로 전송
	public void MayICancelMatch()
	{
		// 프로토콜 셋팅
		protocol = SetProtocol(
				STATE_PROTOCOL.LOBBY_STATE,
				PROTOCOL.MATCH_CANCEL_PROTOCOL,
				RESULT.NODATA);

		// 패킹 및 전송
		int packetSize;
		PackPacket(ref sendBuf, protocol, out packetSize);
		bw.Write(sendBuf, 0, packetSize);
	}

	// 매칭 취소가 성공적으로 처리됐는지 조회
	public bool CheckMatchCancel()
	{
		if (state == STATE_PROTOCOL.LOBBY_STATE &&
			protocol == PROTOCOL.MATCH_CANCEL_PROTOCOL &&
			result == RESULT.LOBBY_SUCCESS)
			return true;

		else
			return false;
	}

	// 무기 선택 정보를 서버로 전송(30초 후)
	public void MayISelectWeapon(sbyte mainW, sbyte subW)
	{
		// 프로토콜 셋팅
		protocol = SetProtocol(
				STATE_PROTOCOL.INGAME_STATE,
				PROTOCOL.WEAPON_PROTOCOL,
				RESULT.NODATA);

		WeaponPacket weapon = new WeaponPacket();
		weapon.mainW = mainW;
		weapon.subW  = subW;

		// 패킹 및 전송
		int packetSize;
		PackPacket(ref sendBuf, protocol, weapon, out packetSize);
		bw.Write(sendBuf, 0, packetSize);
	}

	// 서버로 전송했던 무기 정보가 성공적으로 전달됐는지 조회
	public bool CheckWeaponSelectSuccess()
	{
		if (state == STATE_PROTOCOL.INGAME_STATE &&
			protocol == PROTOCOL.START_PROTOCOL &&
			result == RESULT.INGAME_SUCCESS)
			return true;

		else
			return false;
	}

	// 1초마다 넘겨오는 무기선택 타이머 조회
	public bool CheckTimer(string _beforeTime)
	{
		// 타이머 프로토콜일 때
		if (state == STATE_PROTOCOL.INGAME_STATE &&
			protocol == PROTOCOL.TIMER_PROTOCOL)
		{
			// 이전 시간과 같지 않다면 "~초"를 지속적으로 업데이트
			if (string.Compare(_beforeTime, sysMsg) != 0)
				return true;
		}

		return false;
	}

	// 타이머 끝났는지 조회
	public bool CheckTimerEnd()
	{
		if (state == STATE_PROTOCOL.INGAME_STATE &&
			protocol == PROTOCOL.WEAPON_PROTOCOL)
		{
			return true;
		}

		else
			return false;
	}
}