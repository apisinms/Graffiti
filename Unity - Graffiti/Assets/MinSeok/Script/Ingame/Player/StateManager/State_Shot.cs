using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Shot : MonoBehaviour, IActionState
{
    private static State_Shot instance;

    public static State_Shot GetStateInstance()
    {
        if (instance == null)
            instance = (State_Shot)StateManager.instance.cn_stateList[4]; // StateManager.instance.obj_stateList.GetComponent<State_Shot>();

        return instance;
    }

    public void Idle(bool _value)
    {
        if (_value == true)
        {
            StateManager.instance.SetState(State_Idle.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.ActionIdle());
        }
    }

    public void Circuit(bool _value)
    {
        if (_value == true)
        {
            StateManager.instance.SetState(State_AimCircuitShot.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.ActionAimCircuit());

            if (WeaponManager.instance.curActionCor != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curActionCor);
            WeaponManager.instance.curActionCor = WeaponManager.instance.StartCoroutine(WeaponManager.instance.ActionBullet());

            //BulletCollision.instance.curCheckRangeCor = BulletCollision.instance.StartCoroutine(BulletCollision.instance.CheckBulletRange());
        }
    }

    public void Aim(bool _value)
    {
        if (_value == false)
        {
            StateManager.instance.SetState(State_Idle.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.ActionIdle());
          
            if (WeaponManager.instance.curActionCor != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curActionCor);
            /*
            if (BulletCollision.instance.curCheckRangeCor != null)
                BulletCollision.instance.StopCoroutine(BulletCollision.instance.curCheckRangeCor); */
        }
    }

    public void Shot(bool _value)
    {
        if (_value == false)
        {
            StateManager.instance.SetState(State_Aim.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.ActionAim());

            if (WeaponManager.instance.curActionCor != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curActionCor);
            /*
            if (BulletCollision.instance.curCheckRangeCor != null)
                BulletCollision.instance.StopCoroutine(BulletCollision.instance.curCheckRangeCor); */
        }
    }
}
