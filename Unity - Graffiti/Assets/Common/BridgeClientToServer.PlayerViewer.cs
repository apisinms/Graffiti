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
        playersManager = PlayersManager.instance;
        gameManager = GameManager.instance;
        weaponManager = WeaponManager.instance;
        uiManager = UIManager.instance;
        pathSpawner = GameObject.Find("Spawner").GetComponent<PathCreation.Examples.PathSpawner>();
        myIndex = gameManager.myIndex;

#if NETWORK
      //////////////// 게임 시작 시 최초로 1회 내 위치정보를 서버로 전송해야함 /////////////////
      networkManager.SendIngamePacket(weaponManager.GetCollisionChecker(), true);
#endif

#if !NETWORK
        //////////////////////// 테스트용(상대팀 끄기) ////////////////////

        switch (myIndex)
        {
            case 0:
            case 1:
                playersManager.obj_players[2].SetActive(false);
                playersManager.obj_players[3].SetActive(false);
                break;

            case 2:
            case 3:
                playersManager.obj_players[0].SetActive(false);
                playersManager.obj_players[1].SetActive(false);
                break;
        }
#endif

        curPlayerPos = new Transform[C_Global.MAX_PLAYER];

        for (int i = 0; i < curPlayerPos.Length; i++)
            curPlayerPos[i] = playersManager.obj_players[i].transform;

        pos = new Vector3();
        isStartShotCor = new bool[C_Global.MAX_PLAYER]; //샷 코루틴 중복실행 방지.

#if NETWORK
      // 초기값 설정 
      for (int i = 0; i < C_Global.MAX_PLAYER; i++)
      {
         networkManager.SetPosX(i, curPlayerPos[i].localPosition.x);
         networkManager.SetPosZ(i, curPlayerPos[i].localPosition.z);
         networkManager.SetPlayerNum(i, i + 1);
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
                case _ACTION_STATE.DEATH:
                    {
                        playersManager.Action_Death(i);

                        if (isStartShotCor[i] == true)
                        {
                            if (WeaponManager.instance.curMainActionCor[i] != null)
                                StopCoroutine(WeaponManager.instance.curMainActionCor[i]);
                            EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, i);

                            isStartShotCor[i] = false;
                        }
                    }
                    break;
                case _ACTION_STATE.IDLE:
               {
                  playersManager.Action_Idle(i, pos);

                  if (isStartShotCor[i] == true)
                  {
                     if (weaponManager.curMainActionCor[i] != null)
                        StopCoroutine(weaponManager.curMainActionCor[i]);
                     EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, i);

                     isStartShotCor[i] = false;
                  }

               }
               break;
            case _ACTION_STATE.CIR:
               {
                  playersManager.Action_CircuitNormal(i, pos, networkManager.GetRotY(i));

                  if (isStartShotCor[i] == true)
                  {
                     if (weaponManager.curMainActionCor[i] != null)
                        StopCoroutine(weaponManager.curMainActionCor[i]);
                     EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, i);

                     isStartShotCor[i] = false;
                  }
               }
               break;
            case _ACTION_STATE.AIM:
               {
                  playersManager.Action_AimingNormal(i, pos, networkManager.GetRotY(i));
                  if (isStartShotCor[i] == true)
                  {
                     if (weaponManager.curMainActionCor[i] != null)
                        StopCoroutine(weaponManager.curMainActionCor[i]);
                     EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, i);

                     isStartShotCor[i] = false;
                  }
               }
               break;
            case _ACTION_STATE.SHOT:
               {
                  playersManager.Action_AimingNormal(i, pos, networkManager.GetRotY(i)); //조준애니메이션은 계속하고.

                  if (isStartShotCor[i] == false)
                  {
                     if (weaponManager.curMainActionCor[i] != null)
                        StopCoroutine(weaponManager.curMainActionCor[i]); //이전꺼 멈추고

                     weaponManager.curMainActionCor[i] = StartCoroutine(weaponManager.ActionFire(i));
                     EffectManager.instance.PlayEffect(_EFFECT_TYPE.MUZZLE, i);

                     isStartShotCor[i] = true;
                  }
               }
               break;
            case _ACTION_STATE.CIR_AIM:
               {
                  playersManager.Action_AimingWithCircuit(i, pos, networkManager.GetRotY(i));

                  if (isStartShotCor[i] == true)
                  {
                     if (WeaponManager.instance.curMainActionCor[i] != null)
                        StopCoroutine(WeaponManager.instance.curMainActionCor[i]);
                     EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, i);

                     isStartShotCor[i] = false;
                  }

               }
               break;
            case _ACTION_STATE.CIR_AIM_SHOT:
               {
                  playersManager.Action_AimingWithCircuit(i, pos, networkManager.GetRotY(i)); //조준이동 애니메이션은 계속하고.

                  if (isStartShotCor[i] == false)
                  {
                     if (weaponManager.curMainActionCor[i] != null)
                        StopCoroutine(weaponManager.curMainActionCor[i]);

                     weaponManager.curMainActionCor[i] = StartCoroutine(weaponManager.ActionFire(i));
                     EffectManager.instance.PlayEffect(_EFFECT_TYPE.MUZZLE, i);

                     isStartShotCor[i] = true;
                  }
                  break;
               }
         }
      }
#endif
    }
}