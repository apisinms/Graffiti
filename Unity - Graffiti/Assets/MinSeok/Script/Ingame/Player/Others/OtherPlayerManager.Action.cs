using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class OtherPlayerManager : MonoBehaviour
{
    public _ACTION_STATE[] actionState { get; set; }

    public void Action_Idle(int _index) //서있을때
    {
        Anime_Idle(_index);
    }

    public void Action_CircuitNormal(int _index) //노말 움직임일때. 순회.
    {
        Anime_Circuit(_index);

        //서버로부터 받은 플레이어 오일러각도를 다시 방향벡터로 바꿔서 저장해둠. 나중에 방향쓸일이 있을수도있으므로.
        Vector3 angleToDirection = Quaternion.AngleAxis(eulerAngle[_index], Vector3.forward) * Vector3.right;
        direction[_index] = new Vector3(angleToDirection.y, 0, angleToDirection.x);
 
        // 이걸 서버로 전송.  해당 플레이어 로빈의 오일러각도를넣고 그각도로 다시계산한 방향벡터로 캐릭터움직임.
        obj_otherPlayers[_index].transform.localEulerAngles = new Vector3(0, eulerAngle[_index], 0);
        obj_otherPlayers[_index].transform.Translate(direction[_index] *speed[_index] * Time.smoothDeltaTime, Space.World);
    }

    public void Action_AimingNormal(int _index) //제자리 조준또는 순회와 조준동시.
    {
        Anime_Idle(_index);
        obj_otherPlayers[_index].transform.localEulerAngles = new Vector3(0, eulerAngle2[_index], 0);
    }

    public void Action_AimingWithCircuit(int _index)
    {
        Anime_Idle(_index);

        //서버로부터 받은 플레이어 오일러각도를 다시 방향벡터로 바꿔서 저장해둠. 나중에 방향쓸일이 있을수도있으므로.
        Vector3 angleToDirection = Quaternion.AngleAxis(eulerAngle[_index], Vector3.forward) * Vector3.right;
        direction[_index] = new Vector3(angleToDirection.y, 0, angleToDirection.x);

        obj_otherPlayers[_index].transform.localEulerAngles = new Vector3(0, eulerAngle2[_index], 0);
        obj_otherPlayers[_index].transform.Translate(direction[_index] * (speed[_index] * 0.3f) * Time.smoothDeltaTime, Space.World);
    }

}
