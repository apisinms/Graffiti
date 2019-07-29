using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
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
}