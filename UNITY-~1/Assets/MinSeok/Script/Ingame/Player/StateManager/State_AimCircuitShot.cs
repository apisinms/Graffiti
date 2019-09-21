using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_AimCircuitShot : MonoBehaviour, IActionState
{
    private static State_AimCircuitShot instance;
    private int myIndex { get; set; }

    private void Start()
    {
        myIndex = GameManager.instance.myIndex;
    }

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
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.ActionIdle());
        }
    }

    public void Circuit(bool _value)
    {
        if (_value == false)
        {
            StateManager.instance.SetState(State_Shot.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.ActionAim());

            if (WeaponManager.instance.curActionCor[myIndex] != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curActionCor[myIndex]);
            WeaponManager.instance.curActionCor[myIndex] = WeaponManager.instance.StartCoroutine(WeaponManager.instance.ActionBullet());

           // BulletCollision.instance.curCheckRangeCor = BulletCollision.instance.StartCoroutine(BulletCollision.instance.CheckBulletRange());       
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

            if (WeaponManager.instance.curActionCor[myIndex] != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curActionCor[myIndex]);
            /*
            if (BulletCollision.instance.curCheckRangeCor != null)
                BulletCollision.instance.StopCoroutine(BulletCollision.instance.curCheckRangeCor); */
        }
    }

    public void Shot(bool _value)
    {
        if (_value == false)
        {
            StateManager.instance.SetState(State_AimCircuit.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.ActionAimCircuit());

            if (WeaponManager.instance.curActionCor[myIndex] != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curActionCor[myIndex]);
            /*
            if (BulletCollision.instance.curCheckRangeCor != null)
                BulletCollision.instance.StopCoroutine(BulletCollision.instance.curCheckRangeCor); */
        }
    }
}
