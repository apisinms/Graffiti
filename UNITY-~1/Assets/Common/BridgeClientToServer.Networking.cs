using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NetworkManager;

//          BridgeClientToServer.Networking
public partial class BridgeClientToServer : MonoBehaviour
{
    private Vector3 tmpVec;
    private Vector3 tmpAngle;

    public void Initialization_Networking()
    {
        tmpVec = new Vector3();
        tmpAngle = new Vector3();
    }

    // 플레이어 무기 세팅
    public void SetWeapon(int _playerNum, ref WeaponPacket _weapon)
    {
        //무기정보 저장
        WeaponManager.instance.mainWeapon[_playerNum - 1] = (_WEAPONS)_weapon.mainW;
        WeaponManager.instance.subWeapon[_playerNum - 1] = (_WEAPONS)_weapon.subW;
    } 

    // 섹터 진입시
    public void EnterSectorProcess(ref PositionPacket _packet)
    {
        PlayersManager.instance.obj_players[_packet.playerNum - 1].SetActive(true);   // 켜고

        // 이렇게 해줘야 이전 위치랑 보간을 안해서 캐릭터가 슬라이딩 되지 않음
        tmpVec = PlayersManager.instance.obj_players[_packet.playerNum - 1].transform.localPosition;
        tmpAngle = PlayersManager.instance.obj_players[_packet.playerNum - 1].transform.localEulerAngles;

        tmpVec.x = _packet.posX;
        tmpVec.z = _packet.posZ;
        tmpAngle.y = _packet.rotY;

        PlayersManager.instance.obj_players[_packet.playerNum - 1].transform.localPosition = tmpVec;
        PlayersManager.instance.obj_players[_packet.playerNum - 1].transform.localEulerAngles = tmpAngle;
    }

    // 섹터 아웃시
    public void ExitSectorProcess(ref PositionPacket _packet)
    {
        PlayersManager.instance.obj_players[_packet.playerNum - 1].SetActive(false);   // 끄고
    }

    // 플레이어 위치, 카메라 강제 세팅
    public void ForceMoveProcess(ref PositionPacket _packet)
    {
        // 스피드, 애니메이션 0으로
        _packet.speed = 0.0f;
        _packet.action = (int)_ACTION_STATE.IDLE;

        networkManager.SetPosPacket(_packet.playerNum - 1, ref _packet);

        // 원래 플레이어가 가지고 있던 정보를 임시 벡터에 가져옴
        tmpVec = PlayersManager.instance.obj_players[_packet.playerNum - 1].transform.localPosition;
        tmpAngle = PlayersManager.instance.obj_players[_packet.playerNum - 1].transform.localEulerAngles;

        // 변환된 값을 바로 대입
        tmpVec.x = _packet.posX;
        tmpVec.z = _packet.posZ;
        tmpAngle.y = _packet.rotY;

        // 러프없이 강제로 대입해버림
        PlayersManager.instance.obj_players[_packet.playerNum - 1].transform.localPosition = tmpVec;
        PlayersManager.instance.obj_players[_packet.playerNum - 1].transform.localEulerAngles = tmpAngle;

        // 본인일 경우에는 카메라도 강제 셋팅 시켜준다.
        if (_packet.playerNum == networkManager.MyPlayerNum)
        {
            // 카메라도 설정
            if (GameManager.instance.mainCamera != null)
                GameManager.instance.mainCamera.SetCameraPos(0.0f, C_Global.camPosY, C_Global.camPosZ);
        }
    }

    // 다른 플레이어 위치 정보를 얻어옴
    public void GetOtherPlayerPos(ref PositionPacket _packet)
    {
        networkManager.SetPosPacket(_packet.playerNum - 1, ref _packet);   // 패킷 저장해주고

        // 플레이어 위치를 바로 대입해서 셋팅해버림
        tmpVec = PlayersManager.instance.obj_players[_packet.playerNum - 1].transform.localPosition;
        tmpAngle = PlayersManager.instance.obj_players[_packet.playerNum - 1].transform.localEulerAngles;

        tmpVec.x = _packet.posX;
        tmpVec.z = _packet.posZ;
        tmpAngle.y = _packet.rotY;

        PlayersManager.instance.obj_players[_packet.playerNum - 1].transform.localPosition = tmpVec;
        PlayersManager.instance.obj_players[_packet.playerNum - 1].transform.localEulerAngles = tmpAngle;

        // 꺼져있다면 켜준다!
        if (PlayersManager.instance.obj_players[_packet.playerNum - 1].activeSelf == false)
            PlayersManager.instance.obj_players[_packet.playerNum - 1].SetActive(true);
    }

    // 정상적인 이동시
    public void OnMoveSuccess(ref PositionPacket _packet)
    {
        networkManager.SetPosPacket(_packet.playerNum - 1, ref _packet);
    }

    // 다른 플레이어의 접속이 끊겼을때, 플레이어 로빈 오브젝트를 비활성화.
    public void OnOtherPlayerDisconnected(int _quitPlayerNum)
    {
        if (PlayersManager.instance.obj_players[_quitPlayerNum - 1].activeSelf == true)
            PlayersManager.instance.obj_players[_quitPlayerNum - 1].SetActive(false);
    }
}
