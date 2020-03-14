using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
	public int myIndex { get; set; }
	private int returnIdx;

    #region _PARTICLE_CLONE
    private ParticleSystem ps_effectClone;
    #endregion

    #region _BULLET_CLONE 
    public struct _BULLET_CLONE_INFO
    {
        public Transform tf_bulletClone { get; set; }
        public Rigidbody rb_bulletClone { get; set; }
    }
    public _BULLET_CLONE_INFO bulletCloneInfo;
    #endregion

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

        bulletCloneInfo.tf_bulletClone = this.gameObject.GetComponent<Transform>();
        bulletCloneInfo.rb_bulletClone = this.gameObject.GetComponent<Rigidbody>();
    }

	private void Update()
	{
		WeaponManager.instance.CheckFireRange(this.gameObject, bulletCloneInfo, returnIdx);
	}

	private void OnTriggerEnter(Collider other)
	{

#if !NETWORK
		//적에게 내가맞았냐, 팀킬을했냐, 적1이냐 적2냐에따른 총알피해, 체력적용
		//함수 _type은 피아구분, returnIdx는 총알의 주인인덱스
        for(int i=0; i<C_Global.MAX_CHARACTER; i++)
        {
            if (other.CompareTag(PlayersManager.instance.obj_players[GameManager.instance.playersIndex[i]].tag))
            {
                //bl_MiniMap.Instance.DoHitEffect();
                int tmp = UnityEngine.Random.Range(3, 5);
                AudioManager.Instance.Play(tmp);
                WeaponManager.instance.ApplyDamage(GameManager.instance.playersIndex[i], returnIdx);

                ps_effectClone = PoolManager.instance.GetCollisionEffectFromPool("BloodPool1", other.ClosestPointOnBounds(this.transform.position), this.transform.forward);
                EffectManager.instance.StartCoroutine(EffectManager.instance.CheckEffectEnd(ps_effectClone));
                break;
            }
        }

#endif
		// 플레이어 안맞은 경우
        if(!other.CompareTag("Player1") && !other.CompareTag("Player2") && !other.CompareTag("Player3") && !other.CompareTag("Player4"))
        {
            if (other.CompareTag("Concrete1"))
            {
                ps_effectClone = PoolManager.instance.GetCollisionEffectFromPool("ConcretePool1", other.ClosestPointOnBounds(this.transform.position), -this.transform.forward);
                EffectManager.instance.StartCoroutine(EffectManager.instance.CheckEffectEnd(ps_effectClone));
                AudioManager.Instance.Play(6);
            }
            else
            {
                ps_effectClone = PoolManager.instance.GetCollisionEffectFromPool("IronPool1", other.ClosestPointOnBounds(this.transform.position), -this.transform.forward);
                EffectManager.instance.StartCoroutine(EffectManager.instance.CheckEffectEnd(ps_effectClone));
                AudioManager.Instance.Play(7);
            }
        }
        else
        {
            ps_effectClone = PoolManager.instance.GetCollisionEffectFromPool("BloodPool1", other.ClosestPointOnBounds(this.transform.position), this.transform.forward);
            EffectManager.instance.StartCoroutine(EffectManager.instance.CheckEffectEnd(ps_effectClone));

            if(returnIdx == myIndex)
                AudioManager.Instance.Play(UnityEngine.Random.Range(3, 5));
        }

        PoolManager.instance.ReturnGunToPool(gameObject, bulletCloneInfo, returnIdx);

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