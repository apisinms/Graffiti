﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
	public int myIndex { get; set; }
	private int returnIdx;
	private ParticleSystem ps_clone;

	private void Awake()
	{
		myIndex = GameManager.instance.myIndex;
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

		//prevActionState = (_ACTION_STATE)NetworkManager.instance.GetActionState(returnIdx);
	}

	private void Update()
	{
		WeaponManager.instance.CheckFireRange(this.gameObject, returnIdx);
	}

	private void OnTriggerEnter(Collider other)
	{

#if !NETWORK
		//적에게 내가맞았냐, 팀킬을했냐, 적1이냐 적2냐에따른 총알피해, 체력적용
		//함수 _type은 피아구분, returnIdx는 총알의 주인인덱스
        for(int i=0; i<C_Global.MAX_PLAYER; i++)
        {
            if (other.CompareTag(PlayersManager.instance.obj_players[GameManager.instance.playersIndex[i]].tag))
                WeaponManager.instance.ApplyDamage(GameManager.instance.playersIndex[i], returnIdx);
        }
        /*
		if (other.CompareTag(PlayersManager.instance.obj_players[GameManager.instance.myIndex].tag))
            WeaponManager.instance.ApplyDamage(0, returnIdx);
        else if (other.CompareTag(PlayersManager.instance.obj_players[GameManager.instance.myTeamIndex].tag))
            WeaponManager.instance.ApplyDamage(1, returnIdx);
        else if (other.CompareTag(PlayersManager.instance.obj_players[GameManager.instance.enemyIndex[0]].tag))
            WeaponManager.instance.ApplyDamage(2, returnIdx);
        else if (other.CompareTag(PlayersManager.instance.obj_players[GameManager.instance.enemyIndex[1]].tag))
            WeaponManager.instance.ApplyDamage(3, returnIdx);
            */
#endif

        if (other.CompareTag("Concrete1"))
		{
			ps_clone = PoolManager.instance.GetCollisionEffectFromPool("ConcretePool1", other.ClosestPointOnBounds(this.transform.position), -this.transform.forward);
			EffectManager.instance.StartCoroutine(EffectManager.instance.CheckEffectEnd(ps_clone));
		}
		else
		{
			ps_clone = PoolManager.instance.GetCollisionEffectFromPool("IronPool1", other.ClosestPointOnBounds(this.transform.position), -this.transform.forward);
			EffectManager.instance.StartCoroutine(EffectManager.instance.CheckEffectEnd(ps_clone));
		}

		PoolManager.instance.ReturnBulletToPool(gameObject, returnIdx);

		if (myIndex != returnIdx)
			return;

		// 내가 쏜 총알이면 총알 충돌 구조체 셋팅
		WeaponManager.instance.SetCollisionChecker(other.tag);
	}
}











    /*
     *         //총알끼리는 충돌체크x, 내총알에 내가맞는것도x
        //  for (int i = 0; i < PoolManager.instance.bulletTag.Length; i++)
        //    {
        //     if (other.gameObject.CompareTag(PoolManager.instance.bulletTag[i]))
        //         return;
        //  }
        */