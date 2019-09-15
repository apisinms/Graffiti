using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_AimCircuitShot : MonoBehaviour, IActionState
{
    private static State_AimCircuitShot instance;

    public static State_AimCircuitShot GetStateInstance()
    {
        if (instance == null)
            instance = (State_AimCircuitShot)StateManager.instance.cn_stateList[6];  //StateManager.instance.obj_stateList.GetComponent<State_AimCircuitShot>();

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
        if (_value == false)
        {
            StateManager.instance.SetState(State_Shot.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.cor_ActionAim);

            if (WeaponManager.instance.curCor != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curCor);
            WeaponManager.instance.curCor = WeaponManager.instance.StartCoroutine(WeaponManager.instance.cor_ActionBullet);
        }
    }

    public void Aim(bool _value)
    {
        if (_value == false)
        {
            StateManager.instance.SetState(State_Circuit.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.cor_ActionCircuit);

            if (WeaponManager.instance.curCor != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curCor);
        }
    }

    public void Shot(bool _value)
    {
        if (_value == false)
        {
            StateManager.instance.SetState(State_AimCircuit.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.cor_ActionAimCurcuit);

            if (WeaponManager.instance.curCor != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curCor);
        }
    }
}
