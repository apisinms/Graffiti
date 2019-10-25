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
            instance = (State_AimCircuitShot)StateManager.instance.cn_stateList[7];  //StateManager.instance.obj_stateList.GetComponent<State_AimCircuitShot>();

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
        if (_value == false)
        {
            if (WeaponManager.instance.mainWeapon[myIndex] == _WEAPONS.SG)
            {
                StateManager.instance.SetState(State_Aim.GetStateInstance());

                if (PlayersManager.instance.curCor != null)
                    PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
                PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_Aim());

                if (WeaponManager.instance.curMainActionCor[myIndex] != null)
                    WeaponManager.instance.StopCoroutine(WeaponManager.instance.curMainActionCor[myIndex]);
                EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, myIndex);
                return;
            }

            StateManager.instance.SetState(State_Shot.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_Aim());

            //if (WeaponManager.instance.curMainActionCor[myIndex] != null)
                //WeaponManager.instance.StopCoroutine(WeaponManager.instance.curMainActionCor[myIndex]);
            //WeaponManager.instance.curMainActionCor[myIndex] = WeaponManager.instance.StartCoroutine(WeaponManager.instance.ActionFire(myIndex));
            //EffectManager.instance.PlayEffect(_EFFECT_TYPE.MUZZLE, myIndex);
        }
    }

    public void Aim(bool _value)
    {
        if (_value == false)
        {
            StateManager.instance.SetState(State_Circuit.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_Circuit());

            if (WeaponManager.instance.curMainActionCor[myIndex] != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curMainActionCor[myIndex]);
            EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, myIndex);
        }
    }

    public void Shot(bool _value)
    {
        if (_value == false)
        {
            StateManager.instance.SetState(State_AimCircuit.GetStateInstance());

            if (PlayersManager.instance.curCor != null)
                PlayersManager.instance.StopCoroutine(PlayersManager.instance.curCor);
            PlayersManager.instance.curCor = PlayersManager.instance.StartCoroutine(PlayersManager.instance.Action_AimCircuit());

            if (WeaponManager.instance.curMainActionCor[myIndex] != null)
                WeaponManager.instance.StopCoroutine(WeaponManager.instance.curMainActionCor[myIndex]);
            EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, myIndex);
        }
    }
}
