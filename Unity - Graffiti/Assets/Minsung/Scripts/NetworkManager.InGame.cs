using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NetworkManager : MonoBehaviour
{
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
		weapon.subW = subW;

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


	public void MayIIMove(float posX, float posZ)
	{
		// 프로토콜 셋팅
		protocol = SetProtocol(
				STATE_PROTOCOL.INGAME_STATE,
				PROTOCOL.MOVE_PROTOCOL,
				RESULT.NODATA);

		PositionPacket position = new PositionPacket();
		position.playerNum = myPlayerNum;
		position.posX = posX;
		position.posZ = posZ;

		// 패킹 및 전송
		int packetSize;
		PackPacket(ref sendBuf, protocol, position, out packetSize);

		bw.Write(sendBuf, 0, packetSize);
	}

	public bool CheckInGameSuccess()
	{
		if (result == RESULT.INGAME_SUCCESS)
			return true;

		else
			return false;
	}
}