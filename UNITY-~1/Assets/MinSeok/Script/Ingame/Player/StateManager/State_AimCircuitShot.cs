using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_AimCircuitShot : MonoBehaviour, IActionState
{
    private static State_AimCircuitShot instance;

    public static State_AimCircuitShot GetStateInstance()
    {
        if (instance == null)
            instance = StateManager.instance.obj_stateList.GetComponent<State_AimCircuitShot>();

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

            Debug.Log("그냥샷2");
            //PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.cor_ActionAimCurcuit);
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
        }
    }
}
