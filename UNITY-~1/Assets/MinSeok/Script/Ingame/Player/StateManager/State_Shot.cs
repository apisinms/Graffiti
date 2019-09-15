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
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.cor_ActionIdle);
        }
    }

    public void Circuit(bool _value)
    {
        if (_value == true)
        {
            StateManager.instance.SetState(State_AimCircuitShot.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.cor_ActionAimCurcuit);

            if (WeaponManager.instance.curCor != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curCor);
            WeaponManager.instance.curCor = WeaponManager.instance.StartCoroutine(WeaponManager.instance.cor_ActionBullet);
        }
    }

    public void Aim(bool _value)
    {
        if (_value == false)
        {
            StateManager.instance.SetState(State_Idle.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.cor_ActionIdle);
          
            if (WeaponManager.instance.curCor != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curCor);
        }
    }

    public void Shot(bool _value)
    {
        if (_value == false)
        {
            StateManager.instance.SetState(State_Aim.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.cor_ActionAim);

            if (WeaponManager.instance.curCor != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curCor);
        }
    }
}
