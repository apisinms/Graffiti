using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    public  Transform[] curPlayerPos { get; set; }
    NetworkManager networkManager;
    Vector3 pos;

    void Awake()
    {
#if NETWORK
        networkManager = NetworkManager.instance;
#endif

		curPlayerPos = new Transform[C_Global.MAX_PLAYER];

		for (int i = 0; i < curPlayerPos.Length; i++)
			curPlayerPos[i] = PlayersManager.instance.obj_players[i].transform;

		pos = new Vector3();

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

			switch ((_ACTION_STATE)NetworkManager.instance.GetActionState(i))
			{
				case _ACTION_STATE.IDLE:
					{
						PlayersManager.instance.Action_Idle(i, pos);
					}
					break;
				case _ACTION_STATE.CIRCUIT:
					{
						PlayersManager.instance.Action_CircuitNormal(i, pos, networkManager.GetRotY(i));
					}
					break;
				case _ACTION_STATE.AIMING:
					{
						PlayersManager.instance.Action_AimingNormal(i, pos, networkManager.GetRotY(i));
					}
					break;
				case _ACTION_STATE.CIRCUIT_AND_AIMING:
					{
						PlayersManager.instance.Action_AimingWithCircuit(i, pos, networkManager.GetRotY(i));
					}
					break;
			}
		}
#endif
	}
}
