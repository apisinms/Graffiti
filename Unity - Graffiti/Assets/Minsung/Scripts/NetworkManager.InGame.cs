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


    public void MayIMove(float _posX, float _posZ, float _rotY)
    {
        // 프로토콜 셋팅
        protocol = SetProtocol(
                STATE_PROTOCOL.INGAME_STATE,
                PROTOCOL.MOVE_PROTOCOL,
                RESULT.NODATA);

        PositionPacket position = new PositionPacket();
        position.playerNum = myPlayerNum;
        position.posX = _posX;
        position.posZ = _posZ;
        position.rotY = _rotY;

        // 패킹 및 전송
        int packetSize;
        PackPacket(ref sendBuf, protocol, position, out packetSize);

        bw.Write(sendBuf, 0, packetSize);
    }

    public bool CheckMove()
	{
		if (state == STATE_PROTOCOL.INGAME_STATE &&
			protocol == PROTOCOL.MOVE_PROTOCOL &&
			result == RESULT.INGAME_SUCCESS)
		{
			result = RESULT.NODATA;
			return true;
		}

		else
			return false;
	}

	// 게임을 나간 플레이어가 있는지 조회
	public int CheckQuit()
	{
		// 나간 플레이어가 있을 시에
		if (state == STATE_PROTOCOL.INGAME_STATE &&
			protocol == PROTOCOL.DISCONNECT_PROTOCOL &&
			result == RESULT.INGAME_SUCCESS)
		{
			// 해당 플레이어 넘버를 리턴하고, 다시 -1로 셋팅한다.
			int ret = quitPlayerNum;
			quitPlayerNum = -1;
			return ret;
		}

		// 없으면 호출해도 음수 리턴
		else
			return -1;
	}
}