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
        // Debug.Log(PlayerManager.instance.myActionState);

        /*
        if (Input.GetKeyDown(KeyCode.Space))
            PlayerManager.instance.myActionState += (int)_ACTION_STATE.AIMING;
        if (Input.GetKeyUp(KeyCode.Space))
            PlayerManager.instance.myActionState -= (int)_ACTION_STATE.AIMING;
            */
        switch (PlayerManager.instance.myActionState)
        {
            case _ACTION_STATE.IDLE:
                PlayerManager.instance.Action_Idle();
                break;
            case _ACTION_STATE.CIRCUIT:
                PlayerManager.instance.Action_Circuit();
                break;
            case _ACTION_STATE.AIMING:
                PlayerManager.instance.Action_Aiming();
                break;
        }

    }
    
}
