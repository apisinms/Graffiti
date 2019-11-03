using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BridgeClientToServer : MonoBehaviour
{
    #region PACKET_COROUTINE
    private Coroutine moveCor = null;
    //private Coroutine interpolateCor = null;
    //public bool intervalChecker { get; set; }
    #endregion

    private void Initialization_PacketCoroutine()
    {
#if NETWORK
        networkManager = NetworkManager.instance;
#endif
    }

    /*
    IEnumerator Cor_InterpolatePacket(bool _whichLeftRight)
    {
        while (true)
        {
            if (_whichLeftRight == false && intervalChecker == true)
            {
                Debug.Log("보냈다");
                networkManager.SendIngamePacket();
                yield break;
            }

            yield return null;
        }
    }

    public void StartInterpolateCor(bool _whichLeftRight)
    {
        if (interpolateCor != null)
            StopCoroutine(interpolateCor);

        interpolateCor = StartCoroutine(Cor_InterpolatePacket(_whichLeftRight));
    }

    public void StopInterpolateCor()
    {
        SendPacketOnce();
        StopCoroutine(interpolateCor);
    }
    */

    IEnumerator Cor_MovePlayer()
    {
        while (true)
        {
            networkManager.SendIngamePacket();
            //intervalChecker = true;
            yield return YieldInstructionCache.WaitForSeconds(C_Global.packetInterval);
            //intervalChecker = false;
        }
    }

    public void StartMoveCor()
    {
        if (moveCor != null)
            StopCoroutine(moveCor);

        moveCor = StartCoroutine(Cor_MovePlayer());
    }

    public void StopMoveCor()
    {
        SendPacketOnce();
        StopCoroutine(moveCor);
    }
}