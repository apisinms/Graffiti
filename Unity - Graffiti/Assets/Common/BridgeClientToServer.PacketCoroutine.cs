using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//            BridgeClientToServer.PacketCoroutine //플레이어 매니저 맨밑에 패킷코루틴부분이 여기로옴
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
            networkManager.SendPosition(playersManager.tf_players[myIndex].localPosition.x,
				playersManager.tf_players[myIndex].localPosition.z,
				playersManager.tf_players[myIndex].localEulerAngles.y, playersManager.speed[myIndex], playersManager.actionState[myIndex]);

			/// TODO : 여기에 BulletCollisionCheck 구조체를 초기화 하는 코드를 넣는다.
			/// WeaponManager.instance.ResetCollisionChecker();

            yield return YieldInstructionCache.WaitForSeconds(C_Global.packetInterval);
        }
    }

    public void StartMoveCoroutine()
    {
        if(moveCor != null)
            StopCoroutine(moveCor);

        moveCor = StartCoroutine(MovePlayer());
    }

    public void StopMoveCoroutine()
    {
        /*
        // 코루틴 정지시에 마지막 위치를 보내준다.
        networkManager.SendPosition(playersManager.tf_players[myIndex].localPosition.x,
            playersManager.tf_players[myIndex].localPosition.z,
            playersManager.tf_players[myIndex].localEulerAngles.y, playersManager.speed[myIndex], playersManager.actionState[myIndex]);
            */
        
        SendPacketOnce();

        StopCoroutine(moveCor);
    }
}
