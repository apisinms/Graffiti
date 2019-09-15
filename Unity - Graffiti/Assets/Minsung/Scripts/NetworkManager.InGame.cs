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

	PositionPacket position = new PositionPacket();
	public void SendPosition(float _posX, float _posZ, float _rotY, float _speed ,_ACTION_STATE _action, bool _isInit = false)
    {
        // 초기 위치 보낼 때
        if (_isInit == true)
        {
            // 시작 프로토콜 셋팅
            protocol = SetProtocol(
                  STATE_PROTOCOL.INGAME_STATE,
                  PROTOCOL.START_PROTOCOL,
                  RESULT.NODATA);
        }

        else
        {
            // MOVE 프로토콜 셋팅
            protocol = SetProtocol(
                  STATE_PROTOCOL.INGAME_STATE,
                  PROTOCOL.MOVE_PROTOCOL,
                  RESULT.NODATA);
        }

        //PositionPacket position = new PositionPacket();
        position.playerNum = myPlayerNum;
        position.posX = _posX;
        position.posZ = _posZ;
        position.rotY = _rotY;
        position.speed = _speed;
        position.action = (int)_action;

        // 패킹 및 전송
        int packetSize;
        PackPacket(ref sendBuf, protocol, position, out packetSize);

        bw.Write(sendBuf, 0, packetSize);
    }

	// 포커스 바꾼다고 서버로 전송
	public void MayIChangeFocus(bool _focus)
	{
		// 프로토콜 셋팅(포커스 없어짐)
		protocol = SetProtocol(
				STATE_PROTOCOL.INGAME_STATE,
				PROTOCOL.FOCUS_PROTOCOL,
				( _focus == true 
				? RESULT.INGAME_SUCCESS
				: RESULT.INGAME_FAIL));

		// 패킹 및 전송
		int packetSize;
		PackPacket(ref sendBuf, protocol, out packetSize);
		bw.Write(sendBuf, 0, packetSize);
	}
}
