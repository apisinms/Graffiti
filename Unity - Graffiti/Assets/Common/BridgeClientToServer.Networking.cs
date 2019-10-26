using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static NetworkManager;
using static WeaponManager;

//          BridgeClientToServer.Networking
public partial class BridgeClientToServer : MonoBehaviour
{
    private Vector3 tmpVec;
    private Vector3 tmpAngle;
    private GameInfo tmpGameInfo;
    private WeaponInfo[] tmpWeapons;


    public PathCreation.Examples.PathSpawner pathSpawner;

    public GameInfo GetTempGameInfo { get { return tmpGameInfo; } }
    public WeaponInfo[] GetTempWeapons { get { return tmpWeapons; } }

    public void Initialization_Networking()
    {
        tmpVec = new Vector3();
        tmpAngle = new Vector3();
    }

    // 플레이어 무기 세팅
    public void SetWeapon(int _playerNum, ref WeaponPacket _weapon)
    {
        int index = _playerNum - 1;

        //무기정보 저장
        weaponManager.mainWeapon[index] = (_WEAPONS)_weapon.mainW;
        weaponManager.subWeapon[index] = (_WEAPONS)_weapon.subW;

        //주무기 세팅 스테이트패턴
        switch (weaponManager.mainWeapon[index])
        {
            case _WEAPONS.AR:
                weaponManager.SetMainWeapon(Main_AR.GetMainWeaponInstance(), index);
                break;
            case _WEAPONS.SG:
                weaponManager.SetMainWeapon(Main_SG.GetMainWeaponInstance(), index);
                break;
            case _WEAPONS.SMG:
                weaponManager.SetMainWeapon(Main_SMG.GetMainWeaponInstance(), index);
                break;
        }

        //보조무기세팅 마찬가지로 같은식. 나중에 추가.
        /*

          */

        EffectManager.instance.InitializeMuzzle(index);

		if (index == myIndex)
			UIManager.instance.Initialization_WeaponInfo_2();
	}

    public void SetPlayerNickName(string _nickName, int _idx)
    {
        uiManager.SetNickname(_nickName, _idx);
    }

    // 최초에 무기정보, 게임정보, 자동차 씨드를 bridg의 멤버로 설정(GameManager가 인스턴스 생성되고 나서 저장해야되므로;)
    public void SetGameInfoToBridge(ref GameInfo _gameInfo, ref WeaponInfo[] _weapons)
    {
        tmpGameInfo = _gameInfo;
        tmpWeapons = _weapons;
    }

    // 섹터 진입시
    public void EnterSectorProcess(ref IngamePacket _packet)
    {
        playersManager.obj_players[_packet.playerNum - 1].SetActive(true);   // 켜고

        // 이렇게 해줘야 이전 위치랑 보간을 안해서 캐릭터가 슬라이딩 되지 않음
        tmpVec = playersManager.obj_players[_packet.playerNum - 1].transform.localPosition;
        tmpAngle = playersManager.obj_players[_packet.playerNum - 1].transform.localEulerAngles;

        tmpVec.x = _packet.posX;
        tmpVec.z = _packet.posZ;
        tmpAngle.y = _packet.rotY;

        playersManager.obj_players[_packet.playerNum - 1].transform.localPosition = tmpVec;
        playersManager.obj_players[_packet.playerNum - 1].transform.localEulerAngles = tmpAngle;
    }

    // 섹터 아웃시
    public void ExitSectorProcess(ref IngamePacket _packet)
    {
        playersManager.obj_players[_packet.playerNum - 1].SetActive(false);   // 끄고
    }

    public void UpdatePlayerProcess(byte _playerBit)
    {
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
                // 꺼져있는 플레이어라면 위치를 넘버로 요청한다.(얘네들만 갱신해주면 됨)
                if (playersManager.obj_players[i].activeSelf == false)
                    networkManager.RequestOtherPlayerStatus(i + 1);

                // 이미 켜져있으면 굳이 얻어올 필요 없음
                else
                    continue;
            }

            // 섹터에 포함되어 있지 않다면 오브젝트를 꺼준다.(오브젝트가 켜진 경우만)
            else
            {
                if (playersManager.obj_players[i].activeSelf == true)
                    playersManager.obj_players[i].SetActive(false);
            }
        }
    }

    // 플레이어 위치, 카메라 강제 세팅
    public void ForceMoveProcess(ref IngamePacket _packet)
    {
        // 스피드, 애니메이션 0으로
        _packet.speed = 0.0f;
        _packet.action = (int)_ACTION_STATE.IDLE;

        networkManager.SetIngamePacket(_packet.playerNum - 1, ref _packet);

        HealthChanger(ref _packet); // 체력 및 UI 설정

        //그 다음 총알도 설정

        // 원래 플레이어가 가지고 있던 정보를 임시 벡터에 가져옴
        tmpVec = playersManager.obj_players[_packet.playerNum - 1].transform.localPosition;
        tmpAngle = playersManager.obj_players[_packet.playerNum - 1].transform.localEulerAngles;

        // 변환된 값을 바로 대입
        tmpVec.x = _packet.posX;
        tmpVec.z = _packet.posZ;
        tmpAngle.y = _packet.rotY;

        // 러프없이 강제로 대입해버림
        playersManager.obj_players[_packet.playerNum - 1].transform.localPosition = tmpVec;
        playersManager.obj_players[_packet.playerNum - 1].transform.localEulerAngles = tmpAngle;

        // 본인일 경우에는 카메라도 강제 셋팅 시켜준다.
        if (_packet.playerNum == networkManager.MyPlayerNum)
        {
            // 카메라도 설정
            if (gameManager.mainCamera != null)
                gameManager.mainCamera.SetCameraPos(0.0f, C_Global.camPosY, C_Global.camPosZ);
        }
    }

    // 다른 플레이어 정보를 얻어옴
    public void GetOtherPlayerStatus(ref IngamePacket _packet)
    {
        networkManager.SetIngamePacket(_packet.playerNum - 1, ref _packet);   // 패킷 저장해주고
        HealthChanger(ref _packet);   // 체력 및 UI 설정

        //그 다음 총알도 설정


        // 플레이어 위치를 바로 대입해서 셋팅해버림
        tmpVec = playersManager.obj_players[_packet.playerNum - 1].transform.localPosition;
        tmpAngle = playersManager.obj_players[_packet.playerNum - 1].transform.localEulerAngles;

        tmpVec.x = _packet.posX;
        tmpVec.z = _packet.posZ;
        tmpAngle.y = _packet.rotY;

        playersManager.obj_players[_packet.playerNum - 1].transform.localPosition = tmpVec;
        playersManager.obj_players[_packet.playerNum - 1].transform.localEulerAngles = tmpAngle;

        // 꺼져있다면 켜준다!
        if (playersManager.obj_players[_packet.playerNum - 1].activeSelf == false)
            playersManager.obj_players[_packet.playerNum - 1].SetActive(true);
    }

    public void HealthChanger(ref IngamePacket _packet)
    {
        int absoluteIdx = uiManager.PlayerIndexToAbsoluteIndex(_packet.playerNum - 1);

        uiManager.HealthUIChanger(absoluteIdx, _packet.health); // 실제 체력 UI 적용
        playersManager.hp[_packet.playerNum - 1] = _packet.health;  // 실제 피 세팅
        networkManager.SetIngamePacket(_packet.playerNum - 1, ref _packet);

        // 체력 0이하가 되면 죽음 처리
        if (_packet.health <= 0.0f)
        {
            // 내가 죽은 경우
            if ((_packet.playerNum - 1) == myIndex)
            {
                StateManager.instance.Death(true); //내가 죽은상태로 전환.
                uiManager.SetDeadUI(); // 죽은 UI로 전환
            }

            // 다른 플레이어가 죽은 경우
            else
            {
                gameManager.SetLocalAndNetworkActionState(_packet.playerNum - 1, _ACTION_STATE.DEATH);
                uiManager.OffPlayerUI(_packet.playerNum - 1);
            }
        }
    }

    public void RespawnProcess(ref IngamePacket _packet)
    {
        ForceMoveProcess(ref _packet);  // 강제 이동 실시

        // 내가 죽었던 거면 UI 켜줌
        if ((_packet.playerNum - 1) == myIndex)
        {
            StateManager.instance.Idle(true);
            uiManager.SetAliveUI();
        }

        // 다른 플레이어가 죽었던거면 체력, 서클 등 켜줌
        else
        {
            gameManager.SetLocalAndNetworkActionState(_packet.playerNum - 1, _ACTION_STATE.IDLE);
            int absoluteIdx = uiManager.PlayerIndexToAbsoluteIndex(_packet.playerNum - 1);
            uiManager.OnPlayerUI(absoluteIdx);
        }

		// 남아있는 Addforce 초기화
		playersManager.obj_players[_packet.playerNum - 1].GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void CarSpawn(int _seed)
    {
        // 시드 꼽고 차 생성
        UnityEngine.Random.InitState(_seed);
        pathSpawner.SpawnPrefabs(); // 차 생성
    }

	public void OtherPlayerHitByCar(int _playerNum, float _posX, float _posZ)
	{
		// 맞은 놈 튕기게 하고
		Rigidbody rigid = playersManager.obj_players[_playerNum - 1].GetComponent<Rigidbody>();
		Vector3 force = new Vector3(_posX, 0.0f, _posZ);
		rigid.AddForce(force);

		// 상태 변경
		gameManager.SetLocalAndNetworkActionState(_playerNum - 1, _ACTION_STATE.DEATH);

		// 체력 UI 적용(차는 즉사)
		int absoluteIdx = uiManager.PlayerIndexToAbsoluteIndex(_playerNum - 1);
		uiManager.HealthUIChanger(absoluteIdx, 0.0f);
	}

	// 정상적인 업데이트 시
	public void OnUpdate(ref IngamePacket _packet)
    {
        networkManager.SetIngamePacket(_packet.playerNum - 1, ref _packet);
    }

    // 다른 플레이어의 접속이 끊겼을때, 플레이어 로빈 오브젝트를 비활성화.
    public void OnOtherPlayerDisconnected(int _quitPlayerNum)
    {
        if (playersManager.obj_players[_quitPlayerNum - 1].activeSelf == true)
        {
            playersManager.obj_players[_quitPlayerNum - 1].SetActive(false);

            int absoluteIdx = uiManager.PlayerIndexToAbsoluteIndex(_quitPlayerNum - 1);
            uiManager.OffPlayerUI(absoluteIdx);
        }
    }

    //쐇을때 무조건 1번의 패킷을 보내야됨. 보정용
}