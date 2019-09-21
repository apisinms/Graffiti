using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public static BulletCollision instance;
    public int myIndex { get; set; }
    //public Coroutine curCheckRangeCor { get; set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        myIndex = GameManager.instance.myIndex;
        //curCheckRangeCor = null;
    }

    private void Update()
    {
        //CheckBulletRange(); //얘도 코루틴이든 인보크든 바꿔야됨
    }

    public void CheckBulletRange()
    {
        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            switch (WeaponManager.instance.mainWeapon[i])
            {
                case _WEAPONS.AR:
                    {
                        if (PlayersManager.instance.actionState[i] == _ACTION_STATE.SHOT || PlayersManager.instance.actionState[i] == _ACTION_STATE.CIR_AIM_SHOT)
                        {
                            if (Vector3.Distance(this.transform.position, PlayersManager.instance.obj_players[i].transform.position) >= WeaponManager.instance.infoAR[i].range)
                                WeaponManager.instance.ReturnBulletToPool(gameObject, i);
                        }
                    }
                    break;

                case _WEAPONS.SG:
                    {
                        if (Vector3.Distance(this.transform.position, PlayersManager.instance.obj_players[i].transform.position) >= WeaponManager.instance.infoSG[i].range)
                            WeaponManager.instance.ReturnBulletToPool(gameObject, i);
                    }
                    break;
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
            return;

        // Debug.Log(other.name + "과(와) 충돌하였다111111111111111111111111111.", this);

        if (this.transform.parent.CompareTag("BulletPool1"))
            WeaponManager.instance.ReturnBulletToPool(gameObject, 0);

        else if (this.transform.parent.CompareTag("BulletPool2"))
            WeaponManager.instance.ReturnBulletToPool(gameObject, 1);

        else if (this.transform.parent.CompareTag("BulletPool3"))
            WeaponManager.instance.ReturnBulletToPool(gameObject, 2);

        else if (this.transform.parent.CompareTag("BulletPool4"))
            WeaponManager.instance.ReturnBulletToPool(gameObject, 3);
    }
    
    
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
            return;

        //  Debug.Log(collision.gameObject.name + "과(와) 충돌하였다222222222222222222.", this);
        WeaponManager.instance.ReturnBulletToPool(gameObject);
    }
*/

}
