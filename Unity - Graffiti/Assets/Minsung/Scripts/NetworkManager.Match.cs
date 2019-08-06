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

}