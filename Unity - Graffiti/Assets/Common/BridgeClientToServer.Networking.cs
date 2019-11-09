﻿using KetosGames.SceneTransition;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
		playersManager.nickname[_idx] = _nickName;

		// 닉네임 받을 때 다시 켜줌
#if NETWORK
		playersManager.obj_players[_idx].SetActive(true);
		uiManager.OnPlayerUI(uiManager.PlayerIndexToAbsoluteIndex(_idx));
#endif
	}

	// 최초에 무기정보, 게임정보, 자동차 씨드를 bridg의 멤버로 설정(GameManager가 인스턴스 생성되고 나서 저장해야되므로;)
	public void SetGameInfoToBridge(ref GameInfo _gameInfo, ref WeaponInfo[] _weapons)
	{
		tmpGameInfo = _gameInfo;
		tmpWeapons = _weapons;
	}

	public void ReadyStart()
	{
		//여기
		/// 레디 시작했을때 잠깐 조작 못하게 막아놓음
	}

	public void ReadyEnd()
	{
		StartCoroutine(uiManager.StartGameTimer());

		//여기
		/// 레디 끝났을때 다시 조작하게 풀고 문구 안보이게
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
		for (int i = 0; i < gameManager.gameInfo.maxPlayer; i++, bitMask >>= 1)
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

		/*
        // 체력 0이하가 되면 죽음 처리
        if (_packet.health <= 0.0f)
        {
            // 내가 죽은 경우
            if ((_packet.playerNum - 1) == myIndex)
            {
                StateManager.instance.Death(true); //내가 죽은상태로 전환.
                uiManager.SetDeadUI("당신이(가) 죽었습니다!");
            }

            // 다른 플레이어가 죽은 경우
            else
            {
                gameManager.SetLocalAndNetworkActionState(_packet.playerNum - 1, _ACTION_STATE.DEATH);
                uiManager.OffPlayerUI(_packet.playerNum - 1);
            }
        }
        */
	}

	public void KillProcess(int _killer, int _victim) //누군가 죽으면 무조건 호출
	{
		uiManager.EnqueueKillLog_Player(_killer, _victim); //살인사건시 킬러와 빅팀의 넘버를 킬로그 큐로 보냄
		uiManager.SetScore(_killer, gameManager.gameInfo.killPoint); //인덱스로 팀판별후 킬점수 누적

		if ((_killer - 1) == myIndex) // 내가(_killer - 1) 상대방(_victim - 1)을 죽였을때.
			uiManager.SetMyKillDeath("kill");

		if ((_victim - 1) == myIndex) // 내가(_victim - 1) 상대방(_killer - 1)에게 죽었을때.
		{
			StateManager.instance.Death(true); //내가 죽은상태로 전환.
			uiManager.SetMyKillDeath("death");
			uiManager.SetDeadUI("당신이 " + playersManager.nickname[_killer - 1] + "의 " + weaponManager.GetWeaponName(_killer - 1) + "로 인해 사망했습니다!"); // 죽은 UI로 전환
		}
		else
		{
			gameManager.SetLocalAndNetworkActionState(_victim - 1, _ACTION_STATE.DEATH);
			uiManager.OffPlayerUI(_victim - 1);
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

		// 남아있는 Addforce 초기화 및 drag값 복구
		Rigidbody rigid = playersManager.obj_players[_packet.playerNum - 1].GetComponent<Rigidbody>();
		rigid.velocity = Vector3.zero;
		rigid.drag = C_Global.normalDrag;
	}

	public void CarSpawn(int _seed)
	{
		// 시드 꼽고 차 생성
		UnityEngine.Random.InitState(_seed);
		pathSpawner.SpawnPrefabs(); // 차 생성
	}

	public void TimeSync(double _time)
	{
		uiManager.gameTime = _time;
	}

	public void OtherPlayerHitByCar(int _playerNum, float _posX, float _posZ)
	{
		Debug.Log("다른놈");
		// 맞은 놈 튕기게 하고
		Rigidbody rigid = playersManager.obj_players[_playerNum - 1].GetComponent<Rigidbody>();
		Vector3 force = new Vector3(_posX, 0.0f, _posZ);
		rigid.drag = C_Global.carHitDrag;   // 치일땐 실감나게 낮은 drag로
		rigid.AddForce(force);

		// 상태 변경
		gameManager.SetLocalAndNetworkActionState(_playerNum - 1, _ACTION_STATE.DEATH);

		// 체력 UI 적용(차는 즉사)
		int absoluteIdx = uiManager.PlayerIndexToAbsoluteIndex(_playerNum - 1);
		uiManager.HealthUIChanger(absoluteIdx, 0.0f);

		UIManager.instance.EnqueueKillLog_CarCrash(_playerNum); //킬로그 큐에 넣음
	}

	// 정상적인 업데이트 시
	public void OnUpdate(ref IngamePacket _packet)
	{
		networkManager.SetIngamePacket(_packet.playerNum - 1, ref _packet);
	}

	public void CaptureResult(int _capturePlayerNum, int _triggerIdx)
	{
		int idx = _capturePlayerNum - 1;

		uiManager.SetScore(_capturePlayerNum, gameManager.gameInfo.capturePoint); //인덱스로 팀판별하여 점령점수 누적

		switch ((C_Global.GameType)GameManager.instance.gameInfo.gameType)
		{
			case C_Global.GameType._2vs2:
				{
					//진짜 점령및 탈취가 끝난후.
					if (idx == GameManager.instance.playersIndex[0] || idx == GameManager.instance.playersIndex[1])
					{
						//Debug.Log("점령지를 탈취했습니다!");
						UIManager.instance.txt_captureNotice.color = Color.green;
						UIManager.instance.txt_captureNotice.text = (_triggerIdx + 1).ToString() + "번 점령지를 탈취 했습니다 !";
						CaptureManager.instance.captureResult_team[_triggerIdx] = _CAPTURE_RESULT.GET;
						CaptureManager.instance.captureResult_enemy[_triggerIdx] = _CAPTURE_RESULT.LOSE;
						CaptureManager.instance.territoryOutline[_triggerIdx].OutlineColor = Color.green;
						CaptureManager.instance.territoryOutline[_triggerIdx].HaloColor = Color.green;
					}
					else
					{
						//Debug.Log("점령지를 빼앗겼습니다!");
						UIManager.instance.txt_captureNotice.color = Color.red;
						UIManager.instance.txt_captureNotice.text = (_triggerIdx + 1).ToString() + "번 점령지를 빼앗겼습니다 !";
						CaptureManager.instance.captureResult_team[_triggerIdx] = _CAPTURE_RESULT.LOSE;
						CaptureManager.instance.captureResult_enemy[_triggerIdx] = _CAPTURE_RESULT.GET;
						CaptureManager.instance.territoryOutline[_triggerIdx].OutlineColor = Color.red;
						CaptureManager.instance.territoryOutline[_triggerIdx].HaloColor = Color.red;
					}
				}
				break;

			case C_Global.GameType._1vs1:
				{
					//진짜 점령및 탈취가 끝난후.
					if (idx == GameManager.instance.playersIndex[0])
					{
						//Debug.Log("점령지를 탈취했습니다!");
						UIManager.instance.txt_captureNotice.color = Color.green;
						UIManager.instance.txt_captureNotice.text = (_triggerIdx + 1).ToString() + "번 점령지를 탈취 했습니다 !";
						CaptureManager.instance.captureResult_team[_triggerIdx] = _CAPTURE_RESULT.GET;
						CaptureManager.instance.captureResult_enemy[_triggerIdx] = _CAPTURE_RESULT.LOSE;
						CaptureManager.instance.territoryOutline[_triggerIdx].OutlineColor = Color.green;
						CaptureManager.instance.territoryOutline[_triggerIdx].HaloColor = Color.green;
					}
					else
					{
						//Debug.Log("점령지를 빼앗겼습니다!");
						UIManager.instance.txt_captureNotice.color = Color.red;
						UIManager.instance.txt_captureNotice.text = (_triggerIdx + 1).ToString() + "번 점령지를 빼앗겼습니다 !";
						CaptureManager.instance.captureResult_team[_triggerIdx] = _CAPTURE_RESULT.LOSE;
						CaptureManager.instance.captureResult_enemy[_triggerIdx] = _CAPTURE_RESULT.GET;
						CaptureManager.instance.territoryOutline[_triggerIdx].OutlineColor = Color.red;
						CaptureManager.instance.territoryOutline[_triggerIdx].HaloColor = Color.red;
					}
				}
				break;
		}

		UIManager.instance.txt_captureNotice.gameObject.SetActive(true); //셋팅한거 켜주고
		UIManager.instance.StartCoroutine(UIManager.instance.Cor_CheckCaptureNoticeTime()); // x초뒤에 캡처 알림 텍스트 셋엑티브펄스.
		UIManager.instance.isStartCaptureSubCor[_triggerIdx] = false; //서브코루틴 종료는 여기서 초기화되어야함. 메인캡처코루틴이 끝난후
		UIManager.instance.isStartCaptureCor[_triggerIdx] = false;
	}

	public void ItemEffectProcess(ref IngamePacket _packet, int _itemCode)
	{
		switch ((ItemCode)_itemCode)
		{
			case ItemCode.HP_NORMAL:
				{
					HealthChanger(ref _packet); // 피 변경
					Debug.Log("채운 피:" + _packet.health);
				}
				break;
		}
	}

	// 다른 플레이어의 접속이 끊겼을때, 플레이어 로빈 오브젝트를 비활성화.
	public void OnOtherPlayerDisconnected(int _quitPlayerNum)
	{
		if (playersManager != null) // 플레이어 매니저 있는 경우만
		{
			if (playersManager.obj_players[_quitPlayerNum - 1].activeSelf == true)
			{
				playersManager.obj_players[_quitPlayerNum - 1].SetActive(false);

				int absoluteIdx = uiManager.PlayerIndexToAbsoluteIndex(_quitPlayerNum - 1);
				uiManager.OffPlayerUI(absoluteIdx);
			}
		}
	}

	public void GameEndProcess()
	{
		//여기
		/// 게임이 종료되었다고 텍스트를 띄우던 뭘 하던 하고, 조작 못하도록 한다.
	}

	public void ScoreShowPrcess(ref int[] _playersNum, ref Score[] _scores)
	{
		/// 여기서 _scores 다음 씬까지 저장하고 알아서 EndScene에서 요리해라 광일아
		if (moveCor != null)
			StopCoroutine(moveCor);

		// 지워지지 않는 오브젝트로 정보 이동
		EndSceneManager.Instance.nickName = PlayersManager.instance.nickname;
		EndSceneManager.Instance.playerNum = _playersNum;
		EndSceneManager.Instance.scores = new Score[4];
		for (int i = 0; i < _scores.Length; i++)
		{
			EndSceneManager.Instance.scores[_playersNum[i] - 1] = _scores[i];
		}
		EndSceneManager.Instance.gameType = GameManager.instance.gameInfo.gameType;

		/// 여기서 스무스로딩할때 페이크로딩 안넣고 싶음
		SceneLoader.LoadScene("EndScene");  // 스무스 로딩

		SceneLoader.Instance.waitOtherPlayer = false;

		networkManager.SendGotoLobby();   // 나 로비로 갈랭~(얜 호출해줘야 돼)
	}

	//쐇을때 무조건 1번의 패킷을 보내야됨. 보정용
	public void SendPacketOnce()
	{
		networkManager.SendIngamePacket();
	}
}