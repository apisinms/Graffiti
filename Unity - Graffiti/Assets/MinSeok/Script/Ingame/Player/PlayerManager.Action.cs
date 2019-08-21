using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어의 움직임관련 메서드가 들어있다.
 */

public enum _ACTION_STATE //액션(움직임)의 상태
{
    // 단일 STATE
    IDLE = 0,
    CIRCUIT,
    AIMING,

    // 복합 STATE
    CIRCUIT_AND_AIMING = (CIRCUIT + AIMING),
}

public partial class PlayerManager : MonoBehaviour
{
    public _ACTION_STATE myActionState { get; set; }

    public void Action_Idle() //서있을때
    {
        Anime_Idle();
    }

    public void Action_CircuitNormal() //노말 움직임일때. 순회.
    {
        Anime_Circuit();

		// 이걸 서버로 전송
        transform.localRotation = Quaternion.LookRotation(myDirection);
        transform.Translate(myDirection * mySpeed * Time.smoothDeltaTime, Space.World);
    }

    public void Action_AimingNormal() //제자리 조준또는 순회와 조준동시.
    {
        Anime_Idle();
        transform.localRotation = Quaternion.LookRotation(myDirection2);
    }

    public void Action_CircuitWithAiming() 
    {
        //Anime_Circuit();
        transform.Translate(myDirection * (mySpeed * 0.3f) * Time.smoothDeltaTime, Space.World);
    }

    public void Action_AimingWithCircuit() 
    {
        Anime_Idle();
        transform.localRotation = Quaternion.LookRotation(myDirection2);
    }

}
