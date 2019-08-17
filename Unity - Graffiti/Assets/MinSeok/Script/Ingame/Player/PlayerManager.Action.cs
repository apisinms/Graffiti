using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어의 움직임관련 메서드가 들어있다.
 */

public enum _ACTION_STATE
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

    public void Action_Idle()
    {
        //Debug.Log("4번호출", this);
    //    PlayerManager.instance.Anime_Idle();
    }
    public void Action_Circuit()
    {
    //    PlayerManager.instance.Anime_Circuit();
        this.transform.localRotation = Quaternion.LookRotation(PlayerManager.instance.myDirection);
        this.transform.Translate(PlayerManager.instance.myDirection * PlayerManager.instance.speed * Time.smoothDeltaTime, Space.World);
    }

    public void Action_Aiming()
    {
        this.transform.localRotation = Quaternion.LookRotation(PlayerManager.instance.myDirection);
    }


}
