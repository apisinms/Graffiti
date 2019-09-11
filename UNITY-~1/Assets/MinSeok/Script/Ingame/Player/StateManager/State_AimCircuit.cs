using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_AimCircuit : MonoBehaviour, IActionState
{
    private static State_AimCircuit instance;

    public static State_AimCircuit GetStateInstance()
    {
        if (instance == null)
            instance = StateManager.instance.obj_stateList.GetComponent<State_AimCircuit>();

        return instance;
    }

    public void Idle(bool _value)
    {
        if(_value == true)
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
            StateManager.instance.SetState(State_Aim.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.cor_ActionAim);
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
        if (_value == true)
        {
            StateManager.instance.SetState(State_AimCircuitShot.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            //PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.);

            Debug.Log("걸으면서 조준샷1"); 
        }
    }
}
