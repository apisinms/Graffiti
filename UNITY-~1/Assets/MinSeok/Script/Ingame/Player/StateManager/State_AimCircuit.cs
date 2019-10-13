using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_AimCircuit : MonoBehaviour, IActionState
{
    private static State_AimCircuit instance;
    private int myIndex { get; set; }

    private void Start()
    {
        myIndex = GameManager.instance.myIndex;
    }

    public static State_AimCircuit GetStateInstance()
    {
        if (instance == null)
            instance = (State_AimCircuit)StateManager.instance.cn_stateList[5];  // StateManager.instance.obj_stateList.GetComponent<State_AimCircuit>();

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
        if (_value == false)
        {
            StateManager.instance.SetState(State_Aim.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.ActionAim());
        }
    }

    public void Aim(bool _value)
    { 
        if (_value == false)
        {
            StateManager.instance.SetState(State_Circuit.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.ActionCircuit());
        }
    }

    public void Shot(bool _value)
    {
        if (_value == true)
        {
            StateManager.instance.SetState(State_AimCircuitShot.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.ActionAimCircuit());

            if (WeaponManager.instance.curMainActionCor[myIndex] != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curMainActionCor[myIndex]);
            WeaponManager.instance.curMainActionCor[myIndex] = WeaponManager.instance.StartCoroutine(WeaponManager.instance.ActionFire(myIndex));
            EffectManager.instance.PlayEffect(_EFFECT_TYPE.MUZZLE, myIndex);
        }
    }
}
