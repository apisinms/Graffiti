using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어의 움직임관련 메서드가 들어있다.
 */

public partial class PlayersManager : MonoBehaviour
{

    public void Action_Idle() { Anime_Idle(myIndex); } //서있을때

    public void Action_CircuitNormal() //노말 움직임일때. 순회.
    {
        Anime_Circuit(myIndex);

        // 이걸 서버로 전송
        obj_players[myIndex].transform.localRotation = Quaternion.LookRotation(direction[myIndex]);
        obj_players[myIndex].transform.Translate(direction[myIndex] * speed[myIndex] * Time.smoothDeltaTime, Space.World);
    }

    public void Action_AimingNormal() //제자리 조준
    {
        Anime_Idle(myIndex);
        obj_players[myIndex].transform.localRotation = Quaternion.LookRotation(direction2[myIndex]);
    }

    public void Action_AimingWithCircuit()  // 순회와 조준동시
    {
        Anime_Idle(myIndex);
        obj_players[myIndex].transform.localRotation = Quaternion.LookRotation(direction2[myIndex]);
        obj_players[myIndex].transform.Translate(direction[myIndex] * (speed[myIndex] * 0.3f) * Time.smoothDeltaTime, Space.World);
    }

}
