using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public int myIndex { get; set; }
    private int returnIdx;

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

    /*
    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log(other.gameObject.name);
        //총알끼리는 충돌체크x, 내총알에 내가맞는것도x
        for (int i = 0; i < WeaponManager.instance.bulletTag.Length; i++)
        {
            if (other.gameObject.CompareTag(WeaponManager.instance.bulletTag[i]))
            {
                Debug.Log("총알충돌");
                return;
            }
        }

        ContactPoint contactPoint = other.contacts[0];

        Instantiate(EffectManager.instance.ps_sparkList, contactPoint.point, Quaternion.LookRotation(contactPoint.normal));
        WeaponManager.instance.ReturnBulletToPool(gameObject, returnIdx);
    }
    */

    
    private void OnTriggerEnter(Collider other)
    {       
        //총알끼리는 충돌체크x, 내총알에 내가맞는것도x
        for (int i = 0; i < WeaponManager.instance.bulletTag.Length; i++)
        {
            if (other.gameObject.CompareTag(WeaponManager.instance.bulletTag[i]))
                return;
        }

        Instantiate(EffectManager.instance.ps_sparkList[0], other.ClosestPointOnBounds(this.transform.position), Quaternion.LookRotation(-this.transform.forward));
        WeaponManager.instance.ReturnBulletToPool(gameObject, returnIdx);
    }
    
}
