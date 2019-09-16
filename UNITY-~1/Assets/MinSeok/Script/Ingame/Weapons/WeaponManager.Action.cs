using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WeaponManager : MonoBehaviour
{
    public Coroutine curActionCor { get; set; }

    private void Start()
    {
        CreateBulletPool(infoAR[myIndex].maxAmmo);
    }
    private void Update()
    {
           // Debug.Log(Vector3.Distance(sample);
    }

    public IEnumerator ActionBullet()
    {
        if (mainWeapon[myIndex] == _WEAPONS.AR)
        {
            while (true)
            {
                //if (infoAR[myIndex].curAmmo <= 0)
                //yield break;

                var clone = GetBulletFromPool();

                clone.GetComponent<Rigidbody>().AddForce(
                    new Vector3(Random.Range(PlayersManager.instance.direction2[myIndex].x - infoAR[myIndex].accuracy,
                    PlayersManager.instance.direction2[myIndex].x + infoAR[myIndex].accuracy),
                    0,
                    Random.Range(PlayersManager.instance.direction2[myIndex].z - infoAR[myIndex].accuracy,
                    PlayersManager.instance.direction2[myIndex].z + infoAR[myIndex].accuracy)) * 2000.0f);


                //infoAR[myIndex].curAmmo--;

                yield return YieldInstructionCache.WaitForSeconds(infoAR[myIndex].fireRate);
            }
        }
        else if(mainWeapon[myIndex] == _WEAPONS.SG)
        {
            while (true)
            {
                var clone1 = GetBulletFromPool();
                var clone2 = GetBulletFromPool();
                var clone3 = GetBulletFromPool();
                var clone4 = GetBulletFromPool();
                var clone5 = GetBulletFromPool();

                clone1.GetComponent<Rigidbody>().AddForce(
                    new Vector3(Random.Range(PlayersManager.instance.direction2[myIndex].x - infoSG[myIndex].accuracy,
                    PlayersManager.instance.direction2[myIndex].x + infoSG[myIndex].accuracy),
                    0,
                    Random.Range(PlayersManager.instance.direction2[myIndex].z - infoSG[myIndex].accuracy,
                    PlayersManager.instance.direction2[myIndex].z + infoSG[myIndex].accuracy)) * 2000.0f);

                clone2.GetComponent<Rigidbody>().AddForce(
            new Vector3(Random.Range(PlayersManager.instance.direction2[myIndex].x - infoSG[myIndex].accuracy,
            PlayersManager.instance.direction2[myIndex].x + infoSG[myIndex].accuracy),
            0,
            Random.Range(PlayersManager.instance.direction2[myIndex].z - infoSG[myIndex].accuracy,
            PlayersManager.instance.direction2[myIndex].z + infoSG[myIndex].accuracy)) * 2000.0f);


                clone3.GetComponent<Rigidbody>().AddForce(
            new Vector3(Random.Range(PlayersManager.instance.direction2[myIndex].x - infoSG[myIndex].accuracy,
            PlayersManager.instance.direction2[myIndex].x + infoSG[myIndex].accuracy),
            0,
            Random.Range(PlayersManager.instance.direction2[myIndex].z - infoSG[myIndex].accuracy,
            PlayersManager.instance.direction2[myIndex].z + infoSG[myIndex].accuracy)) * 2000.0f);


                clone4.GetComponent<Rigidbody>().AddForce(
            new Vector3(Random.Range(PlayersManager.instance.direction2[myIndex].x - infoSG[myIndex].accuracy,
            PlayersManager.instance.direction2[myIndex].x + infoSG[myIndex].accuracy),
            0,
            Random.Range(PlayersManager.instance.direction2[myIndex].z - infoSG[myIndex].accuracy,
            PlayersManager.instance.direction2[myIndex].z + infoSG[myIndex].accuracy)) * 2000.0f);


                clone5.GetComponent<Rigidbody>().AddForce(
            new Vector3(Random.Range(PlayersManager.instance.direction2[myIndex].x - infoSG[myIndex].accuracy,
            PlayersManager.instance.direction2[myIndex].x + infoSG[myIndex].accuracy),
            0,
            Random.Range(PlayersManager.instance.direction2[myIndex].z - infoSG[myIndex].accuracy,
            PlayersManager.instance.direction2[myIndex].z + infoSG[myIndex].accuracy)) * 2000.0f);

                yield break;
            }
        }


    }
}
/*
 * float time = 0;
 *
             time += Time.smoothDeltaTime;

            if (time >= infoAR[myIndex].fireRate)
            {
                  

                time = 0;
            }
 */
