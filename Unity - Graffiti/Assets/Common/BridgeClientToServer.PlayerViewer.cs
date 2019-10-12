using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//            BridgeClientToServer.PlayerViewer 기준 MoveManager역할을 하던게 여기로옴
public partial class BridgeClientToServer : MonoBehaviour
{
    #region PLAYER_VIEWER
    public Transform[] curPlayerPos { get; set; }
    private Vector3 pos;
    public bool[] isStartShotCor { get; set; } //샷 코루틴 중복실행 방지.
    private _ACTION_STATE actionState;
    #endregion

    public void Initialization_PlayerViewer()
    {
		myIndex = GameManager.instance.myIndex;
		playersManager = PlayersManager.instance;
#if NETWORK
		//////////////// 게임 시작 시 최초로 1회 내 위치정보를 서버로 전송해야함 /////////////////
		networkManager.SendPosition(playersManager.tf_players[myIndex].localPosition.x,
             playersManager.tf_players[myIndex].localPosition.z,
             playersManager.tf_players[myIndex].localEulerAngles.y, playersManager.speed[myIndex], playersManager.actionState[myIndex], true);
#endif

#if NETWORK
        //////////////////////// 테스트용(상대팀 끄기) ////////////////////
        
        switch (myIndex)
        {
            case 0:
            case 1:
				//playerInSector[0] = true;
				//playerInSector[1] = true;
                playersManager.obj_players[2].SetActive(false);
                playersManager.obj_players[3].SetActive(false);
                break;

            case 2:
            case 3:
				//playerInSector[2] = true;
				//playerInSector[3] = true;
				playersManager.obj_players[0].SetActive(false);
                playersManager.obj_players[1].SetActive(false);
                break;
        }
#endif

		curPlayerPos = new Transform[C_Global.MAX_PLAYER];

        for (int i = 0; i < curPlayerPos.Length; i++)
            curPlayerPos[i] = PlayersManager.instance.obj_players[i].transform;

        pos = new Vector3();
        isStartShotCor = new bool[C_Global.MAX_PLAYER]; //샷 코루틴 중복실행 방지.

#if NETWORK
      // 초기값 설정 
      for (int i = 0; i < C_Global.MAX_PLAYER; i++)
      {
         networkManager.SetPosX(i, curPlayerPos[i].localPosition.x);
         networkManager.SetPosZ(i, curPlayerPos[i].localPosition.z);
         networkManager.SetPosPlayerNum(i, i + 1);
      }
#endif
    }

    public void PlayerActionViewer()
    {
#if NETWORK
        for (int i = 0; i < curPlayerPos.Length; i++)
		{
			// 자기 또는 섹터에 없는 플레이어 제외하고
			if (myIndex == i || playersManager.obj_players[i].activeSelf == false)
                continue;

            pos.x = networkManager.GetPosX(i);
            //pos.y = curPlayerPos[i].localPosition.y;
            pos.y = 0;
            pos.z = networkManager.GetPosZ(i);

            actionState = (_ACTION_STATE)networkManager.GetActionState(i); // 변수하나파서 스테이트여기에 넣음

            switch (actionState)
            {
                case _ACTION_STATE.IDLE:
                    {
                        playersManager.Action_Idle(i, pos);

                        if (isStartShotCor[i] == true)
                        {
                            if (WeaponManager.instance.curActionCor[i] != null)
                                StopCoroutine(WeaponManager.instance.curActionCor[i]);

                            //StopCoroutine(WeaponManager.instance.ActionBullet(i));
                            isStartShotCor[i] = false;
                        }

                    }
                    break;
                case _ACTION_STATE.CIR:
                    {
						playersManager.Action_CircuitNormal(i, pos, networkManager.GetRotY(i));
                        if (isStartShotCor[i] == true)
                        {
                            if (WeaponManager.instance.curActionCor[i] != null)
                                StopCoroutine(WeaponManager.instance.curActionCor[i]);

                            //StopCoroutine(WeaponManager.instance.ActionBullet(i));
                            isStartShotCor[i] = false;
                        }

                    }
                    break;
                case _ACTION_STATE.AIM:
                    {
						playersManager.Action_AimingNormal(i, pos, networkManager.GetRotY(i));
                        if (isStartShotCor[i] == true)
                        {
                            if (WeaponManager.instance.curActionCor[i] != null)
                                StopCoroutine(WeaponManager.instance.curActionCor[i]);

                            //StopCoroutine(WeaponManager.instance.ActionBullet(i));
                            isStartShotCor[i] = false;
                        }

                    }
                    break;
                case _ACTION_STATE.SHOT:
                    { 
						playersManager.Action_AimingNormal(i, pos, networkManager.GetRotY(i)); //조준애니메이션은 계속하고.

                        if (isStartShotCor[i] == false) //샷만 중복실행 방지하면서 코루틴 1회실행.
                        {
                            if (WeaponManager.instance.curActionCor[i] != null)
                                StopCoroutine(WeaponManager.instance.curActionCor[i]); //이전꺼 멈추고

                            WeaponManager.instance.curActionCor[i] = StartCoroutine(WeaponManager.instance.ActionBullet(i));
                            isStartShotCor[i] = true;
                        }
                    }
                    break;
                case _ACTION_STATE.CIR_AIM:
                    {
						playersManager.Action_AimingWithCircuit(i, pos, networkManager.GetRotY(i));

                        if (isStartShotCor[i] == true)
                        {
                            if (WeaponManager.instance.curActionCor[i] != null)
                                StopCoroutine(WeaponManager.instance.curActionCor[i]);

                            //StopCoroutine(WeaponManager.instance.ActionBullet(i));
                            isStartShotCor[i] = false;
                        }

                    }
                    break;
                case _ACTION_STATE.CIR_AIM_SHOT:
                    {
						playersManager.Action_AimingWithCircuit(i, pos, networkManager.GetRotY(i)); //조준이동 애니메이션은 계속하고.

                        if (isStartShotCor[i] == false) //샷만 중복실행 방지하면서 코루틴 1회실행.
                        {
                            if (WeaponManager.instance.curActionCor[i] != null)
                                StopCoroutine(WeaponManager.instance.curActionCor[i]);

                            WeaponManager.instance.curActionCor[i] = StartCoroutine(WeaponManager.instance.ActionBullet(i));
                            isStartShotCor[i] = true;
                        }
                    }
                    break;
            }
        }
#endif
    }
}
