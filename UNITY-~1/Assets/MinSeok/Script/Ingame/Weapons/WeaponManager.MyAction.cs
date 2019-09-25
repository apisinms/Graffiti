using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WeaponManager : MonoBehaviour
{
    private void Start()
    {
        for(int i=0; i<C_Global.MAX_PLAYER; i++) //모든플레이어들의 총알풀 생성.
            CreateBulletPool(i, 30);
    }
    private void Update()
    {
           // Debug.Log(Vector3.Distance(sample);
    }

    public IEnumerator ActionBullet()
    {
        switch(mainWeapon[myIndex])
        {
            case _WEAPONS.AR:
                {
                    while (true)
                    {
                        ActionBullet_AR();
                        yield return YieldInstructionCache.WaitForSeconds(infoAR[myIndex].fireRate);
                    }
                }
            case _WEAPONS.SG:
                {
                    while (true)
                    {
                        ActionBullet_SG();
                        yield break;
                    }
                }
            case _WEAPONS.SMG:
                {
                    while (true)
                    {
                        ActionBullet_SMG();
                        yield return YieldInstructionCache.WaitForSeconds(infoSMG[myIndex].fireRate);
                    }
                }
        }
    }

    public void ActionBullet_AR()
    {
        var clone = GetBulletFromPool(myIndex);
        Transform tf_clone = clone.transform;

        infoAR[myIndex].vt_bulletPattern[0].x = tf_clone.forward.x - (tf_clone.right.x * infoAR[myIndex].accuracy);
        infoAR[myIndex].vt_bulletPattern[0].z = tf_clone.forward.z - (tf_clone.right.z * infoAR[myIndex].accuracy);
        infoAR[myIndex].vt_bulletPattern[1].x = tf_clone.forward.x;
        infoAR[myIndex].vt_bulletPattern[1].z = tf_clone.forward.z;
        infoAR[myIndex].vt_bulletPattern[2].x = tf_clone.forward.x + (tf_clone.right.x * infoAR[myIndex].accuracy);
        infoAR[myIndex].vt_bulletPattern[2].z = tf_clone.forward.z + (tf_clone.right.z * infoAR[myIndex].accuracy);

        tf_clone.localRotation = Quaternion.LookRotation(infoAR[myIndex].vt_bulletPattern[infoAR[myIndex].bulletPatternIndex]);
        clone.GetComponent<Rigidbody>().AddForce(infoAR[myIndex].vt_bulletPattern[infoAR[myIndex].bulletPatternIndex] * infoAR[myIndex].speed, ForceMode.Acceleration);
        
        switch (infoAR[myIndex].bulletPatternIndex)
        {
            case 0:
                infoAR[myIndex].bulletPatternIndex = 1;
                //Debug.Log("중");
                break;
            case 1:
                if (infoAR[myIndex].prevBulletPatternIndex == 1)
                {
                    infoAR[myIndex].bulletPatternIndex = 0;
                    infoAR[myIndex].prevBulletPatternIndex = 2;
                    //Debug.Log("좌");
                }
                else if (infoAR[myIndex].prevBulletPatternIndex == 2)
                {
                    infoAR[myIndex].bulletPatternIndex = 2;
                    infoAR[myIndex].prevBulletPatternIndex = 1;
                    //Debug.Log("우");
                }
                break;
            case 2:
                infoAR[myIndex].bulletPatternIndex = 1;
                //Debug.Log("중");
                break;
        }

        /*
         infoAR[myIndex].vt_bulletPattern[0].x = PlayersManager.instance.direction2[myIndex].x - (tf_clone.right.x * infoAR[myIndex].accuracy);
        infoAR[myIndex].vt_bulletPattern[0].z = PlayersManager.instance.direction2[myIndex].z - (tf_clone.right.z * infoAR[myIndex].accuracy);
        infoAR[myIndex].vt_bulletPattern[1].x = PlayersManager.instance.direction2[myIndex].x;
        infoAR[myIndex].vt_bulletPattern[1].z = PlayersManager.instance.direction2[myIndex].z;
        infoAR[myIndex].vt_bulletPattern[2].x = PlayersManager.instance.direction2[myIndex].x + (tf_clone.right.x * infoAR[myIndex].accuracy);
        infoAR[myIndex].vt_bulletPattern[2].z = PlayersManager.instance.direction2[myIndex].z + (tf_clone.right.z * infoAR[myIndex].accuracy);
        */
    }

    public void ActionBullet_SG()
    {
        for (int i = 0; i < 5; i++)
        {
            infoSG[myIndex].obj_bulletClone[i] = GetBulletFromPool(myIndex);
            infoSG[myIndex].tf_bulletClone[i] = infoSG[myIndex].obj_bulletClone[i].transform;
        }

        infoSG[myIndex].vt_bulletPattern[0, 0].x = (infoSG[myIndex].tf_bulletClone[0].forward.x - infoSG[myIndex].tf_bulletClone[0].right.x * 0.2f) - (infoSG[myIndex].tf_bulletClone[0].right.x * 0.4f);
        infoSG[myIndex].vt_bulletPattern[0, 0].z = (infoSG[myIndex].tf_bulletClone[0].forward.z - infoSG[myIndex].tf_bulletClone[0].right.z * 0.2f) - (infoSG[myIndex].tf_bulletClone[0].right.z * 0.4f);
        infoSG[myIndex].vt_bulletPattern[0, 1].x = (infoSG[myIndex].tf_bulletClone[1].forward.x - infoSG[myIndex].tf_bulletClone[1].right.x * 0.2f) - (infoSG[myIndex].tf_bulletClone[1].right.x * 0.2f);
        infoSG[myIndex].vt_bulletPattern[0, 1].z = (infoSG[myIndex].tf_bulletClone[1].forward.z - infoSG[myIndex].tf_bulletClone[1].right.z * 0.2f) - (infoSG[myIndex].tf_bulletClone[1].right.z * 0.2f);    
        infoSG[myIndex].vt_bulletPattern[0, 2].x = (infoSG[myIndex].tf_bulletClone[2].forward.x - infoSG[myIndex].tf_bulletClone[2].right.x * 0.2f);
        infoSG[myIndex].vt_bulletPattern[0, 2].z = (infoSG[myIndex].tf_bulletClone[2].forward.z - infoSG[myIndex].tf_bulletClone[2].right.z * 0.2f);
        infoSG[myIndex].vt_bulletPattern[0, 3].x = (infoSG[myIndex].tf_bulletClone[3].forward.x - infoSG[myIndex].tf_bulletClone[3].right.x * 0.2f) + (infoSG[myIndex].tf_bulletClone[3].right.x * 0.2f);
        infoSG[myIndex].vt_bulletPattern[0, 3].z = (infoSG[myIndex].tf_bulletClone[3].forward.z - infoSG[myIndex].tf_bulletClone[3].right.z * 0.2f) + (infoSG[myIndex].tf_bulletClone[3].right.z * 0.2f);
        infoSG[myIndex].vt_bulletPattern[0, 4].x = (infoSG[myIndex].tf_bulletClone[4].forward.x - infoSG[myIndex].tf_bulletClone[4].right.x * 0.2f) + (infoSG[myIndex].tf_bulletClone[4].right.x * 0.4f);
        infoSG[myIndex].vt_bulletPattern[0, 4].z = (infoSG[myIndex].tf_bulletClone[4].forward.z - infoSG[myIndex].tf_bulletClone[4].right.z * 0.2f) + (infoSG[myIndex].tf_bulletClone[4].right.z * 0.4f);

        infoSG[myIndex].vt_bulletPattern[1, 0].x = (infoSG[myIndex].tf_bulletClone[0].forward.x + infoSG[myIndex].tf_bulletClone[0].right.x * 0.2f) - (infoSG[myIndex].tf_bulletClone[0].right.x * 0.4f);
        infoSG[myIndex].vt_bulletPattern[1, 0].z = (infoSG[myIndex].tf_bulletClone[0].forward.z + infoSG[myIndex].tf_bulletClone[0].right.z * 0.2f) - (infoSG[myIndex].tf_bulletClone[0].right.z * 0.4f);
        infoSG[myIndex].vt_bulletPattern[1, 1].x = (infoSG[myIndex].tf_bulletClone[1].forward.x + infoSG[myIndex].tf_bulletClone[1].right.x * 0.2f) - (infoSG[myIndex].tf_bulletClone[1].right.x * 0.2f);
        infoSG[myIndex].vt_bulletPattern[1, 1].z = (infoSG[myIndex].tf_bulletClone[1].forward.z + infoSG[myIndex].tf_bulletClone[1].right.z * 0.2f) - (infoSG[myIndex].tf_bulletClone[1].right.z * 0.2f);
        infoSG[myIndex].vt_bulletPattern[1, 2].x = (infoSG[myIndex].tf_bulletClone[2].forward.x + infoSG[myIndex].tf_bulletClone[2].right.x * 0.2f);
        infoSG[myIndex].vt_bulletPattern[1, 2].z = (infoSG[myIndex].tf_bulletClone[2].forward.z + infoSG[myIndex].tf_bulletClone[2].right.z * 0.2f);
        infoSG[myIndex].vt_bulletPattern[1, 3].x = (infoSG[myIndex].tf_bulletClone[3].forward.x + infoSG[myIndex].tf_bulletClone[3].right.x * 0.2f) + (infoSG[myIndex].tf_bulletClone[3].right.x * 0.2f);
        infoSG[myIndex].vt_bulletPattern[1, 3].z = (infoSG[myIndex].tf_bulletClone[3].forward.z + infoSG[myIndex].tf_bulletClone[3].right.z * 0.2f) + (infoSG[myIndex].tf_bulletClone[3].right.z * 0.2f);
        infoSG[myIndex].vt_bulletPattern[1, 4].x = (infoSG[myIndex].tf_bulletClone[4].forward.x + infoSG[myIndex].tf_bulletClone[4].right.x * 0.2f) + (infoSG[myIndex].tf_bulletClone[4].right.x * 0.4f);
        infoSG[myIndex].vt_bulletPattern[1, 4].z = (infoSG[myIndex].tf_bulletClone[4].forward.z + infoSG[myIndex].tf_bulletClone[4].right.z * 0.2f) + (infoSG[myIndex].tf_bulletClone[4].right.z * 0.4f);

        for (int i = 0; i < infoSG[myIndex].obj_bulletClone.Length; i++)
        {
            infoSG[myIndex].tf_bulletClone[i].localRotation = Quaternion.LookRotation(infoSG[myIndex].vt_bulletPattern[infoSG[myIndex].bulletPatternIndex, i]);
            infoSG[myIndex].obj_bulletClone[i].GetComponent<Rigidbody>().AddForce(infoSG[myIndex].vt_bulletPattern[infoSG[myIndex].bulletPatternIndex, i] * infoSG[myIndex].speed, ForceMode.Acceleration);
        }

        switch (infoSG[myIndex].bulletPatternIndex)
        {
            case 0:
                infoSG[myIndex].bulletPatternIndex = 1;
                break;
            case 1:
                infoSG[myIndex].bulletPatternIndex = 0;
                break;
        }
        /*
        infoSG[myIndex].vt_bulletDir[0].x = PlayersManager.instance.direction2[myIndex].x - (infoSG[myIndex].tf_bulletClone[0].right.x * 0.2f);
        infoSG[myIndex].vt_bulletDir[0].z = PlayersManager.instance.direction2[myIndex].z - (infoSG[myIndex].tf_bulletClone[0].right.z * 0.2f);
        infoSG[myIndex].vt_bulletDir[1].x = PlayersManager.instance.direction2[myIndex].x - (infoSG[myIndex].tf_bulletClone[1].right.x * 0.1f);
        infoSG[myIndex].vt_bulletDir[1].z = PlayersManager.instance.direction2[myIndex].z - (infoSG[myIndex].tf_bulletClone[1].right.z * 0.1f);
        infoSG[myIndex].vt_bulletDir[2].x = PlayersManager.instance.direction2[myIndex].x;
        infoSG[myIndex].vt_bulletDir[2].z = PlayersManager.instance.direction2[myIndex].z;
        infoSG[myIndex].vt_bulletDir[3].x = PlayersManager.instance.direction2[myIndex].x + (infoSG[myIndex].tf_bulletClone[3].right.x * 0.1f);
        infoSG[myIndex].vt_bulletDir[3].z = PlayersManager.instance.direction2[myIndex].z + (infoSG[myIndex].tf_bulletClone[3].right.z * 0.1f);
        infoSG[myIndex].vt_bulletDir[4].x = PlayersManager.instance.direction2[myIndex].x + (infoSG[myIndex].tf_bulletClone[4].right.x * 0.2f);
        infoSG[myIndex].vt_bulletDir[4].z = PlayersManager.instance.direction2[myIndex].z + (infoSG[myIndex].tf_bulletClone[4].right.z * 0.2f);
        */
    }

    public void ActionBullet_SMG()
    {
        var clone = GetBulletFromPool(myIndex);
        Transform tf_clone = clone.transform;

        infoSMG[myIndex].vt_bulletPattern[0].x = tf_clone.forward.x - (tf_clone.right.x * infoSMG[myIndex].accuracy);
        infoSMG[myIndex].vt_bulletPattern[0].z = tf_clone.forward.z - (tf_clone.right.z * infoSMG[myIndex].accuracy);
        infoSMG[myIndex].vt_bulletPattern[1].x = tf_clone.forward.x;
        infoSMG[myIndex].vt_bulletPattern[1].z = tf_clone.forward.z;
        infoSMG[myIndex].vt_bulletPattern[2].x = tf_clone.forward.x + (tf_clone.right.x * infoSMG[myIndex].accuracy);
        infoSMG[myIndex].vt_bulletPattern[2].z = tf_clone.forward.z + (tf_clone.right.z * infoSMG[myIndex].accuracy);

        tf_clone.localRotation = Quaternion.LookRotation(infoSMG[myIndex].vt_bulletPattern[infoSMG[myIndex].bulletPatternIndex]);
        clone.GetComponent<Rigidbody>().AddForce(infoSMG[myIndex].vt_bulletPattern[infoSMG[myIndex].bulletPatternIndex] * infoSMG[myIndex].speed, ForceMode.Acceleration);

        switch (infoSMG[myIndex].bulletPatternIndex)
        {
            case 0:
                infoSMG[myIndex].bulletPatternIndex = 1;
                //Debug.Log("중");
                break;
            case 1:
                if (infoSMG[myIndex].prevBulletPatternIndex == 1)
                {
                    infoSMG[myIndex].bulletPatternIndex = 0;
                    infoSMG[myIndex].prevBulletPatternIndex = 2;
                    //Debug.Log("좌");
                }
                else if (infoSMG[myIndex].prevBulletPatternIndex == 2)
                {
                    infoSMG[myIndex].bulletPatternIndex = 2;
                    infoSMG[myIndex].prevBulletPatternIndex = 1;
                    //Debug.Log("우");
                }
                break;
            case 2:
                infoSMG[myIndex].bulletPatternIndex = 1;
                //Debug.Log("중");
                break;
        }

        /*
         infoAR[myIndex].vt_bulletPattern[0].x = PlayersManager.instance.direction2[myIndex].x - (tf_clone.right.x * infoAR[myIndex].accuracy);
        infoAR[myIndex].vt_bulletPattern[0].z = PlayersManager.instance.direction2[myIndex].z - (tf_clone.right.z * infoAR[myIndex].accuracy);
        infoAR[myIndex].vt_bulletPattern[1].x = PlayersManager.instance.direction2[myIndex].x;
        infoAR[myIndex].vt_bulletPattern[1].z = PlayersManager.instance.direction2[myIndex].z;
        infoAR[myIndex].vt_bulletPattern[2].x = PlayersManager.instance.direction2[myIndex].x + (tf_clone.right.x * infoAR[myIndex].accuracy);
        infoAR[myIndex].vt_bulletPattern[2].z = PlayersManager.instance.direction2[myIndex].z + (tf_clone.right.z * infoAR[myIndex].accuracy);
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
