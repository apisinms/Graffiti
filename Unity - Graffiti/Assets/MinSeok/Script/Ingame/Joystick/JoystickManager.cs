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

        switch (PlayerManager.instance.myActionState)
        {
            case _ACTION_STATE.IDLE:
                PlayerManager.instance.Action_Idle();
                break;
            case _ACTION_STATE.CIRCUIT:
                //ClientSectorManager.instance.ProcessWhereAmI();
                PlayerManager.instance.Action_Circuit();
                break;
            case _ACTION_STATE.AIMING:
                PlayerManager.instance.Action_Aiming();
                break;
        }
    }

}
