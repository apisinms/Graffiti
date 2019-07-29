﻿using System;
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
		Console.WriteLine((Int64)protocol);

		// 패킹 및 전송
		int packetSize;
		PackPacket(ref sendBuf, protocol, out packetSize);
		bw.Write(sendBuf, 0, packetSize);
	}

	public bool CheckMatched()
	{
		if (protocol == PROTOCOL.START_PROTOCOL)
			return true;

		return false;
	}

	public bool CheckMatchSuccess()
	{
		if (result == RESULT.MATCH_SUCCESS)
			return true;

		return false;
	}


	public void MayIItemSelect(sbyte mainW, sbyte subW)
	{
		// 프로토콜 셋팅
		protocol = SetProtocol(
				STATE_PROTOCOL.INGAME_STATE,
				PROTOCOL.ITEMSELECT_PROTOCOL,
				RESULT.NODATA);

		WeaponPacket weapon = new WeaponPacket();
		weapon.mainW = mainW;
		weapon.subW  = subW;

		// 패킹 및 전송
		int packetsize;
		PackPacket(ref sendBuf, protocol, weapon, out packetsize);
		//PackPacket(ref sendBuf, protocol, weapon.mainW, weapon.subW, out packetsize);
		bw.Write(sendBuf, 0, packetsize);
	}

	public bool CheckItemSelectSuccess()
	{
		if (result == RESULT.INGAME_SUCCESS)
			return true;

		else
			return false;
	}
}