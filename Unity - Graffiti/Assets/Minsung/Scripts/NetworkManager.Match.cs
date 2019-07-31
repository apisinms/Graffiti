using System;
using UnityEngine;

/// <summary>
/// NetworkManager_Match.cs파일
/// 매칭에 관련된 내용이 있다.
/// </summary>
public partial class NetworkManager : MonoBehaviour
{
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

	public bool CheckMatched()
	{
		if (state    == STATE_PROTOCOL.LOBBY_STATE &&
			protocol == PROTOCOL.GOTO_INGAME_PROTOCOL &&
			result   == RESULT.LOBBY_SUCCESS)
			return true;

		else
			return false;
	}


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

	public bool CheckMatchCancel()
	{
		if (state == STATE_PROTOCOL.LOBBY_STATE &&
			protocol == PROTOCOL.MATCH_CANCEL_PROTOCOL &&
			result == RESULT.LOBBY_SUCCESS)
			return true;

		else
			return false;
	}

	//public bool CheckMatchSuccess()
	//{
	//	if (state == STATE_PROTOCOL.INGAME_STATE && result == RESULT.MATCH_SUCCESS)
	//		return true;

	//	return false;
	//}


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

	public bool CheckItemSelectSuccess()
	{
		if (result == RESULT.INGAME_SUCCESS)
			return true;

		else
			return false;
	}
}