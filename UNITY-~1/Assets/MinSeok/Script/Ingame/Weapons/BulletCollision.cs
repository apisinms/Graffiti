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
        if(this.gameObject.CompareTag("Bullet1"))
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
        //총알끼리는 충돌체크x, 내총알에 내가맞는것도x
        //  for (int i = 0; i < PoolManager.instance.bulletTag.Length; i++)
        //    {
        //     if (other.gameObject.CompareTag(PoolManager.instance.bulletTag[i]))
        //         return;
        //  }
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
    }

}
