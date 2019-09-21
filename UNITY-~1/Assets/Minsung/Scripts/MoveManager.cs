using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    public Transform[] curPlayerPos { get; set; }
    private NetworkManager networkManager;
    private Vector3 pos;
    public bool[] isStartShotCor { get; set; } //샷 코루틴 중복실행 방지.
    private _ACTION_STATE actionState;

    void Awake()
    {
#if NETWORK
        networkManager = NetworkManager.instance;
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


    // 플레이어를 뒤져봐서 위치가 다르면 업데이트
    void Update()
    {
#if NETWORK
        for (int i = 0; i < curPlayerPos.Length; i++)
        {
            // 자기 제외하고
            if (GameManager.instance.myIndex == i)
                continue;

            pos.x = networkManager.GetPosX(i);
            //pos.y = curPlayerPos[i].localPosition.y;
            pos.y = 0;
            pos.z = networkManager.GetPosZ(i);

            actionState = (_ACTION_STATE)NetworkManager.instance.GetActionState(i); //변수하나파서 스테이트여기에 넣음

            if (actionState != _ACTION_STATE.SHOT && actionState != _ACTION_STATE.CIR_AIM_SHOT) //두가지아닐때는 샷코루틴실행이 아닌상태.
            {
                if(isStartShotCor[i] == true)
                {
                    if (WeaponManager.instance.curActionCor[i] != null)
                        StopCoroutine(WeaponManager.instance.curActionCor[i]);

                    isStartShotCor[i] = false;
                }
            }

            switch (actionState)
            {
                case _ACTION_STATE.IDLE:
                    {
                        PlayersManager.instance.Action_Idle(i, pos);
                    }
                    break;
                case _ACTION_STATE.CIR:
                    {
                        PlayersManager.instance.Action_CircuitNormal(i, pos, networkManager.GetRotY(i));
                    }
                    break;
                case _ACTION_STATE.AIM:
                    {
                        PlayersManager.instance.Action_AimingNormal(i, pos, networkManager.GetRotY(i));
                    }
                    break;
                case _ACTION_STATE.SHOT:
                    {
                        PlayersManager.instance.Action_AimingNormal(i, pos, networkManager.GetRotY(i)); //조준애니메이션은 계속하고.

                        if (isStartShotCor[i] == false) //샷만 중복실행 방지하면서 코루틴 1회실행.
                        {
                            if (WeaponManager.instance.curActionCor[i] != null)
                                StopCoroutine(WeaponManager.instance.curActionCor[i]); //이전꺼 멈추고
                            StartCoroutine(WeaponManager.instance.ActionBullet(i));
                            isStartShotCor[i] = true;
                        }
                    }
                    break;
                case _ACTION_STATE.CIR_AIM:
                    {
                        PlayersManager.instance.Action_AimingWithCircuit(i, pos, networkManager.GetRotY(i));
                    }
                    break;
                case _ACTION_STATE.CIR_AIM_SHOT:
                    {
                        PlayersManager.instance.Action_AimingWithCircuit(i, pos, networkManager.GetRotY(i)); //조준이동 애니메이션은 계속하고.

                        if (isStartShotCor[i] == false) //샷만 중복실행 방지하면서 코루틴 1회실행.
                        {
                            if (WeaponManager.instance.curActionCor[i] != null)
                                StopCoroutine(WeaponManager.instance.curActionCor[i]);
                            StartCoroutine(WeaponManager.instance.ActionBullet(i));
                            isStartShotCor[i] = true;
                        }
                    }
                    break;
            }
        }
#endif
    }
}