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
        ClientSectorManager.instance.GetMyArea();

        switch (PlayersManager.instance.actionState[PlayersManager.instance.myIndex])
        {
            case _ACTION_STATE.IDLE:
                PlayersManager.instance.Action_Idle();
                break;
            case _ACTION_STATE.CIRCUIT:
                
                PlayersManager.instance.Action_CircuitNormal();
                break;
            case _ACTION_STATE.AIMING:
                PlayersManager.instance.Action_AimingNormal();
                break;
            case _ACTION_STATE.CIRCUIT_AND_AIMING:
                PlayersManager.instance.Action_AimingWithCircuit();
                break;
        }
    }

}
