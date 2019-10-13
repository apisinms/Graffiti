using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_SMG : MonoBehaviour, IMainWeaponType
{
    public static Main_SMG instance;
    private int myIndex { get; set; }

    #region SMG
    public struct _INFO_SMG
    {
        public Vector3[] vt_bulletPattern;
        public int bulletPatternIndex;
        public int prevBulletPatternIndex;

        public int curAmmo;
        public int maxAmmo;
        public float fireRate;
        public float damage;
        public float accuracy;
        public float range;
        public float speed;
    }
    public _INFO_SMG[] infoSMG { get; set; }
    #endregion    

    private void Start()
    {
        myIndex = GameManager.instance.myIndex;

        infoSMG = new _INFO_SMG[C_Global.MAX_PLAYER];

        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            infoSMG[i].vt_bulletPattern = new Vector3[3];
            infoSMG[i].bulletPatternIndex = 2;
            infoSMG[i].prevBulletPatternIndex = 1;
            infoSMG[i].curAmmo = 25;
            infoSMG[i].maxAmmo = 25;
            infoSMG[i].fireRate = 0.07f;
            infoSMG[i].damage = 0.5f;
            infoSMG[i].accuracy = 0.1f;
            infoSMG[i].range = 15.0f;
            infoSMG[i].speed = 1200.0f;
        }
    }

    public static Main_SMG GetMainWeaponInstance()
    {
        if (instance == null)
            instance = (Main_SMG)WeaponManager.instance.cn_mainWeaponList[3]; //StateManager.instance.obj_stateList.GetComponent<State_Circuit>();

        return instance;
    }

    public IEnumerator ActionFire(int _index)
    {
        while (true)
        {
            var clone = WeaponManager.instance.GetBulletFromPool(_index);
            Transform tf_clone = clone.transform;

            infoSMG[_index].vt_bulletPattern[0].x = tf_clone.forward.x - (tf_clone.right.x * infoSMG[_index].accuracy);
            infoSMG[_index].vt_bulletPattern[0].z = tf_clone.forward.z - (tf_clone.right.z * infoSMG[_index].accuracy);
            infoSMG[_index].vt_bulletPattern[1].x = tf_clone.forward.x;
            infoSMG[_index].vt_bulletPattern[1].z = tf_clone.forward.z;
            infoSMG[_index].vt_bulletPattern[2].x = tf_clone.forward.x + (tf_clone.right.x * infoSMG[_index].accuracy);
            infoSMG[_index].vt_bulletPattern[2].z = tf_clone.forward.z + (tf_clone.right.z * infoSMG[_index].accuracy);

            EffectManager.instance.ps_tmpMuzzle[_index].body.option.loop = true;
            EffectManager.instance.ps_tmpMuzzle[_index].glow.option.loop = true;
            EffectManager.instance.ps_tmpMuzzle[_index].plane2.option.loop = true;
            EffectManager.instance.ps_tmpMuzzle[_index].plane3.option.loop = true;
            EffectManager.instance.ps_tmpMuzzle[_index].plane4.option.loop = true;
            EffectManager.instance.ps_tmpMuzzle[_index].spark.option.loop = true;

            EffectManager.instance.ps_tmpMuzzle[_index].body.option.duration = 1.0f;
            EffectManager.instance.ps_tmpMuzzle[_index].glow.option.duration = 1.0f;
            EffectManager.instance.ps_tmpMuzzle[_index].plane2.option.duration = 1.0f;
            EffectManager.instance.ps_tmpMuzzle[_index].plane3.option.duration = 1.0f;
            EffectManager.instance.ps_tmpMuzzle[_index].plane4.option.duration = 1.0f;
            EffectManager.instance.ps_tmpMuzzle[_index].spark.option.duration = 1.0f;

            EffectManager.instance.ps_tmpMuzzle[_index].body.option.simulationSpeed = 1.5f;
            EffectManager.instance.ps_tmpMuzzle[_index].glow.option.simulationSpeed = 1.5f;
            EffectManager.instance.ps_tmpMuzzle[_index].plane2.option.simulationSpeed = 1.5f;
            EffectManager.instance.ps_tmpMuzzle[_index].plane3.option.simulationSpeed = 1.5f;
            EffectManager.instance.ps_tmpMuzzle[_index].plane4.option.simulationSpeed = 1.5f;
            EffectManager.instance.ps_tmpMuzzle[_index].spark.option.simulationSpeed = 1.5f;

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
            yield return YieldInstructionCache.WaitForSeconds(infoSMG[_index].fireRate);
        }
    }

    public void CheckFireRange(GameObject _obj_bullet, int _index)
    {
        if (Vector3.Distance(_obj_bullet.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= Main_SMG.instance.infoSMG[_index].range)
            WeaponManager.instance.ReturnBulletToPool(_obj_bullet, _index);
    }
}
