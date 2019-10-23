using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Shot : MonoBehaviour, IActionState
{
    private static State_Shot instance;
    private int myIndex { get; set; }

    private void Start()
    {
        myIndex = GameManager.instance.myIndex;
    }

    public static State_Shot GetStateInstance()
    {
        if (instance == null)
            instance = (State_Shot)StateManager.instance.cn_stateList[5]; // StateManager.instance.obj_stateList.GetComponent<State_Shot>();

        return instance;
    }

    public void Death(bool _value)
    {
        if (_value == true)
        {
            StateManager.instance.SetState(State_Death.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_Death());

            if (WeaponManager.instance.curMainActionCor[myIndex] != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curMainActionCor[myIndex]);
            EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, myIndex);
        }
    }

    public void Idle(bool _value)
    {
        if (_value == true)
        {
            StateManager.instance.SetState(State_Idle.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_Idle());

            if (WeaponManager.instance.curMainActionCor[myIndex] != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curMainActionCor[myIndex]);
            EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, myIndex);
        }
    }

    public void Circuit(bool _value)
    {
        if (_value == true)
        {
            StateManager.instance.SetState(State_AimCircuitShot.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_AimCircuit());

            if (WeaponManager.instance.curMainActionCor[myIndex] != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curMainActionCor[myIndex]);
            WeaponManager.instance.curMainActionCor[myIndex] = WeaponManager.instance.StartCoroutine(WeaponManager.instance.ActionFire(myIndex));
            EffectManager.instance.PlayEffect(_EFFECT_TYPE.MUZZLE, myIndex);
        }
    }

    public void Aim(bool _value)
    {
        if (_value == false)
        {
            StateManager.instance.SetState(State_Idle.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_Idle());
          
            if (WeaponManager.instance.curMainActionCor[myIndex] != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curMainActionCor[myIndex]);
            EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, myIndex);
        }
    }

    public void Shot(bool _value)
    {
        if (_value == false)
        {
            StateManager.instance.SetState(State_Aim.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_Aim());

            if (WeaponManager.instance.curMainActionCor[myIndex] != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curMainActionCor[myIndex]);
            EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, myIndex);
        }
    }
}
