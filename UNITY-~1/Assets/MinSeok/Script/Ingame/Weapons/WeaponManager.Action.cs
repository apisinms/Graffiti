using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WeaponManager : MonoBehaviour
{
    public Coroutine curCor { get; set; }
    public IEnumerator cor_ActionBullet { get; set; }
    public GameObject[] sample { get; set; }

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
        
        while (true)
        {
            Debug.Log("실행");

            var clone1 = GetBulletFromPool();
            var clone2 = GetBulletFromPool();
            var clone3 = GetBulletFromPool();
            var clone4 = GetBulletFromPool();
            var clone5 = GetBulletFromPool();

          //  clone1.transform.localPosition = new Vector3(clone1.transform.localPosition.x - 2.0f, 1, clone1.transform.localPosition.z);
          //  clone2.transform.localPosition = new Vector3(clone2.transform.localPosition.x - 1.0f, 1, clone2.transform.localPosition.z);
           // clone3.transform.localPosition = new Vector3(clone3.transform.localPosition.x        , 1, clone3.transform.localPosition.z);
           // clone4.transform.localPosition = new Vector3(clone4.transform.localPosition.x + 1.0f, 1, clone4.transform.localPosition.z);
           // clone5.transform.localPosition = new Vector3(clone5.transform.localPosition.x + 2.0f, 1, clone5.transform.localPosition.z);


            clone1.SetActive(true);
            clone2.SetActive(true);
            clone3.SetActive(true);
            clone4.SetActive(true);
            clone5.SetActive(true);
            
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

            // yield break;
            yield return YieldInstructionCache.WaitForSeconds(5.0f);
        }
        

        /*
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
         */

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
