using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Circuit : MonoBehaviour, IActionState
{
    private static State_Circuit instance;

    public static State_Circuit GetStateInstance()
    {
        if (instance == null)
            instance = (State_Circuit)StateManager.instance.cn_stateList[2]; //StateManager.instance.obj_stateList.GetComponent<State_Circuit>();

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
            StateManager.instance.SetState(State_Idle.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.cor_ActionIdle);
        }
    }

    public void Aim(bool _value)
    {
        if (_value == true)
        {
            StateManager.instance.SetState(State_AimCircuit.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.cor_ActionAimCurcuit);
        }
    }

    public void Shot(bool _value) { }
}
