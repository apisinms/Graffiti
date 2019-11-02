using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeCollision : MonoBehaviour
{
    public int myIndex { get; set; }
    private int returnIdx;

    /*
    #region _PARTICLE_CLONE
    private ParticleSystem ps_effectClone;
    #endregion
    */

    #region _GRENADE_CLONE 
    public struct _GRENADE_CLONE_INFO
    {
        public Transform tf_grenadeClone { get; set; }
        public Rigidbody rb_grenadeClone { get; set; }
        public TrailRenderer tr_grenadeClone { get; set; }
    }
    public _GRENADE_CLONE_INFO grenadeCloneInfo;
    #endregion

    private void Awake()
    {
        myIndex = GameManager.instance.myIndex;
    }

    private void Start()
    {
        if (this.gameObject.CompareTag("Grenade1"))
            returnIdx = 0;
        else if (this.gameObject.CompareTag("Grenade2"))
            returnIdx = 1;
        else if (this.gameObject.CompareTag("Grenade3"))
            returnIdx = 2;
        else if (this.gameObject.CompareTag("Grenade4"))
            returnIdx = 3;

        grenadeCloneInfo.tf_grenadeClone = this.gameObject.GetComponent<Transform>();
        grenadeCloneInfo.rb_grenadeClone = this.gameObject.GetComponent<Rigidbody>();
        grenadeCloneInfo.tr_grenadeClone = this.gameObject.GetComponent<TrailRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PoolManager.instance.ReturnGrenadeToPool(gameObject, grenadeCloneInfo, returnIdx);
    }
}
