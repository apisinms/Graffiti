using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WeaponManager : MonoBehaviour
{
    private Vector3 tmpRot = new Vector3();

    public IEnumerator ActionBullet(int _index)
    {
        if (_index == myIndex)
            yield break;

        switch (mainWeapon[_index])
        {
            case _WEAPONS.AR:
                {
                    while (true)
                    {
                        ActionBullet_AR(_index);
                        yield return YieldInstructionCache.WaitForSeconds(infoAR[_index].fireRate);
                    }
                }
            case _WEAPONS.SG:
                {
                    while (true)
                    {
                        ActionBullet_SG(_index);
                        yield break;
                    }
                }
            case _WEAPONS.SMG:
                {
                    while (true)
                    {
                        ActionBullet_SMG(_index);
                        yield return YieldInstructionCache.WaitForSeconds(infoSMG[_index].fireRate);
                    }
                }
        }
    }

    public void ActionBullet_AR(int _index)
    {
        var clone = GetBulletFromPool(_index);
        Transform tf_clone = clone.transform;

        infoAR[_index].vt_bulletPattern[0].x = tf_clone.forward.x - (tf_clone.right.x * infoAR[_index].accuracy);
        infoAR[_index].vt_bulletPattern[0].z = tf_clone.forward.z - (tf_clone.right.z * infoAR[_index].accuracy);
        infoAR[_index].vt_bulletPattern[1].x = tf_clone.forward.x;
        infoAR[_index].vt_bulletPattern[1].z = tf_clone.forward.z;
        infoAR[_index].vt_bulletPattern[2].x = tf_clone.forward.x + (tf_clone.right.x * infoAR[_index].accuracy);
        infoAR[_index].vt_bulletPattern[2].z = tf_clone.forward.z + (tf_clone.right.z * infoAR[_index].accuracy);

        tf_clone.localRotation = Quaternion.LookRotation(infoAR[_index].vt_bulletPattern[infoAR[_index].bulletPatternIndex]);
        clone.GetComponent<Rigidbody>().AddForce(infoAR[_index].vt_bulletPattern[infoAR[_index].bulletPatternIndex] * infoAR[_index].speed, ForceMode.Acceleration);

        switch (infoAR[_index].bulletPatternIndex)
        {
            case 0:
                infoAR[_index].bulletPatternIndex = 1;
                //Debug.Log("중");
                break;
            case 1:
                if (infoAR[_index].prevBulletPatternIndex == 1)
                {
                    infoAR[_index].bulletPatternIndex = 0;
                    infoAR[_index].prevBulletPatternIndex = 2;
                    //Debug.Log("좌");
                }
                else if (infoAR[_index].prevBulletPatternIndex == 2)
                {
                    infoAR[_index].bulletPatternIndex = 2;
                    infoAR[_index].prevBulletPatternIndex = 1;
                    //Debug.Log("우");
                }
                break;
            case 2:
                infoAR[_index].bulletPatternIndex = 1;
                //Debug.Log("중");
                break;
        }
    }

    public void ActionBullet_SG(int _index)
    {
        for (int i = 0; i < 5; i++)
        {
            infoSG[_index].obj_bulletClone[i] = GetBulletFromPool(_index);
            infoSG[_index].tf_bulletClone[i] = infoSG[_index].obj_bulletClone[i].transform;
        }

        infoSG[_index].vt_bulletPattern[0, 0].x = (infoSG[_index].tf_bulletClone[0].forward.x - infoSG[_index].tf_bulletClone[0].right.x * 0.2f) - (infoSG[_index].tf_bulletClone[0].right.x * 0.2f);
        infoSG[_index].vt_bulletPattern[0, 0].z = (infoSG[_index].tf_bulletClone[0].forward.z - infoSG[_index].tf_bulletClone[0].right.z * 0.2f) - (infoSG[_index].tf_bulletClone[0].right.z * 0.2f);
        infoSG[_index].vt_bulletPattern[0, 1].x = (infoSG[_index].tf_bulletClone[1].forward.x - infoSG[_index].tf_bulletClone[1].right.x * 0.2f) - (infoSG[_index].tf_bulletClone[1].right.x * 0.1f);
        infoSG[_index].vt_bulletPattern[0, 1].z = (infoSG[_index].tf_bulletClone[1].forward.z - infoSG[_index].tf_bulletClone[1].right.z * 0.2f) - (infoSG[_index].tf_bulletClone[1].right.z * 0.1f);
        infoSG[_index].vt_bulletPattern[0, 2].x = (infoSG[_index].tf_bulletClone[2].forward.x - infoSG[_index].tf_bulletClone[2].right.x * 0.2f);
        infoSG[_index].vt_bulletPattern[0, 2].z = (infoSG[_index].tf_bulletClone[2].forward.z - infoSG[_index].tf_bulletClone[2].right.z * 0.2f);
        infoSG[_index].vt_bulletPattern[0, 3].x = (infoSG[_index].tf_bulletClone[3].forward.x - infoSG[_index].tf_bulletClone[3].right.x * 0.2f) + (infoSG[_index].tf_bulletClone[3].right.x * 0.1f);
        infoSG[_index].vt_bulletPattern[0, 3].z = (infoSG[_index].tf_bulletClone[3].forward.z - infoSG[_index].tf_bulletClone[3].right.z * 0.2f) + (infoSG[_index].tf_bulletClone[3].right.z * 0.1f);
        infoSG[_index].vt_bulletPattern[0, 4].x = (infoSG[_index].tf_bulletClone[4].forward.x - infoSG[_index].tf_bulletClone[4].right.x * 0.2f) + (infoSG[_index].tf_bulletClone[4].right.x * 0.2f);
        infoSG[_index].vt_bulletPattern[0, 4].z = (infoSG[_index].tf_bulletClone[4].forward.z - infoSG[_index].tf_bulletClone[4].right.z * 0.2f) + (infoSG[_index].tf_bulletClone[4].right.z * 0.2f);
                                       
        infoSG[_index].vt_bulletPattern[1, 0].x = (infoSG[_index].tf_bulletClone[0].forward.x + infoSG[_index].tf_bulletClone[0].right.x * 0.2f) - (infoSG[_index].tf_bulletClone[0].right.x * 0.2f);
        infoSG[_index].vt_bulletPattern[1, 0].z = (infoSG[_index].tf_bulletClone[0].forward.z + infoSG[_index].tf_bulletClone[0].right.z * 0.2f) - (infoSG[_index].tf_bulletClone[0].right.z * 0.2f);
        infoSG[_index].vt_bulletPattern[1, 1].x = (infoSG[_index].tf_bulletClone[1].forward.x + infoSG[_index].tf_bulletClone[1].right.x * 0.2f) - (infoSG[_index].tf_bulletClone[1].right.x * 0.1f);
        infoSG[_index].vt_bulletPattern[1, 1].z = (infoSG[_index].tf_bulletClone[1].forward.z + infoSG[_index].tf_bulletClone[1].right.z * 0.2f) - (infoSG[_index].tf_bulletClone[1].right.z * 0.1f);
        infoSG[_index].vt_bulletPattern[1, 2].x = (infoSG[_index].tf_bulletClone[2].forward.x + infoSG[_index].tf_bulletClone[2].right.x * 0.2f);
        infoSG[_index].vt_bulletPattern[1, 2].z = (infoSG[_index].tf_bulletClone[2].forward.z + infoSG[_index].tf_bulletClone[2].right.z * 0.2f);
        infoSG[_index].vt_bulletPattern[1, 3].x = (infoSG[_index].tf_bulletClone[3].forward.x + infoSG[_index].tf_bulletClone[3].right.x * 0.2f) + (infoSG[_index].tf_bulletClone[3].right.x * 0.1f);
        infoSG[_index].vt_bulletPattern[1, 3].z = (infoSG[_index].tf_bulletClone[3].forward.z + infoSG[_index].tf_bulletClone[3].right.z * 0.2f) + (infoSG[_index].tf_bulletClone[3].right.z * 0.1f);
        infoSG[_index].vt_bulletPattern[1, 4].x = (infoSG[_index].tf_bulletClone[4].forward.x + infoSG[_index].tf_bulletClone[4].right.x * 0.2f) + (infoSG[_index].tf_bulletClone[4].right.x * 0.2f);
        infoSG[_index].vt_bulletPattern[1, 4].z = (infoSG[_index].tf_bulletClone[4].forward.z + infoSG[_index].tf_bulletClone[4].right.z * 0.2f) + (infoSG[_index].tf_bulletClone[4].right.z * 0.2f);

        for (int i = 0; i < infoSG[_index].obj_bulletClone.Length; i++)
        {
            infoSG[_index].tf_bulletClone[i].localRotation = Quaternion.LookRotation(infoSG[_index].vt_bulletPattern[infoSG[_index].bulletPatternIndex, i]);
            infoSG[_index].obj_bulletClone[i].GetComponent<Rigidbody>().AddForce(infoSG[_index].vt_bulletPattern[infoSG[_index].bulletPatternIndex, i] * infoSG[_index].speed, ForceMode.Acceleration);
        }

        switch (infoSG[_index].bulletPatternIndex)
        {
            case 0:
                infoSG[_index].bulletPatternIndex = 1;
                break;
            case 1:
                infoSG[_index].bulletPatternIndex = 0;
                break;
        }
        /*
        infoSG[_index].vt_bulletDir[0].x = PlayersManager.instance.direction2[myIndex].x - (infoSG[_index].tf_bulletClone[0].right.x * 0.2f);
        infoSG[_index].vt_bulletDir[0].z = PlayersManager.instance.direction2[myIndex].z - (infoSG[_index].tf_bulletClone[0].right.z * 0.2f);
        infoSG[_index].vt_bulletDir[1].x = PlayersManager.instance.direction2[myIndex].x - (infoSG[_index].tf_bulletClone[1].right.x * 0.1f);
        infoSG[_index].vt_bulletDir[1].z = PlayersManager.instance.direction2[myIndex].z - (infoSG[_index].tf_bulletClone[1].right.z * 0.1f);
        infoSG[_index].vt_bulletDir[2].x = PlayersManager.instance.direction2[myIndex].x;
        infoSG[_index].vt_bulletDir[2].z = PlayersManager.instance.direction2[myIndex].z;
        infoSG[_index].vt_bulletDir[3].x = PlayersManager.instance.direction2[myIndex].x + (infoSG[_index].tf_bulletClone[3].right.x * 0.1f);
        infoSG[_index].vt_bulletDir[3].z = PlayersManager.instance.direction2[myIndex].z + (infoSG[_index].tf_bulletClone[3].right.z * 0.1f);
        infoSG[_index].vt_bulletDir[4].x = PlayersManager.instance.direction2[myIndex].x + (infoSG[_index].tf_bulletClone[4].right.x * 0.2f);
        infoSG[_index].vt_bulletDir[4].z = PlayersManager.instance.direction2[myIndex].z + (infoSG[_index].tf_bulletClone[4].right.z * 0.2f);
        */
    }

    public void ActionBullet_SMG(int _index)
    {
        var clone = GetBulletFromPool(_index);
        Transform tf_clone = clone.transform;

        infoSMG[_index].vt_bulletPattern[0].x = tf_clone.forward.x - (tf_clone.right.x * infoSMG[_index].accuracy);
        infoSMG[_index].vt_bulletPattern[0].z = tf_clone.forward.z - (tf_clone.right.z * infoSMG[_index].accuracy);
        infoSMG[_index].vt_bulletPattern[1].x = tf_clone.forward.x;
        infoSMG[_index].vt_bulletPattern[1].z = tf_clone.forward.z;
        infoSMG[_index].vt_bulletPattern[2].x = tf_clone.forward.x + (tf_clone.right.x * infoSMG[_index].accuracy);
        infoSMG[_index].vt_bulletPattern[2].z = tf_clone.forward.z + (tf_clone.right.z * infoSMG[_index].accuracy);

        tf_clone.localRotation = Quaternion.LookRotation(infoSMG[_index].vt_bulletPattern[infoSMG[_index].bulletPatternIndex]);
        clone.GetComponent<Rigidbody>().AddForce(infoSMG[_index].vt_bulletPattern[infoSMG[_index].bulletPatternIndex] * infoSMG[_index].speed, ForceMode.Acceleration);

        switch (infoSMG[_index].bulletPatternIndex)
        {
            case 0:
                infoSMG[_index].bulletPatternIndex = 1;
                //Debug.Log("중");
                break;
            case 1:
                if (infoSMG[_index].prevBulletPatternIndex == 1)
                {
                    infoSMG[_index].bulletPatternIndex = 0;
                    infoSMG[_index].prevBulletPatternIndex = 2;
                    //Debug.Log("좌");
                }
                else if (infoSMG[_index].prevBulletPatternIndex == 2)
                {
                    infoSMG[_index].bulletPatternIndex = 2;
                    infoSMG[_index].prevBulletPatternIndex = 1;
                    //Debug.Log("우");
                }
                break;
            case 2:
                infoSMG[_index].bulletPatternIndex = 1;
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
