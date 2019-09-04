﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayersManager : MonoBehaviour
{
    public void Action_Idle(int _index)  //서있을때
    {
        if (myIndex == _index) //함수가 아더용이므로 내인덱스가 혹여라도 인풋됐을때 예외처리. 내전용함수는 따로있다.
            return;

        Anime_Idle(_index);
    } 

    public void Action_CircuitNormal(int _index, Vector3 _pos, float _roty) //노말 움직임일때. 순회.
    {
        if (myIndex == _index)
            return;

        Anime_Circuit(_index);

        obj_players[_index].transform.localPosition = Vector3.Lerp(obj_players[_index].transform.localPosition, _pos,
            Time.smoothDeltaTime * (speed[_index]));

        //서버로부터 받은 플레이어 오일러각도를 다시 방향벡터로 바꿔서 저장해둠.
        Vector3 angleToDirection = Quaternion.AngleAxis(_roty, Vector3.forward) * Vector3.right;
        direction[_index] = new Vector3(angleToDirection.y, 0, angleToDirection.x);

        // lerp보간작업
        obj_players[_index].transform.localRotation = Quaternion.Lerp(obj_players[_index].transform.localRotation, 
            Quaternion.LookRotation(direction[_index]), Time.smoothDeltaTime * 15);
    }

    public void Action_AimingNormal(int _index, float _roty) //제자리 조준
    {
        if (myIndex == _index)
            return;

        Anime_Aiming_Idle(_index);

        Vector3 angleToDirection = Quaternion.AngleAxis(_roty, Vector3.forward) * Vector3.right;
        direction2[_index] = new Vector3(angleToDirection.y, 0, angleToDirection.x);

        obj_players[_index].transform.localRotation = Quaternion.Lerp(obj_players[_index].transform.localRotation,
    Quaternion.LookRotation(direction2[_index]), Time.smoothDeltaTime * 15);
    }

    public void Action_AimingWithCircuit(int _index, Vector3 _pos, float _roty )
    {
        if (myIndex == _index)
            return;

        //좌측조이스틱 위로일때.         우측조이스틱의 방향에따라서 애니메이션을 달리함.
        if ((new Vector3(0, Mathf.Atan2(direction[_index].x, direction[_index].z) * Mathf.Rad2Deg, 0).y >= -30.0f) &&
            (new Vector3(0, Mathf.Atan2(direction[_index].x, direction[_index].z) * Mathf.Rad2Deg, 0).y <= 30.0f))
        {
            Anime_AimingWithCircuit(_index, 1);
        }
        //우측일때
        else if ((new Vector3(0, Mathf.Atan2(direction[_index].x, direction[_index].z) * Mathf.Rad2Deg, 0).y >= 30.0f) &&
            (new Vector3(0, Mathf.Atan2(direction[_index].x, direction[_index].z) * Mathf.Rad2Deg, 0).y <= 150.0f))
        {
            Anime_AimingWithCircuit(_index, 2);
        }
        //아래일때
        else if ((new Vector3(0, Mathf.Atan2(direction[_index].x, direction[_index].z) * Mathf.Rad2Deg, 0).y >= 150.0f &&
            new Vector3(0, Mathf.Atan2(direction[_index].x, direction[_index].z) * Mathf.Rad2Deg, 0).y <= 180.0f) ||
            (new Vector3(0, Mathf.Atan2(direction[_index].x, direction[_index].z) * Mathf.Rad2Deg, 0).y >= -180.0f &&
            new Vector3(0, Mathf.Atan2(direction[_index].x, direction[_index].z) * Mathf.Rad2Deg, 0).y <= -150.0f))
        {
            Anime_AimingWithCircuit(_index, 3);
        }
        //좌측일때
        else if ((new Vector3(0, Mathf.Atan2(direction[_index].x, direction[_index].z) * Mathf.Rad2Deg, 0).y >= -150.0f) &&
            (new Vector3(0, Mathf.Atan2(direction[_index].x, direction[_index].z) * Mathf.Rad2Deg, 0).y <= -30.0f))
        {
            Anime_AimingWithCircuit(_index, 4);
        }


        obj_players[_index].transform.localPosition = Vector3.Lerp(obj_players[_index].transform.localPosition, _pos,
    Time.smoothDeltaTime * (speed[_index]));

        Vector3 angleToDirection = Quaternion.AngleAxis(_roty, Vector3.forward) * Vector3.right;
        direction2[_index] = new Vector3(angleToDirection.y, 0, angleToDirection.x);

        obj_players[_index].transform.localRotation = Quaternion.Lerp(obj_players[_index].transform.localRotation,
    Quaternion.LookRotation(direction2[_index]), Time.smoothDeltaTime * 15);

        //서버로부터 받은 플레이어 오일러각도를 다시 방향벡터로 바꿔서 저장해둠. 나중에 방향쓸일이 있을수도있으므로.
        //Vector3 angleToDirection = Quaternion.AngleAxis(eulerAngle[_index], Vector3.forward) * Vector3.right;
        //direction[_index] = new Vector3(angleToDirection.y, 0, angleToDirection.x);

        // obj_otherPlayers[_index].transform.localEulerAngles = new Vector3(0, eulerAngle2[_index], 0);
        // obj_otherPlayers[_index].transform.Translate(direction[_index] * (speed[_index] * 0.3f) * Time.smoothDeltaTime, Space.World);
    }

}
