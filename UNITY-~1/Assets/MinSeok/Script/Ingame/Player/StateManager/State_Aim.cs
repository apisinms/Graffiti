using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Aim : MonoBehaviour, IActionState
{
    private static State_Aim instance;
    private int myIndex { get; set; }

    private void Start()
    {
        myIndex = GameManager.instance.myIndex;
    }

    public static State_Aim GetStateInstance()
    {
        if (instance == null)
            instance = (State_Aim)StateManager.instance.cn_stateList[3];  //StateManager.instance.obj_stateList.GetComponent<State_Aim>();

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
            StateManager.instance.SetState(State_AimCircuit.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.ActionAimCircuit());
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
        }
    }

    public void Shot(bool _value)
    {
        if (_value == true)
        {
            StateManager.instance.SetState(State_Shot.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.ActionAim());

            if (WeaponManager.instance.curMainActionCor[myIndex] != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curMainActionCor[myIndex]);
            WeaponManager.instance.curMainActionCor[myIndex] = WeaponManager.instance.StartCoroutine(WeaponManager.instance.ActionFire(myIndex));
            EffectManager.instance.PlayEffect(_EFFECT_TYPE.MUZZLE, myIndex);
        }
    }
}
