using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_AR : MonoBehaviour, IMainWeaponType
{
    public static Main_AR instance;
    private int myIndex { get; set; }

    #region AR
    public struct _INFO_AR
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
    public _INFO_AR[] infoAR { get; set; }
    #endregion

    private void Start()
    {
        myIndex = GameManager.instance.myIndex;

        infoAR = new _INFO_AR[C_Global.MAX_PLAYER];

        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            infoAR[i].vt_bulletPattern = new Vector3[3];
            infoAR[i].bulletPatternIndex = 1;
            infoAR[i].prevBulletPatternIndex = 2;
            infoAR[i].curAmmo = 30;
            infoAR[i].maxAmmo = 30;
            infoAR[i].fireRate = 0.14f;
            infoAR[i].damage = 1.0f;
            infoAR[i].accuracy = 0.06f;
            infoAR[i].range = 20.0f;
            infoAR[i].speed = 2000.0f;
        }
    }

    public static Main_AR GetMainWeaponInstance()
    {
        if (instance == null)
            instance = (Main_AR)WeaponManager.instance.cn_mainWeaponList[1]; //StateManager.instance.obj_stateList.GetComponent<State_Circuit>();

        return instance;
    }

    public IEnumerator ActionFire(int _index)
    {
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

        EffectManager.instance.ps_tmpMuzzle[_index].body.option.simulationSpeed = 0.8f;
        EffectManager.instance.ps_tmpMuzzle[_index].glow.option.simulationSpeed = 0.8f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane2.option.simulationSpeed = 0.8f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane3.option.simulationSpeed = 0.8f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane4.option.simulationSpeed = 0.8f;
        EffectManager.instance.ps_tmpMuzzle[_index].spark.option.simulationSpeed = 0.8f;

        while (true)
        {          
            var clone = PoolManager.instance.GetBulletFromPool(_index);
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

            yield return YieldInstructionCache.WaitForSeconds(infoAR[_index].fireRate);
            
        }
    }

    public void CheckFireRange(GameObject _obj_bullet, int _index)
    {
        if (Vector3.Distance(_obj_bullet.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= Main_AR.instance.infoAR[_index].range)
            PoolManager.instance.ReturnBulletToPool(_obj_bullet, _index);
    }
}
