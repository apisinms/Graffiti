using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어의 움직임관련 메서드가 들어있다.
 */

public partial class PlayerManager : MonoBehaviour
{
    public bool moveFlag { get; set; }

    public void Move()
    {
        if (PlayerManager.instance.moveFlag == true)
        {
            PlayerManager.instance.am_playerMovement.SetBool("idle_to_run", true);
            this.transform.localRotation = Quaternion.LookRotation(PlayerManager.instance.myDirection);
            this.transform.Translate(PlayerManager.instance.myDirection * PlayerManager.instance.speed * Time.smoothDeltaTime, Space.World);
        }
        else
            PlayerManager.instance.am_playerMovement.SetBool("idle_to_run", false);
    }


}
