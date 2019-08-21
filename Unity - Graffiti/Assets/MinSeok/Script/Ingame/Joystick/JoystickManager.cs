using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public interface JoystickControll
{
    void DragStart();
    void Drag(BaseEventData _Data);
    void DragEnd();
}

public class JoystickManager : MonoBehaviour
{
    void Update()
    {
        //    if (Input.GetKeyDown(KeyCode.Space))

        //Debug.Log(MyPlayerManager.instance.myActionState);
        //    Debug.Log(MyPlayerManager.instance.myDirection + "         " +
        //       new Vector3(Mathf.Cos(MyPlayerManager.instance.obj_myPlayer.transform.localEulerAngles.y),
        //       0.0f, Mathf.Sin(MyPlayerManager.instance.obj_myPlayer.transform.localEulerAngles.y)));

        switch (MyPlayerManager.instance.myActionState)
        {
            case _ACTION_STATE.IDLE:
                MyPlayerManager.instance.Action_Idle();
                break;
            case _ACTION_STATE.CIRCUIT:
                //ClientSectorManager.instance.ProcessWhereAmI();
                MyPlayerManager.instance.Action_CircuitNormal();
                break;
            case _ACTION_STATE.AIMING:
                MyPlayerManager.instance.Action_AimingNormal();
                break;
            case _ACTION_STATE.CIRCUIT_AND_AIMING:
                MyPlayerManager.instance.Action_AimingWithCircuit();
                break;
        }
    }

}
