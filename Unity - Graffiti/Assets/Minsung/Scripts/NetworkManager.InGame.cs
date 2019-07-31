using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NetworkManager : MonoBehaviour
{
    public void MayIItemSelect(sbyte mainW, sbyte subW)
    {
        // 프로토콜 셋팅
        protocol = SetProtocol(
                STATE_PROTOCOL.INGAME_STATE,
                PROTOCOL.ITEMSELECT_PROTOCOL,
                RESULT.NODATA);

        WeaponPacket weapon = new WeaponPacket();
        weapon.mainW = mainW;
        weapon.subW = subW;

        // 패킹 및 전송
        int packetSize;
        PackPacket(ref sendBuf, protocol, weapon, out packetSize);
        //PackPacket(ref sendBuf, protocol, weapon.mainW, weapon.subW, out packetsize);
        //tcpClient.GetStream().Write(sendBuf, 0, packetSize);
        bw.Write(sendBuf, 0, packetSize);
    }

    public void MayIIMove(float posX, float posZ)
    {
        // 프로토콜 셋팅
        protocol = SetProtocol(
                STATE_PROTOCOL.INGAME_STATE,
                PROTOCOL.MOVE_PROTOCOL,
                RESULT.NODATA);

        PositionPacket position = new PositionPacket();
        position.posX = posX;
        position.posZ = posZ;

        // 패킹 및 전송
        int packetSize;
        PackPacket(ref sendBuf, protocol, position, out packetSize);

        bw.Write(sendBuf, 0, packetSize);
    }

    public bool CheckInGameSuccess()
    {
        if (result == RESULT.INGAME_SUCCESS)
            return true;

        else
            return false;
    }
}
