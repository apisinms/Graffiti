﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//            BridgeClientToServer.PacketCoroutine //플레이어 매니저 맨밑에 패킷코루틴부분이 여기로옴
public partial class BridgeClientToServer : MonoBehaviour
{
    #region PACKET_COROUTINE
    private IEnumerator moveCor;
    #endregion

    private void Initialization_PacketCoroutine()
    {
#if NETWORK
      networkManager = NetworkManager.instance;
      moveCor = MovePlayer();
#endif
    }

    IEnumerator MovePlayer()
    {
        while (true)
        {
            networkManager.SendPosition(playersManager.tf_players[myIndex].localPosition.x,
				playersManager.tf_players[myIndex].localPosition.z,
				playersManager.tf_players[myIndex].localEulerAngles.y, playersManager.speed[myIndex], playersManager.actionState[myIndex]);

            yield return YieldInstructionCache.WaitForSeconds(C_Global.packetInterval);
        }
    }

    public void StartMoveCoroutine()
    {
        StartCoroutine(moveCor);
    }

    public void StopMoveCoroutine()
    {
        // 코루틴 정지시에 마지막 위치를 보내준다.
        networkManager.SendPosition(playersManager.tf_players[myIndex].localPosition.x,
            playersManager.tf_players[myIndex].localPosition.z,
            playersManager.tf_players[myIndex].localEulerAngles.y, playersManager.speed[myIndex], playersManager.actionState[myIndex]);

        StopCoroutine(moveCor);
    }

	// 패킷을 1회 보내준다.
	public void SendPacket()
	{
		networkManager.SendPosition(playersManager.tf_players[myIndex].localPosition.x,
		playersManager.tf_players[myIndex].localPosition.z,
		playersManager.tf_players[myIndex].localEulerAngles.y, playersManager.speed[myIndex], playersManager.actionState[myIndex]);
	}
}
