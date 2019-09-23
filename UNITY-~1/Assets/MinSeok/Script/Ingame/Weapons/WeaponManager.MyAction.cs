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
                        yield break; //YieldInstructionCache.WaitForSeconds(infoAR[myIndex].fireRate);
                    }
                }
        }
    }

    public void ActionBullet_AR()
    {
        var clone = GetBulletFromPool(myIndex);
        /*
        infoAR[myIndex].vt_bulletPattern[0].x = PlayersManager.instance.direction2[myIndex].x - 0.04f;
        infoAR[myIndex].vt_bulletPattern[0].z = PlayersManager.instance.direction2[myIndex].z - 0.08f;
        infoAR[myIndex].vt_bulletPattern[1].x = PlayersManager.instance.direction2[myIndex].x;
        infoAR[myIndex].vt_bulletPattern[1].z = PlayersManager.instance.direction2[myIndex].z;
        infoAR[myIndex].vt_bulletPattern[2].x = PlayersManager.instance.direction2[myIndex].x + 0.04f;
        infoAR[myIndex].vt_bulletPattern[2].z = PlayersManager.instance.direction2[myIndex].z + 0.08f;
        */
        //var clone = GetBulletFromPool(myIndex);
        infoAR[myIndex].vt_bulletPattern[0].x = clone.transform.forward.x - (clone.transform.right.x * infoAR[myIndex].accuracy);
        infoAR[myIndex].vt_bulletPattern[0].z = clone.transform.forward.z - (clone.transform.right.z * infoAR[myIndex].accuracy);
        infoAR[myIndex].vt_bulletPattern[1].x = clone.transform.forward.x;
        infoAR[myIndex].vt_bulletPattern[1].z = clone.transform.forward.z;
        infoAR[myIndex].vt_bulletPattern[2].x = clone.transform.forward.x + (clone.transform.right.x * infoAR[myIndex].accuracy);
        infoAR[myIndex].vt_bulletPattern[2].z = clone.transform.forward.z + (clone.transform.right.z * infoAR[myIndex].accuracy);

        clone.transform.localRotation = Quaternion.LookRotation(infoAR[myIndex].vt_bulletPattern[infoAR[myIndex].bulletPatternIndex]);
        clone.GetComponent<Rigidbody>().AddForce(infoAR[myIndex].vt_bulletPattern[infoAR[myIndex].bulletPatternIndex] * 2000.0f, ForceMode.Acceleration);

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
    }
    public void ActionBullet_SG()
    {
        for (int i = 0; i < 5; i++)
        {
            infoSG[myIndex].obj_bulletClone[i] = GetBulletFromPool(myIndex);
            infoSG[myIndex].tf_bulletClone[i] = infoSG[myIndex].obj_bulletClone[i].transform;
        }

        infoSG[myIndex].vt_bulletDir[0] = new Vector3(infoSG[myIndex].tf_bulletClone[0].forward.x - (infoSG[myIndex].tf_bulletClone[0].right.x * 0.2f), 0, infoSG[myIndex].tf_bulletClone[0].forward.z - (infoSG[myIndex].tf_bulletClone[0].right.z * 0.2f));
        infoSG[myIndex].vt_bulletDir[1] = new Vector3(infoSG[myIndex].tf_bulletClone[1].forward.x - (infoSG[myIndex].tf_bulletClone[1].right.x * 0.1f), 0, infoSG[myIndex].tf_bulletClone[1].forward.z - (infoSG[myIndex].tf_bulletClone[1].right.z * 0.1f));
        infoSG[myIndex].vt_bulletDir[2] = new Vector3(infoSG[myIndex].tf_bulletClone[2].forward.x, 0, infoSG[myIndex].tf_bulletClone[2].forward.z);
        infoSG[myIndex].vt_bulletDir[3] = new Vector3(infoSG[myIndex].tf_bulletClone[3].forward.x + (infoSG[myIndex].tf_bulletClone[3].right.x * 0.1f), 0, infoSG[myIndex].tf_bulletClone[3].forward.z + (infoSG[myIndex].tf_bulletClone[3].right.z * 0.1f));
        infoSG[myIndex].vt_bulletDir[4] = new Vector3(infoSG[myIndex].tf_bulletClone[4].forward.x + (infoSG[myIndex].tf_bulletClone[4].right.x * 0.2f), 0, infoSG[myIndex].tf_bulletClone[4].forward.z + (infoSG[myIndex].tf_bulletClone[4].right.z * 0.2f));
        /*
        infoSG[myIndex].vt_bulletDir[0].x = new Vector3(PlayersManager.instance.direction2[myIndex].x - 20.0f;
        infoSG[myIndex].vt_bulletDir[0].z = PlayersManager.instance.direction2[myIndex].y - 20.0f;

        infoSG[myIndex].vt_bulletDir[1].x = PlayersManager.instance.direction2[myIndex].x - 10.0f;
        infoSG[myIndex].vt_bulletDir[1].z = PlayersManager.instance.direction2[myIndex].y - 10.0f;

        infoSG[myIndex].vt_bulletDir[2].x = PlayersManager.instance.direction2[myIndex].x;
        infoSG[myIndex].vt_bulletDir[2].z = PlayersManager.instance.direction2[myIndex].y;

        infoSG[myIndex].vt_bulletDir[3].x = PlayersManager.instance.direction2[myIndex].x + 10.0f;
        infoSG[myIndex].vt_bulletDir[3].z = PlayersManager.instance.direction2[myIndex].y + 10.0f;

        infoSG[myIndex].vt_bulletDir[4].x = PlayersManager.instance.direction2[myIndex].x + 20.0f;
        infoSG[myIndex].vt_bulletDir[4].z = PlayersManager.instance.direction2[myIndex].y + 20.0f;
        */

        /*
        Vector3 tmp0 = (RightJoystick.instance.right_joystick.touchPos - (RightJoystick.instance.right_joystick.stickFirstPos - new Vector3(20.0f, 20.0f, 0))).normalized;
        Vector3 tmp1 = (RightJoystick.instance.right_joystick.touchPos - (RightJoystick.instance.right_joystick.stickFirstPos - new Vector3(10.0f, 10.0f, 0))).normalized;
        Vector3 tmp2 = (RightJoystick.instance.right_joystick.touchPos - RightJoystick.instance.right_joystick.stickFirstPos).normalized;
        Vector3 tmp3 = (RightJoystick.instance.right_joystick.touchPos - (RightJoystick.instance.right_joystick.stickFirstPos + new Vector3(10.0f, 10.0f, 0))).normalized;
        Vector3 tmp4 = (RightJoystick.instance.right_joystick.touchPos - (RightJoystick.instance.right_joystick.stickFirstPos + new Vector3(20.0f, 20.0f, 0))).normalized;

        infoSG[myIndex].vt_bulletDir[0].x = tmp0.x;   infoSG[myIndex].vt_bulletDir[1].x = tmp1.x;
        infoSG[myIndex].vt_bulletDir[0].z = tmp0.y;   infoSG[myIndex].vt_bulletDir[1].z = tmp1.y;
        infoSG[myIndex].vt_bulletDir[2].x = tmp2.x;   infoSG[myIndex].vt_bulletDir[3].x = tmp3.x;
        infoSG[myIndex].vt_bulletDir[2].z = tmp2.y;   infoSG[myIndex].vt_bulletDir[3].z = tmp3.y;
        infoSG[myIndex].vt_bulletDir[4].x = tmp4.x;
        infoSG[myIndex].vt_bulletDir[4].z = tmp4.y; */

        for (int i = 0; i < infoSG[myIndex].obj_bulletClone.Length; i++)
        {
            infoSG[myIndex].tf_bulletClone[i].localRotation = Quaternion.LookRotation(infoSG[myIndex].vt_bulletDir[i]);
            infoSG[myIndex].obj_bulletClone[i].GetComponent<Rigidbody>().AddForce(infoSG[myIndex].vt_bulletDir[i] * 2000.0f, ForceMode.Acceleration);
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















/*
 * float time = 0;
 *
             time += Time.smoothDeltaTime;

            if (time >= infoAR[myIndex].fireRate)
            {
                  

                time = 0;
            }
 */
