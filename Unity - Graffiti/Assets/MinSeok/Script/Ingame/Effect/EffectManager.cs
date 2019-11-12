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

    public struct _PS_TmpMuzzle_Spike
    {
        public ParticleSystem.MainModule option;
        public ParticleSystem body;
    }

    public struct _PS_TmpMuzzle_Flare
    {
        public ParticleSystem.MainModule option;
        public ParticleSystem body;
    }

    public struct _PS_TmpMuzzle
    {
        public _PS_TmpMuzzle_Body body;
        public _PS_TmpMuzzle_Glow glow;
        public _PS_TmpMuzzle_Spike spike;
        public _PS_TmpMuzzle_Flare flare;
    }

    public _PS_TmpMuzzle[] ps_tmpMuzzle { get; set; }
    #endregion

    public ParticleSystem[] ps_sparkPrefebsList;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        myIndex = GameManager.instance.myIndex;

        ps_tmpMuzzle = new _PS_TmpMuzzle[GameManager.instance.gameInfo.maxPlayer];

#if !NETWORK
        InitializeMuzzle2(C_Global.MAX_CHARACTER);
#endif
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
        }
    }

    public void InitializeMuzzle(int _index)
    {
        if (ps_tmpMuzzle[_index].body.body != null)
            Destroy(ps_tmpMuzzle[_index].body.body.gameObject);

        switch (WeaponManager.instance.mainWeapon[_index])
        {
            case _WEAPONS.AR:
                ps_tmpMuzzle[_index].body.body = Instantiate(ps_muzzlePrefebsList[0], PlayersManager.instance.tf_players[_index].transform) as ParticleSystem;
                break;
            case _WEAPONS.SG:
                ps_tmpMuzzle[_index].body.body = Instantiate(ps_muzzlePrefebsList[1], PlayersManager.instance.tf_players[_index].transform) as ParticleSystem;
                break;
            case _WEAPONS.SMG:
                ps_tmpMuzzle[_index].body.body = Instantiate(ps_muzzlePrefebsList[2], PlayersManager.instance.tf_players[_index].transform) as ParticleSystem;
                break;
        }

        ps_tmpMuzzle[_index].glow.body = ps_tmpMuzzle[_index].body.body.transform.GetChild(0).GetComponent<ParticleSystem>();
        ps_tmpMuzzle[_index].spike.body = ps_tmpMuzzle[_index].body.body.transform.GetChild(1).GetComponent<ParticleSystem>();
        ps_tmpMuzzle[_index].flare.body = ps_tmpMuzzle[_index].body.body.transform.GetChild(2).GetComponent<ParticleSystem>();

        ps_tmpMuzzle[_index].body.option = ps_tmpMuzzle[_index].body.body.main;
        ps_tmpMuzzle[_index].glow.option = ps_tmpMuzzle[_index].glow.body.main;
        ps_tmpMuzzle[_index].spike.option = ps_tmpMuzzle[_index].spike.body.main;
        ps_tmpMuzzle[_index].flare.option = ps_tmpMuzzle[_index].flare.body.main;
    }


    public void InitializeMuzzle2(float _num)
    {
        for (int i = 0; i < C_Global.MAX_CHARACTER; i++)
        {
            if (ps_tmpMuzzle[i].body.body != null)
                Destroy(ps_tmpMuzzle[i].body.body.gameObject);

            switch (WeaponManager.instance.mainWeapon[i])
            {
                case _WEAPONS.AR:
                    ps_tmpMuzzle[i].body.body = Instantiate(ps_muzzlePrefebsList[0], PlayersManager.instance.tf_players[i].transform) as ParticleSystem;
                    break;
                case _WEAPONS.SG:
                    ps_tmpMuzzle[i].body.body = Instantiate(ps_muzzlePrefebsList[1], PlayersManager.instance.tf_players[i].transform) as ParticleSystem;
                    break;
                case _WEAPONS.SMG:
                    ps_tmpMuzzle[i].body.body = Instantiate(ps_muzzlePrefebsList[2], PlayersManager.instance.tf_players[i].transform) as ParticleSystem;
                    break;
            }

            ps_tmpMuzzle[i].glow.body = ps_tmpMuzzle[i].body.body.transform.GetChild(0).GetComponent<ParticleSystem>();
            ps_tmpMuzzle[i].spike.body = ps_tmpMuzzle[i].body.body.transform.GetChild(1).GetComponent<ParticleSystem>();
            ps_tmpMuzzle[i].flare.body = ps_tmpMuzzle[i].body.body.transform.GetChild(2).GetComponent<ParticleSystem>();
                               
            ps_tmpMuzzle[i].body.option = ps_tmpMuzzle[i].body.body.main;
            ps_tmpMuzzle[i].glow.option = ps_tmpMuzzle[i].glow.body.main;
            ps_tmpMuzzle[i].spike.option = ps_tmpMuzzle[i].spike.body.main;
            ps_tmpMuzzle[i].flare.option = ps_tmpMuzzle[i].flare.body.main;
        }
    }

    public IEnumerator CheckEffectEnd(ParticleSystem _ps_effectClone)
    {
        yield return YieldInstructionCache.WaitForSeconds(0.5f);

        PoolManager.instance.ReturnCollisionEffectToPool(_ps_effectClone);
    }

}
