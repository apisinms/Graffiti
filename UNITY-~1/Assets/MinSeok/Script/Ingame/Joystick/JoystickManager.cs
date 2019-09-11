using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public interface IJoystickControll
{
    void DragStart();
    void Drag(BaseEventData _Data);
    void DragEnd();
}

public class JoystickManager : MonoBehaviour
{
    private int myIndex { get; set; }

    void Awake()
    {
        myIndex = GameManager.instance.myIndex;
    }
    void Update()
    {
        //    if (Input.GetKeyDown(KeyCode.Space))

        //Debug.Log(MyPlayerManager.instance.myActionState);
        //Debug.Log(PlayersManager.instance.direction[PlayersManager.instance.myIndex]);
        //       new Vector3(Mathf.Cos(MyPlayerManager.instance.obj_myPlayer.transform.localEulerAngles.y),
        //       0.0f, Mathf.Sin(MyPlayerManager.instance.obj_myPlayer.transform.localEulerAngles.y)));
        //ClientSectorManager.instance.GetMyArea();
        //  Debug.Log(PlayersManager.instance.direction[PlayersManager.instance.myIndex]);
        //Debug.Log(PlayersManager.instance.direction2[PlayersManager.instance.myIndex]);

        //Debug.Log(PlayersManager.instance.actionState[PlayersManager.instance.myIndex]);

        //  Debug.Log(new Vector3(0, Mathf.Atan2(PlayersManager.instance.direction[PlayersManager.instance.myIndex].x,
        //    PlayersManager.instance.direction[PlayersManager.instance.myIndex].z) * Mathf.Rad2Deg, 0));
        //Debug.Log(PlayersManager.instance.obj_players[PlayersManager.instance.myIndex].transform.);
        //Debug.Log(PlayersManager.instance.obj_players[PlayersManager.instance.myIndex].transform.localEulerAngles.y);

        //tf_player.eulerAngles = new Vector3(0, Mathf.Atan2(playerDir.x, playerDir.y) * Mathf.Rad2Deg, 0); /
        /*
        Debug.Log(

            new Vector3(0, Mathf.Atan2(PlayersManager.instance.direction[PlayersManager.instance.myIndex].x,
            PlayersManager.instance.direction[PlayersManager.instance.myIndex].z) * Mathf.Rad2Deg, 0)
           
            );
          */

        //Debug.Log(PlayersManager.instance.stateInfo[myIndex].actionState);



        /*
        switch (PlayersManager.instance.stateInfo[myIndex].actionState)
        {
            case _ACTION_STATE.IDLE:
                PlayersManager.instance.Action_Idle();
                break;
            case _ACTION_STATE.CIR:          
                PlayersManager.instance.Action_CircuitNormal();
                break;
            case _ACTION_STATE.AIM:
                PlayersManager.instance.Action_AimingNormal();
                break;
            case _ACTION_STATE.CIR_AIM:
                PlayersManager.instance.Action_AimingWithCircuit();
                break;
            case _ACTION_STATE.CIR_AIM_SHOT:
                break;
        }
        */
    }

}
