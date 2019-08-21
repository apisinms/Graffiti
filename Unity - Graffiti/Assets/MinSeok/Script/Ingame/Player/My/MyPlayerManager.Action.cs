using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어의 움직임관련 메서드가 들어있다.
 */


public partial class MyPlayerManager : MonoBehaviour
{
    public _ACTION_STATE myActionState { get; set; }

    public void Action_Idle() //서있을때
    {
        Anime_Idle();
        moveFlag = false;
    }

    public void Action_CircuitNormal() //노말 움직임일때. 순회.
    {
        moveFlag = true;
        Anime_Circuit();

        // 이걸 서버로 전송
        obj_myPlayer.transform.localRotation = Quaternion.LookRotation(myDirection);
        obj_myPlayer.transform.Translate(myDirection * mySpeed * Time.smoothDeltaTime, Space.World);

    }

    public void Action_AimingNormal() //제자리 조준
    {
        Anime_Idle();
        obj_myPlayer.transform.localRotation = Quaternion.LookRotation(myDirection2);
    }

    public void Action_AimingWithCircuit()  // 순회와 조준동시
    {
        Anime_Idle();
        obj_myPlayer.transform.localRotation = Quaternion.LookRotation(myDirection2);
        obj_myPlayer.transform.Translate(myDirection * (mySpeed * 0.3f) * Time.smoothDeltaTime, Space.World);
    }
}
