using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
	public int myIndex { get; set; }
	//public _ACTION_STATE[] prevActionState { get; set; }
	public _ACTION_STATE prevActionState { get; set; }
	private int returnIdx;

	private void Awake()
	{
		myIndex = GameManager.instance.myIndex;
		//prevActionState = new _ACTION_STATE[C_Global.MAX_PLAYER];
	}
	private void Start()
	{
		if (this.gameObject.CompareTag("Bullet1"))
			returnIdx = 0;
		else if (this.gameObject.CompareTag("Bullet2"))
			returnIdx = 1;
		else if (this.gameObject.CompareTag("Bullet3"))
			returnIdx = 2;
		else if (this.gameObject.CompareTag("Bullet4"))
			returnIdx = 3;

#if NETWORK
		prevActionState = (_ACTION_STATE)NetworkManager.instance.GetActionState(returnIdx);
#endif
		/*
        if (returnIdx == myIndex)
            prevActionState = PlayersManager.instance.actionState[myIndex];
        else
            prevActionState = (_ACTION_STATE)NetworkManager.instance.GetActionState(returnIdx);

        */
		/*
        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            if (i == myIndex)
                prevActionState[myIndex] = PlayersManager.instance.actionState[myIndex];
            else
                prevActionState[i] = (_ACTION_STATE)NetworkManager.instance.GetActionState(i);
        } 
        */
		//prevActionState[myIndex] = PlayersManager.instance.actionState[myIndex];
	}
	private void Update()
	{
		CheckBulletRange(returnIdx);

		/*
        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            //총알이 쐇을때의 플레이어 상태가 샷일때만 사정거리 검사진행
            if (prevActionState[i] != _ACTION_STATE.SHOT && prevActionState[i] != _ACTION_STATE.CIR_AIM_SHOT)
                continue;

            CheckBulletRange(i);
        }
        */

	}


	public void CheckBulletRange(int _index)
	{
		switch (WeaponManager.instance.mainWeapon[_index])
		{
			case _WEAPONS.AR:
				{
					if (Vector3.Distance(this.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= WeaponManager.instance.infoAR[_index].range)
						WeaponManager.instance.ReturnBulletToPool(gameObject, _index);
				}
				break;

			case _WEAPONS.SG:
				{
					if (Vector3.Distance(this.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= WeaponManager.instance.infoSG[_index].range)
						WeaponManager.instance.ReturnBulletToPool(gameObject, _index);
				}
				break;

			case _WEAPONS.SMG:
				{
					if (Vector3.Distance(this.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= WeaponManager.instance.infoSMG[_index].range)
						WeaponManager.instance.ReturnBulletToPool(gameObject, _index);
				}
				break;
		}
	}


	private void OnTriggerEnter(Collider other)
	{
		//총알끼리는 충돌체크x, 내총알에 내가맞는것도x
		for (int i = 0; i < WeaponManager.instance.bulletTag.Length; i++)
		{
			if (other.gameObject.CompareTag(WeaponManager.instance.bulletTag[i]))
				return;
		}

		WeaponManager.instance.ReturnBulletToPool(gameObject, returnIdx);
	}
}