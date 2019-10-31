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
    public bool[] isStartReloadGageCor { get; set; }
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
      networkManager.SendIngamePacket(true);
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

        curPlayerPos = new Transform[gameManager.gameInfo.maxPlayer];

        for (int i = 0; i < curPlayerPos.Length; i++)
            curPlayerPos[i] = playersManager.obj_players[i].transform;

        pos = new Vector3();
        isStartShotCor = new bool[gameManager.gameInfo.maxPlayer]; //샷 코루틴 중복실행 방지.
        isStartReloadGageCor = new bool[gameManager.gameInfo.maxPlayer];

#if NETWORK
        // 초기값 설정 
        for (int i = 0; i < gameManager.gameInfo.maxPlayer; i++)
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

            if (NetworkManager.instance.GetReloadState(i) == true)
                View_Reloading(i);

            actionState = (_ACTION_STATE)networkManager.GetActionState(i); // 변수하나파서 스테이트여기에 넣음

            switch (actionState)
            {
                case _ACTION_STATE.DEATH:
                    {
                        playersManager.Action_Death(i);
                        View_StopFire(i);
                    }
                    break;
                case _ACTION_STATE.IDLE:
                    {
                        playersManager.Action_Idle(i, pos);
                        View_StopFire(i);
                    }
                    break;
                case _ACTION_STATE.CIR:
                    {
                        playersManager.Action_CircuitNormal(i, pos, networkManager.GetRotY(i));
                        View_StopFire(i);
                    }
                    break;
                case _ACTION_STATE.AIM:
                    {
                        playersManager.Action_AimingNormal(i, pos, networkManager.GetRotY(i));
                        View_StopFire(i);
                    }
                    break;
                case _ACTION_STATE.SHOT:
                    {
                        playersManager.Action_AimingNormal(i, pos, networkManager.GetRotY(i)); //조준애니메이션은 계속하고.
                        View_StartFire(i);
                    }
                    break;
                case _ACTION_STATE.CIR_AIM:
                    {
                        playersManager.Action_AimingWithCircuit(i, pos, networkManager.GetRotY(i));
                        View_StopFire(i);
                    }
                    break;
                case _ACTION_STATE.CIR_AIM_SHOT:
                    {
                        playersManager.Action_AimingWithCircuit(i, pos, networkManager.GetRotY(i)); //조준이동 애니메이션은 계속하고.
                        View_StartFire(i);
                        break;
                    }
                case _ACTION_STATE.SPRAY:
                    {
                        playersManager.Action_Spray(i, networkManager.GetRotY(i));
                        break;
                    }
            }
        }
#endif
    }

    private void View_StartFire(int _idx)
    {
        if (isStartShotCor[_idx] == false)
        {
            if (weaponManager.curMainActionCor[_idx] != null)
                StopCoroutine(weaponManager.curMainActionCor[_idx]);
            weaponManager.curMainActionCor[_idx] = StartCoroutine(weaponManager.ActionFire(_idx));

            isStartShotCor[_idx] = true;
        }
    }

    private void View_StopFire(int _idx)
    {
        if (isStartShotCor[_idx] == true)
        {
            if (weaponManager.curMainActionCor[_idx] != null)
                StopCoroutine(weaponManager.curMainActionCor[_idx]);
            EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, _idx);

            isStartShotCor[_idx] = false;
        }
    }

    private void View_Reloading(int _idx)
    {
        if (isStartReloadGageCor[_idx] == false)
        {
            uiManager.StartCoroutine(uiManager.Cor_DecreaseReloadGageImg(weaponManager.GetReloadTime(_idx), _idx));
            isStartReloadGageCor[_idx] = true;
        }
    }
}