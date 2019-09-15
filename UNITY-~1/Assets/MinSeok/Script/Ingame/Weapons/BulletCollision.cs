using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public static BulletCollision instance;
    public int myIndex { get; set; }
    private void Awake()
    {
        if (instance == null)
            instance = this;

        myIndex = GameManager.instance.myIndex;
    }
    private void Update()
    {
     /*   
            if (Vector3.Distance(this.transform.position, PlayersManager.instance.obj_players[myIndex].transform.position) >= WeaponManager.instance.infoSG[myIndex].range)
            {
                WeaponManager.instance.ReturnBulletToPool(gameObject);
            }
   */
   
            if (Vector3.Distance(this.transform.position, PlayersManager.instance.obj_players[myIndex].transform.position) >= WeaponManager.instance.infoAR[myIndex].range)
            {
                WeaponManager.instance.ReturnBulletToPool(gameObject);
            }
     
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
            return;

        Debug.Log(other.name + "과(와) 충돌하였다111111111111111111111111111.", this);
        WeaponManager.instance.ReturnBulletToPool(gameObject);
    }
    
    
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
            return;

        Debug.Log(collision.gameObject.name + "과(와) 충돌하였다222222222222222222.", this);
        WeaponManager.instance.ReturnBulletToPool(gameObject);
    }
    
}
