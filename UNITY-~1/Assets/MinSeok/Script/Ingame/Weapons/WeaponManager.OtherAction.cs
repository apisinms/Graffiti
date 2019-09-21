﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WeaponManager : MonoBehaviour
{
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
                        yield break; //YieldInstructionCache.WaitForSeconds(infoAR[myIndex].fireRate);
                    }
                }
        }
    }

    public void ActionBullet_AR(int _index)
    {
        infoAR[_index].vt_bulletPattern[0].x = PlayersManager.instance.direction2[_index].x - infoAR[_index].accuracy;
        infoAR[_index].vt_bulletPattern[0].z = PlayersManager.instance.direction2[_index].z - infoAR[_index].accuracy;
        infoAR[_index].vt_bulletPattern[1].x = PlayersManager.instance.direction2[_index].x;
        infoAR[_index].vt_bulletPattern[1].z = PlayersManager.instance.direction2[_index].z;
        infoAR[_index].vt_bulletPattern[2].x = PlayersManager.instance.direction2[_index].x + infoAR[_index].accuracy;
        infoAR[_index].vt_bulletPattern[2].z = PlayersManager.instance.direction2[_index].z + infoAR[_index].accuracy;

        var clone = GetBulletFromPool(_index);
        clone.GetComponent<Rigidbody>().AddForce(infoAR[_index].vt_bulletPattern[infoAR[_index].bulletPatternIndex] * 2000.0f, ForceMode.Acceleration);

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

        Vector3 tmp0 = (RightJoystick.instance.right_joystick.touchPos - (RightJoystick.instance.right_joystick.stickFirstPos - new Vector3(20.0f, 20.0f, 0))).normalized;
        Vector3 tmp1 = (RightJoystick.instance.right_joystick.touchPos - (RightJoystick.instance.right_joystick.stickFirstPos - new Vector3(10.0f, 10.0f, 0))).normalized;
        Vector3 tmp2 = (RightJoystick.instance.right_joystick.touchPos - RightJoystick.instance.right_joystick.stickFirstPos).normalized;
        Vector3 tmp3 = (RightJoystick.instance.right_joystick.touchPos - (RightJoystick.instance.right_joystick.stickFirstPos + new Vector3(10.0f, 10.0f, 0))).normalized;
        Vector3 tmp4 = (RightJoystick.instance.right_joystick.touchPos - (RightJoystick.instance.right_joystick.stickFirstPos + new Vector3(20.0f, 20.0f, 0))).normalized;

        infoSG[_index].vt_bulletDir[0].x = tmp0.x; infoSG[_index].vt_bulletDir[1].x = tmp1.x;
        infoSG[_index].vt_bulletDir[0].z = tmp0.y; infoSG[_index].vt_bulletDir[1].z = tmp1.y;
        infoSG[_index].vt_bulletDir[2].x = tmp2.x; infoSG[_index].vt_bulletDir[3].x = tmp3.x;
        infoSG[_index].vt_bulletDir[2].z = tmp2.y; infoSG[_index].vt_bulletDir[3].z = tmp3.y;
        infoSG[_index].vt_bulletDir[4].x = tmp4.x;
        infoSG[_index].vt_bulletDir[4].z = tmp4.y;

        for (int i = 0; i < infoSG[_index].obj_bulletClone.Length; i++)
        {
            infoSG[_index].tf_bulletClone[i].localRotation = Quaternion.LookRotation(infoSG[_index].vt_bulletDir[i]);
            infoSG[_index].obj_bulletClone[i].GetComponent<Rigidbody>().AddForce(infoSG[_index].vt_bulletDir[i] * 2000.0f, ForceMode.Acceleration);
        }
        /*
        clone1.GetComponent<Rigidbody>().AddForce(new Vector3(PlayersManager.instance.direction2[myIndex].x - 0.1f, 0, PlayersManager.instance.direction2[myIndex].z - 0.1f) * 2000.0f);
        clone2.GetComponent<Rigidbody>().AddForce(new Vector3(PlayersManager.instance.direction2[myIndex].x - 0.05f, 0, PlayersManager.instance.direction2[myIndex].z - 0.05f) * 2000.0f);
        clone3.GetComponent<Rigidbody>().AddForce(PlayersManager.instance.direction2[myIndex] * 2000.0f);
        clone4.GetComponent<Rigidbody>().AddForce(new Vector3(PlayersManager.instance.direction2[myIndex].x + 0.05f, 0, PlayersManager.instance.direction2[myIndex].z + 0.05f) * 2000.0f);
        clone5.GetComponent<Rigidbody>().AddForce(new Vector3(PlayersManager.instance.direction2[myIndex].x + 0.1f, 0, PlayersManager.instance.direction2[myIndex].z + 0.1f) * 2000.0f);
        */
    }
}
