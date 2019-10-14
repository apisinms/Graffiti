using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_SG : MonoBehaviour, IMainWeaponType
{
    public static Main_SG instance;
    private int myIndex { get; set; }

    #region SG
    public struct _INFO_SG
    {
        public GameObject[] obj_bulletClone;
        public Transform[] tf_bulletClone;
        public Vector3[,] vt_bulletPattern;
        public int bulletPatternIndex;

        public int curAmmo;
        public int maxAmmo;
        public float fireRate;
        public float damage;
        public float accuracy;
        public float range;
        public float speed;
    }
    public _INFO_SG[] infoSG { get; set; }
    #endregion

    private void Start()
    {
        myIndex = GameManager.instance.myIndex;

        infoSG = new _INFO_SG[C_Global.MAX_PLAYER];

        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            infoSG[i].obj_bulletClone = new GameObject[5];
            infoSG[i].tf_bulletClone = new Transform[5];
            infoSG[i].vt_bulletPattern = new Vector3[2, 5];
            infoSG[i].bulletPatternIndex = 0;
            infoSG[i].curAmmo = 2;
            infoSG[i].maxAmmo = 2;
            infoSG[i].fireRate = 0;
            infoSG[i].damage = 1.5f;
            infoSG[i].accuracy = 0.25f;
            infoSG[i].range = 10.0f;
            infoSG[i].speed = 2300.0f;
        }
    }

    public static Main_SG GetMainWeaponInstance()
    {
        if (instance == null)
            instance = (Main_SG)WeaponManager.instance.cn_mainWeaponList[2]; //StateManager.instance.obj_stateList.GetComponent<State_Circuit>();

        return instance;
    }

    public IEnumerator ActionFire(int _index)
    {
        for (int i = 0; i < 5; i++)
        {
            infoSG[_index].obj_bulletClone[i] = PoolManager.instance.GetBulletFromPool(_index);
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

        EffectManager.instance.ps_tmpMuzzle[_index].body.option.loop = false;
        EffectManager.instance.ps_tmpMuzzle[_index].glow.option.loop = false;
        EffectManager.instance.ps_tmpMuzzle[_index].plane2.option.loop = false;
        EffectManager.instance.ps_tmpMuzzle[_index].plane3.option.loop = false;
        EffectManager.instance.ps_tmpMuzzle[_index].plane4.option.loop = false;
        EffectManager.instance.ps_tmpMuzzle[_index].spark.option.loop = false;

        EffectManager.instance.ps_tmpMuzzle[_index].body.option.duration = 0.15f;
        EffectManager.instance.ps_tmpMuzzle[_index].glow.option.duration = 0.15f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane2.option.duration = 0.15f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane3.option.duration = 0.15f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane4.option.duration = 0.15f;
        EffectManager.instance.ps_tmpMuzzle[_index].spark.option.duration = 0.15f;

        EffectManager.instance.ps_tmpMuzzle[_index].body.option.simulationSpeed = 1.7f;
        EffectManager.instance.ps_tmpMuzzle[_index].glow.option.simulationSpeed = 1.7f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane2.option.simulationSpeed = 1.7f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane3.option.simulationSpeed = 1.7f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane4.option.simulationSpeed = 1.7f;
        EffectManager.instance.ps_tmpMuzzle[_index].spark.option.simulationSpeed = 1.7f;

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

        yield break;
    }

    public void CheckFireRange(GameObject _obj_bullet, int _index)
    {
        if (Vector3.Distance(_obj_bullet.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= Main_SG.instance.infoSG[_index].range)
            PoolManager.instance.ReturnBulletToPool(_obj_bullet, _index);
    }
}
