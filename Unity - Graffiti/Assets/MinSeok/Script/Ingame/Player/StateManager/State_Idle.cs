using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Idle : MonoBehaviour, IActionState
{
    private static State_Idle instance;
    private int myIndex { get; set; }

    private void Start()
    {
        myIndex = GameManager.instance.myIndex;
    }

    public static State_Idle GetStateInstance()
    {
        if (instance == null)
            instance = (State_Idle)StateManager.instance.cn_stateList[1];// StateManager.instance.obj_stateList.GetComponent<State_Idle>();

        return instance;
    }

    public void Idle(bool _value)
    {
        /*
        if (_value == false)
        {
            StateManager.instance.SetState(State_Death.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_Death());
        }
        */
    }

    public void Death(bool _value)
    {
        if (_value == true)
        {
            StateManager.instance.SetState(State_Death.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_Death());
        }
    }

    public void Circuit(bool _value)
    {
        if (_value == true)
        {
            StateManager.instance.SetState(State_Circuit.GetStateInstance()); //스테이트객체 갱신. 

            if (PlayersManager.instance.curCor != null) //실행된 코루틴을 저장해놓고, 다음코루틴실행때 그실행됬던 코루틴을 정지하고 새코루틴재생 
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_Circuit());
        }
    }
    
    public void Aim(bool _value)
    {
        if (_value == true)
        {
            StateManager.instance.SetState(State_Aim.GetStateInstance()); //스테이트객체 갱신. 

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_Aim());
        }
    }

    public void Shot(bool _value)
    {
        if (_value == true)
        {
            StateManager.instance.SetState(State_Shot.GetStateInstance()); //스테이트객체 갱신. 

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_Aim());

            if (WeaponManager.instance.curMainActionCor[myIndex] != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curMainActionCor[myIndex]);
            WeaponManager.instance.curMainActionCor[myIndex] = WeaponManager.instance.StartCoroutine(WeaponManager.instance.ActionFire(myIndex));
            //EffectManager.instance.PlayEffect(_EFFECT_TYPE.MUZZLE, myIndex);
        }
    }
}
