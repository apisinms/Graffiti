﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    public  Transform[] curPlayerPos { get; set; }
    NetworkManager networkManager;
    Vector3 pos;

    void Awake()
    {
        Application.targetFrameRate = 60;
        networkManager = NetworkManager.instance;

		curPlayerPos = new Transform[C_Global.MAX_PLAYER];

		for (int i = 0; i < curPlayerPos.Length; i++)
			curPlayerPos[i] = PlayersManager.instance.obj_players[i].transform;

		pos = new Vector3();

		// 초기값 설정 
		for (int i = 0; i < C_Global.MAX_PLAYER; i++)
		{
			networkManager.SetPosX(i, curPlayerPos[i].localPosition.x);
			networkManager.SetPosZ(i, curPlayerPos[i].localPosition.z);
			networkManager.SetPosPlayerNum(i, i + 1);
		}

		StartCoroutine(this.CheckQuit());
    }


    // 플레이어를 뒤져봐서 위치가 다르면 업데이트
    void Update()
    {
        for (int i = 0; i < curPlayerPos.Length; i++)
        {
            // 자기 제외하고
            if (GameManager.instance.myIndex == i)
                continue;

            switch ((_ACTION_STATE)NetworkManager.instance.GetActionState(i))
            {
                case _ACTION_STATE.IDLE:
                    PlayersManager.instance.Action_Idle(i);
                    break;
                case _ACTION_STATE.CIRCUIT:
                    {
                        pos.x = networkManager.GetPosX(i);
                        pos.y = curPlayerPos[i].localPosition.y;
                        pos.z = networkManager.GetPosZ(i);

                        PlayersManager.instance.Action_CircuitNormal(i, pos, networkManager.GetRotY(i));
                    }
                    break;
                case _ACTION_STATE.AIMING:
                    {
                        PlayersManager.instance.Action_AimingNormal(i, networkManager.GetRotY(i));
                    }
                    break;
                case _ACTION_STATE.CIRCUIT_AND_AIMING:
                    {
                        pos.x = networkManager.GetPosX(i);
                        pos.y = curPlayerPos[i].localPosition.y;
                        pos.z = networkManager.GetPosZ(i);

                        PlayersManager.instance.Action_AimingWithCircuit(i, pos, networkManager.GetRotY(i));
                    }
                    break;
            }

            ////위치가 바뀐애가 있으면
            //if (networkManager.GetPosX(i) != curPlayerPos[i].localPosition.x ||
            //   networkManager.GetPosZ(i) != curPlayerPos[i].localPosition.z)
            //{
            //	pos.x = networkManager.GetPosX(i);
            //	pos.y = curPlayerPos[i].localPosition.y;
            //	pos.z = networkManager.GetPosZ(i);

            //	//curPlayerPos[i].transform.localEulerAngles = new Vector3(0, networkManager.GetRotY(i), 0);

            //	// curPlayerPos[i].position = Vector3.Lerp(curPlayerPos[i].position, pos,
            //	// Time.smoothDeltaTime * (PlayersManager.instance.speed[i] * 3));

            //	PlayersManager.instance.Action_CircuitNormal(i, pos, networkManager.GetRotY(i));

            //             if (Mathf.Abs(networkManager.GetPosX(i) - curPlayerPos[i].localPosition.x) > 0.015f ||
            //                 Mathf.Abs(networkManager.GetPosZ(i) - curPlayerPos[i].localPosition.z) > 0.015f)
            //                 PlayersManager.instance.Action_CircuitNormal(i, pos, networkManager.GetRotY(i));
            //             else
            //                 curPlayerPos[i].transform.localPosition = pos;
            //         }
            //else // 위치가 바뀌지 않았다면 
            //{
            //	PlayersManager.instance.Action_Idle(i);
            //	//PlayersManager.instance.Anime_Idle(i);
            //}
        }
    }

    IEnumerator CheckQuit()
    {
        while (true)
        {
            switch (networkManager.CheckQuit())
            {
                case 1:
					Destroy(GameObject.FindGameObjectWithTag("Player1"));
                    break;

                case 2:
					Destroy(GameObject.FindGameObjectWithTag("Player2"));
					break;

                case 3:
					Destroy(GameObject.FindGameObjectWithTag("Player3"));
					break;

                case 4:
					Destroy(GameObject.FindGameObjectWithTag("Player4"));
					break;

                default:
                    break;
            }

            yield return YieldInstructionCache.WaitForSeconds(0.25f);
        }
    }
}