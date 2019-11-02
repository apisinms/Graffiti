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

    // 서버로 Loading 됐다는 프로토콜을 보내준다.
    public void SendLoadingComplete()
    {
        int packetSize = 0;

        // 로딩 완료 프로토콜
        PROTOCOL loadingProtocol = SetProtocol(
           STATE_PROTOCOL.INGAME_STATE,
           PROTOCOL.LOADING_PROTOCOL,
           RESULT.INGAME_SUCCESS);

        // 로딩 완료 전송
        PackPacket(ref sendBuf, loadingProtocol, out packetSize);
        bw.Write(sendBuf, 0, packetSize);
    }

    PlayersManager playersManager;
    IngamePacket ingameSendPacket = new IngamePacket();
    public void SendIngamePacket(bool _isInit = false)
    {
        if (playersManager == null)
        {
            playersManager = PlayersManager.instance;
        }


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
                PROTOCOL.UPDATE_PROTOCOL,
                RESULT.NODATA);
        }

        ingameSendPacket.playerNum = myPlayerNum;
        ingameSendPacket.posX = playersManager.tf_players[myPlayerNum - 1].localPosition.x;
        ingameSendPacket.posZ = playersManager.tf_players[myPlayerNum - 1].localPosition.z;
        ingameSendPacket.rotY = playersManager.tf_players[myPlayerNum - 1].localEulerAngles.y;
        ingameSendPacket.speed = playersManager.speed[myPlayerNum - 1];
        ingameSendPacket.action = (int)playersManager.actionState[myPlayerNum - 1];
        ingameSendPacket.health = playersManager.hp[myPlayerNum - 1];
        ingameSendPacket.isReloading = WeaponManager.instance.isReloading;//[myPlayerNum - 1];
        ingameSendPacket.collisionChecker = WeaponManager.instance.GetCollisionChecker();

        // 패킹 및 전송
        int packetSize;
        PackPacket(ref sendBuf, protocol, ingameSendPacket, out packetSize);

        bw.Write(sendBuf, 0, packetSize);

        WeaponManager.instance.ResetCollisionChecker(); // 충돌체커 초기화
    }

    // 포커스 바꾼다고 서버로 전송
    public void MayIChangeFocus(bool _focus)
    {
        if (_focus == false)
        {
            bridge.pathSpawner.TurnOffAllPrefabs(); // 생성된 자동차 프리팹 다 끔
        }

        // 프로토콜 셋팅(포커스 없어짐)
        protocol = SetProtocol(
            STATE_PROTOCOL.INGAME_STATE,
            PROTOCOL.FOCUS_PROTOCOL,
            (_focus == true
            ? RESULT.INGAME_SUCCESS
            : RESULT.INGAME_FAIL));

        // 패킹 및 전송
        int packetSize;
        PackPacket(ref sendBuf, protocol, out packetSize);
        bw.Write(sendBuf, 0, packetSize);
    }

    // 나 차에 치임
    public void ImHitByCar(Vector3 _force)
    {
        // 프로토콜 셋팅(차에 치임)
        protocol = SetProtocol(
            STATE_PROTOCOL.INGAME_STATE,
            PROTOCOL.UPDATE_PROTOCOL,
            RESULT.CAR_HIT);

        // 패킹 및 전송
        int packetSize;
        PackPacket(ref sendBuf, protocol, _force.x, _force.z, out packetSize);
        bw.Write(sendBuf, 0, packetSize);
    }

    public void SendRespawnPacket(ref Transform _respawnSpot)
    {
        // 리스폰 프로토콜 셋팅
        protocol = SetProtocol(
            STATE_PROTOCOL.INGAME_STATE,
            PROTOCOL.UPDATE_PROTOCOL,
            RESULT.RESPAWN);


        ingameSendPacket.playerNum = myPlayerNum;
        ingameSendPacket.posX = _respawnSpot.localPosition.x;   // 내가 리스폰 할 위치를 서버에 보내준다.
        ingameSendPacket.posZ = _respawnSpot.localPosition.z;
        ingameSendPacket.rotY = _respawnSpot.localEulerAngles.y;
        ingameSendPacket.speed = playersManager.speed[myPlayerNum - 1];
        ingameSendPacket.action = (int)playersManager.actionState[myPlayerNum - 1];
        ingameSendPacket.health = playersManager.hp[myPlayerNum - 1];
        ingameSendPacket.collisionChecker = BulletCollisionChecker.GetDummy();   // 더미

        // 패킹 및 전송
        int packetSize;
        PackPacket(ref sendBuf, protocol, ingameSendPacket, out packetSize);

        bw.Write(sendBuf, 0, packetSize);
    }

    // 로비로 갈래
    public void SendGotoLobby()
    {
        int packetSize = 0;

        // 로비가기 프로토콜
        protocol = SetProtocol(
           STATE_PROTOCOL.INGAME_STATE,
           PROTOCOL.GOTO_LOBBY_PROTOCOL,
           RESULT.INGAME_SUCCESS);

        // 로비가기 프로토콜 전송
        PackPacket(ref sendBuf, protocol, out packetSize);
        bw.Write(sendBuf, 0, packetSize);
    }

    // 점령 성공했다고 보낸다.
    public void SendCaptureSuccess(int _buildingIdx)
    {
        int packetSize = 0;

        // 점령성공 프로토콜
        protocol = SetProtocol(
           STATE_PROTOCOL.INGAME_STATE,
           PROTOCOL.CAPTURE_PROTOCOL,
           RESULT.INGAME_SUCCESS);

        // 점령성공 프로토콜 전송(건물 인덱스도 같이)
        PackPacket(ref sendBuf, protocol, _buildingIdx, out packetSize);
        bw.Write(sendBuf, 0, packetSize);

    }
}