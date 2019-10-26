using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BridgeClientToServer : MonoBehaviour
{
    #region PACKET_COROUTINE
    private Coroutine moveCor = null;
    #endregion

    private void Initialization_PacketCoroutine()
    {
#if NETWORK
      networkManager = NetworkManager.instance;
#endif
    }

    IEnumerator MovePlayer()
    {
        while (true)
        {
			networkManager.SendIngamePacket();


            yield return YieldInstructionCache.WaitForSeconds(C_Global.packetInterval);
        }
    }

    public void StartMoveCoroutine()
    {
        if (moveCor != null)
            StopCoroutine(moveCor);

        moveCor = StartCoroutine(MovePlayer());
    }

    public void StopMoveCoroutine()
    {
		// 한번 더 보냄
		networkManager.SendIngamePacket();
		StopCoroutine(moveCor);
    }
}