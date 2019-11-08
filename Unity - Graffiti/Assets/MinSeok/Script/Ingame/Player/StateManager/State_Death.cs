using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Death : MonoBehaviour, IActionState
{
    private static State_Death instance;
    private int myIndex { get; set; }

    private void Start()
    {
        myIndex = GameManager.instance.myIndex;
    }

    public static State_Death GetStateInstance()
    {
        if (instance == null)
            instance = (State_Death)StateManager.instance.cn_stateList[2]; //StateManager.instance.obj_stateList.GetComponent<State_Circuit>();

        return instance;
    }

    public void Death(bool _value)
    {
        /*
        if (_value == false)
        {
            StateManager.instance.SetState(State_Idle.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_Idle());
        }
        */
    }

    public void Idle(bool _value)
    {
        if (_value == true)
        {
            StateManager.instance.SetState(State_Idle.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_Idle());
        }
    }

    public void Circuit(bool _value) { }
    public void Aim(bool _value) { }
    public void Shot(bool _value) { }
    public void Spray(bool _value, int _triggerIdx) { }
}
