using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum _EFFECT_TYPE
{
    MUZZLE = 0,
    SPARK,
}

public class EffectManager : MonoBehaviour
{ 
    public static EffectManager instance;
    public int myIndex { get; set; }

    #region PARTICLE_MUZZLE
    public ParticleSystem[] ps_muzzlePrefebsList;
   
    public struct _PS_TmpMuzzle_Body
    {
        public ParticleSystem.MainModule option;
        public ParticleSystem body;
    }

    public struct _PS_TmpMuzzle_Glow
    {
        public ParticleSystem.MainModule option;
        public ParticleSystem body;
    }

    public struct _PS_TmpMuzzle_Plane2
    {
        public ParticleSystem.MainModule option;
        public ParticleSystem body;
    }

    public struct _PS_TmpMuzzle_Plane3
    {
        public ParticleSystem.MainModule option;
        public ParticleSystem body;
    }

    public struct _PS_TmpMuzzle_Plane4
    {
        public ParticleSystem.MainModule option;
        public ParticleSystem body;
    }

    public struct _PS_TmpMuzzle_Spark
    {
        public ParticleSystem.MainModule option;
        public ParticleSystem body;
    }

    public struct _PS_TmpMuzzle
    {
        public _PS_TmpMuzzle_Body body;
        public _PS_TmpMuzzle_Glow glow;
        public _PS_TmpMuzzle_Plane2 plane2;
        public _PS_TmpMuzzle_Plane3 plane3;
        public _PS_TmpMuzzle_Plane4 plane4;
        public _PS_TmpMuzzle_Spark spark;
    }

    public _PS_TmpMuzzle[] ps_tmpMuzzle { get; set; }
    #endregion

    public ParticleSystem[] ps_sparkPrefebsList;
    public ParticleSystem ps_tmpSpark { get; set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        myIndex = GameManager.instance.myIndex;

        ps_tmpMuzzle = new _PS_TmpMuzzle[C_Global.MAX_PLAYER];
        //ps_tmpSpark = new ParticleSystem[C_Global.MAX_PLAYER];

#if !NETWORK
        InitializeMuzzle2(C_Global.MAX_PLAYER);
#endif
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            PlayEffect(_EFFECT_TYPE.MUZZLE, myIndex);
        }
        else if(Input.GetKeyDown(KeyCode.O))
        {
            StopEffect(_EFFECT_TYPE.MUZZLE, myIndex);
        }
    }
    public void PlayEffect(_EFFECT_TYPE _value, int _index)
    {
        switch(_value)
        {
            case _EFFECT_TYPE.MUZZLE:
                ps_tmpMuzzle[_index].body.body.Stop();
                ps_tmpMuzzle[_index].body.body.Clear();
                ps_tmpMuzzle[_index].body.body.Play();
                break;
            case _EFFECT_TYPE.SPARK:
                break;
        }
    }

    public void StopEffect(_EFFECT_TYPE _value, int _index)
    {
        switch (_value)
        {
            case _EFFECT_TYPE.MUZZLE:
                ps_tmpMuzzle[_index].body.body.Stop();
                ps_tmpMuzzle[_index].body.body.Clear();
                break;
            case _EFFECT_TYPE.SPARK:
                break;
        }
    }

    public void InitializeMuzzle(int _index)
    {
        if (ps_tmpMuzzle[_index].body.body != null)
            Destroy(ps_tmpMuzzle[_index].body.body.gameObject);

        switch (WeaponManager.instance.mainWeapon[_index])
        {
            case _WEAPONS.AR:
                ps_tmpMuzzle[_index].body.body = Instantiate(ps_muzzlePrefebsList[1], PlayersManager.instance.obj_players[_index].transform) as ParticleSystem;
                break;
            case _WEAPONS.SG:
                ps_tmpMuzzle[_index].body.body = Instantiate(ps_muzzlePrefebsList[0], PlayersManager.instance.obj_players[_index].transform) as ParticleSystem;
                break;
            case _WEAPONS.SMG:
                ps_tmpMuzzle[_index].body.body = Instantiate(ps_muzzlePrefebsList[2], PlayersManager.instance.obj_players[_index].transform) as ParticleSystem;
                break;
        }

        ps_tmpMuzzle[_index].glow.body = ps_tmpMuzzle[_index].body.body.transform.GetChild(0).GetComponent<ParticleSystem>();
        ps_tmpMuzzle[_index].plane2.body = ps_tmpMuzzle[_index].body.body.transform.GetChild(1).GetComponent<ParticleSystem>();
        ps_tmpMuzzle[_index].plane3.body = ps_tmpMuzzle[_index].body.body.transform.GetChild(2).GetComponent<ParticleSystem>();
        ps_tmpMuzzle[_index].plane4.body = ps_tmpMuzzle[_index].body.body.transform.GetChild(3).GetComponent<ParticleSystem>();
        ps_tmpMuzzle[_index].spark.body = ps_tmpMuzzle[_index].body.body.transform.GetChild(4).GetComponent<ParticleSystem>();

        ps_tmpMuzzle[_index].body.option = ps_tmpMuzzle[_index].body.body.main;
        ps_tmpMuzzle[_index].glow.option = ps_tmpMuzzle[_index].glow.body.main;
        ps_tmpMuzzle[_index].plane2.option = ps_tmpMuzzle[_index].plane2.body.main;
        ps_tmpMuzzle[_index].plane3.option = ps_tmpMuzzle[_index].plane3.body.main;
        ps_tmpMuzzle[_index].plane4.option = ps_tmpMuzzle[_index].plane4.body.main;
        ps_tmpMuzzle[_index].spark.option = ps_tmpMuzzle[_index].spark.body.main;
    }


    public void InitializeMuzzle2(float _num)
    {
        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            if (ps_tmpMuzzle[i].body.body != null)
                Destroy(ps_tmpMuzzle[i].body.body.gameObject);

            switch (WeaponManager.instance.mainWeapon[i])
            {
                case _WEAPONS.AR:
                    ps_tmpMuzzle[i].body.body = Instantiate(ps_muzzlePrefebsList[1], PlayersManager.instance.obj_players[i].transform) as ParticleSystem;
                    break;
                case _WEAPONS.SG:
                    ps_tmpMuzzle[i].body.body = Instantiate(ps_muzzlePrefebsList[0], PlayersManager.instance.obj_players[i].transform) as ParticleSystem;
                    break;
                case _WEAPONS.SMG:
                    ps_tmpMuzzle[i].body.body = Instantiate(ps_muzzlePrefebsList[2], PlayersManager.instance.obj_players[i].transform) as ParticleSystem;
                    break;
            }

            ps_tmpMuzzle[i].glow.body = ps_tmpMuzzle[i].body.body.transform.GetChild(0).GetComponent<ParticleSystem>();
            ps_tmpMuzzle[i].plane2.body = ps_tmpMuzzle[i].body.body.transform.GetChild(1).GetComponent<ParticleSystem>();
            ps_tmpMuzzle[i].plane3.body = ps_tmpMuzzle[i].body.body.transform.GetChild(2).GetComponent<ParticleSystem>();
            ps_tmpMuzzle[i].plane4.body = ps_tmpMuzzle[i].body.body.transform.GetChild(3).GetComponent<ParticleSystem>();
            ps_tmpMuzzle[i].spark.body = ps_tmpMuzzle[i].body.body.transform.GetChild(4).GetComponent<ParticleSystem>();
            //ps_tmpSpark[i] = Instantiate(ps_spark, GameObject.FindGameObjectWithTag("Effects").transform);

            ps_tmpMuzzle[i].body.option = ps_tmpMuzzle[i].body.body.main;
            ps_tmpMuzzle[i].glow.option = ps_tmpMuzzle[i].glow.body.main;
            ps_tmpMuzzle[i].plane2.option = ps_tmpMuzzle[i].plane2.body.main;
            ps_tmpMuzzle[i].plane3.option = ps_tmpMuzzle[i].plane3.body.main;
            ps_tmpMuzzle[i].plane4.option = ps_tmpMuzzle[i].plane4.body.main;
            ps_tmpMuzzle[i].spark.option = ps_tmpMuzzle[i].spark.body.main;
        }
    }

    public IEnumerator CheckEffectEnd(ParticleSystem _ps_effectClone)
    {
        yield return YieldInstructionCache.WaitForSeconds(0.5f);

        PoolManager.instance.ReturnCollisionEffectToPool(_ps_effectClone);
    }

}
