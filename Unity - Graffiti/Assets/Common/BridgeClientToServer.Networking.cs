using System;
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

		WeaponManager.instance.CreateWeapon(_playerNum - 1);
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

	public void UpdatePlayerProcess(byte _playerBit)
	{
		//Array.Clear(playerInSector, 0, playerInSector.Length);	// 섹터에 있는 플레이어 갱신해야되니까 다 지우고 시작.

		// 마스크 만들어서 어떤 플레이어가 같은 섹터에 있는지 확인하고, 오브젝트를 켜고 끔
		byte bitMask = (byte)C_Global.PLAYER_BIT.PLAYER_1;
		for (int i = 0; i < C_Global.MAX_PLAYER; i++, bitMask >>= 1)
		{
			// 본인은 걍 건너 뜀
			if ((networkManager.MyPlayerNum - 1) == i)
				continue;

			// 섹터에 포함되어있는 플레이어인데
			if ((_playerBit & bitMask) > 0)
			{
				//playerInSector[i] = true;   // 포함되어 있으면 이 플레이어 true로 만듦

				// 꺼져있는 플레이어라면 위치를 넘버로 요청한다.(얘네들만 갱신해주면 됨)
				if (PlayersManager.instance.obj_players[i].activeSelf == false)
					networkManager.RequestOtherPlayerPos(i + 1);

				// 이미 켜져있으면 굳이 얻어올 필요 없음
				else
					continue;
			}

			// 섹터에 포함되어 있지 않다면 오브젝트를 꺼준다.(오브젝트가 켜진 경우만)
			else
			{
				//playerInSector[i] = false;

				if (PlayersManager.instance.obj_players[i].activeSelf == true)
					PlayersManager.instance.obj_players[i].SetActive(false);
			}
		}
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

    //쐇을때 무조건 1번의 패킷을 보내야됨. 보정용
    public void SendPacketOnce()
    {
        networkManager.SendPosition(playersManager.tf_players[myIndex].localPosition.x,
        playersManager.tf_players[myIndex].localPosition.z,
        playersManager.tf_players[myIndex].localEulerAngles.y, playersManager.speed[myIndex], playersManager.actionState[myIndex]);
    }
}
