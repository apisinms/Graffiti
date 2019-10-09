﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayersManager : MonoBehaviour
{
    private Vector3 tmpRot = new Vector3(); // 자꾸 new해서 가비지 생성하는거 방지용

    public void Action_Idle(int _index, Vector3 _pos)  //서있을때
    {
        if (myIndex == _index) //함수가 아더용이므로 내인덱스가 혹여라도 인풋됐을때 예외처리. 내 전용함수는 따로있다.
            return;

        // 포지션 동기화
        if (tf_players[_index].localPosition.x != _pos.x ||
           tf_players[_index].localPosition.z != _pos.z)
        {
            tf_players[_index].localPosition = Vector3.MoveTowards(tf_players[_index].localPosition, _pos,
            Time.smoothDeltaTime * speed[_index] * C_Global.interpolation_Pos);
        }

        // 로테이션 동기화(0.5도이내면 수행, 오일러 앵글로 검사)
        if (Mathf.Abs(tf_players[_index].localEulerAngles.y - networkManager.GetRotY(_index)) > 0.5f)
        {
            //서버로부터 받은 플레이어 오일러각도를 다시 방향벡터로 바꿔서 저장해둠.(자꾸 new 하는데 바꾸자)
            tmpRot = Quaternion.AngleAxis(networkManager.GetRotY(_index), Vector3.forward) * Vector3.right;
            direction[_index].x = tmpRot.y;
            direction[_index].z = tmpRot.x;

            tf_players[_index].localRotation = Quaternion.Lerp(tf_players[_index].localRotation,
            Quaternion.LookRotation(direction[_index]), Time.smoothDeltaTime * (C_Global.interpolation_Rot * 2));   // 2배로 러프
        }

        else
        {
            // 각도가 0.1도보다 크다면 보간없이 그냥 대입한다.(어차피 0.5도씩 보간하다가 왔을테니까)
            if (Mathf.Abs(tf_players[_index].localEulerAngles.y - networkManager.GetRotY(_index)) > 0.1f)
            {
                tmpRot.y = networkManager.GetRotY(_index);
                tf_players[_index].localEulerAngles = tmpRot;    // localEulerAngles값을 변경하면 localRotation값도 자동으로 변경된다.
            }

            Anime_Idle(_index);
        }
    }

    public void Action_CircuitNormal(int _index, Vector3 _pos, float _roty) //노말 움직임일때. 순회.
    {
        if (myIndex == _index)
            return;

        // 서버에서 넘겨져온 속도를 가져옴
        speed[_index] = networkManager.GetSpeed(_index);
        Debug.Log("노멀서킷속도" + speed[_index]);
        Anime_Circuit(_index);

        tf_players[_index].localPosition = Vector3.MoveTowards(tf_players[_index].localPosition, _pos,
           Time.smoothDeltaTime * (speed[_index] * C_Global.interpolation_Pos));

        //서버로부터 받은 플레이어 오일러각도를 다시 방향벡터로 바꿔서 저장해둠.
        tmpRot = Quaternion.AngleAxis(_roty, Vector3.forward) * Vector3.right;
        direction[_index].x = tmpRot.y;
        direction[_index].z = tmpRot.x;

        // lerp보간작업
        tf_players[_index].localRotation = Quaternion.Lerp(tf_players[_index].localRotation,
        Quaternion.LookRotation(direction[_index]), Time.smoothDeltaTime * C_Global.interpolation_Rot);
    }

    public void Action_AimingNormal(int _index, Vector3 _pos, float _roty) //제자리 조준
    {
        //if (myIndex == _index)
            //return;

        if (tf_players[_index].localPosition.x != _pos.x ||
           tf_players[_index].localPosition.z != _pos.z)
        {
            tf_players[_index].localPosition = Vector3.MoveTowards(tf_players[_index].localPosition, _pos,
            Time.smoothDeltaTime * speed[_index] * C_Global.interpolation_Pos);
        }

        tmpRot = Quaternion.AngleAxis(_roty, Vector3.forward) * Vector3.right;
        direction2[_index].x = tmpRot.y;
        direction2[_index].z = tmpRot.x;

        // lerp보간작업

        tf_players[_index].localRotation = Quaternion.Lerp(tf_players[_index].localRotation,
        Quaternion.LookRotation(direction2[_index]), Time.smoothDeltaTime * C_Global.interpolation_Rot);

        Anime_Aiming_Idle(_index);
    }

    public void Action_AimingWithCircuit(int _index, Vector3 _pos, float _roty)
    {
        if (myIndex == _index)
            return;

        // 서버에서 넘겨져온 속도를 가져옴
        speed[_index] = networkManager.GetSpeed(_index);
        Debug.Log("에이밍서킷속도:" + speed[_index]);

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

        /*

                tf_players[_index].localPosition = Vector3.Lerp(tf_players[_index].localPosition, _pos,
      Time.smoothDeltaTime * (speed[_index]));
          Vector3 angleToDirection = Quaternion.AngleAxis(_roty, Vector3.forward) * Vector3.right;
          direction2[_index] = new Vector3(angleToDirection.y, 0, angleToDirection.x);
          tf_players[_index].localRotation = Quaternion.Lerp(tf_players[_index].localRotation,
      Quaternion.LookRotation(direction2[_index]), Time.smoothDeltaTime * 15);
          //서버로부터 받은 플레이어 오일러각도를 다시 방향벡터로 바꿔서 저장해둠. 나중에 방향쓸일이 있을수도있으므로.
          //Vector3 angleToDirection = Quaternion.AngleAxis(eulerAngle[_index], Vector3.forward) * Vector3.right;
          //direction[_index] = new Vector3(angleToDirection.y, 0, angleToDirection.x);
          // obj_otherPlayers[_index].transform.localEulerAngles = new Vector3(0, eulerAngle2[_index], 0);
          // obj_otherPlayers[_index].transform.Translate(direction[_index] * (speed[_index] * 0.3f) * Time.smoothDeltaTime, Space.World);
      */
        tf_players[_index].localPosition = Vector3.MoveTowards(tf_players[_index].localPosition, _pos,
       Time.smoothDeltaTime * (speed[_index] * C_Global.amingSpeed * C_Global.interpolation_Pos));

        tmpRot = Quaternion.AngleAxis(_roty, Vector3.forward) * Vector3.right;
        direction2[_index].x = tmpRot.y;
        direction2[_index].z = tmpRot.x;


        tf_players[_index].localRotation = Quaternion.Lerp(tf_players[_index].localRotation,
        Quaternion.LookRotation(direction2[_index]), Time.smoothDeltaTime * C_Global.interpolation_Rot);

    }
}
/*
using System.Collections;
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
        //speed[_index] = NetworkManager.instance.GetSpeed(_index);
        Anime_Circuit(_index);
        tf_players[_index].localPosition = Vector3.Lerp(tf_players[_index].localPosition, _pos,
            Time.smoothDeltaTime * (speed[_index]));
        //서버로부터 받은 플레이어 오일러각도를 다시 방향벡터로 바꿔서 저장해둠.
        Vector3 angleToDirection = Quaternion.AngleAxis(_roty, Vector3.forward) * Vector3.right;
        direction[_index] = new Vector3(angleToDirection.y, 0, angleToDirection.x);
        // lerp보간작업
        tf_players[_index].localRotation = Quaternion.Lerp(tf_players[_index].localRotation, 
            Quaternion.LookRotation(direction[_index]), Time.smoothDeltaTime * 15);
    }
    public void Action_AimingNormal(int _index, float _roty) //제자리 조준
    {
        if (myIndex == _index)
            return;
        Anime_Aiming_Idle(_index);
        Vector3 angleToDirection = Quaternion.AngleAxis(_roty, Vector3.forward) * Vector3.right;
        direction2[_index] = new Vector3(angleToDirection.y, 0, angleToDirection.x);
        tf_players[_index].localRotation = Quaternion.Lerp(tf_players[_index].localRotation,
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
        tf_players[_index].localPosition = Vector3.Lerp(tf_players[_index].localPosition, _pos,
    Time.smoothDeltaTime * (speed[_index]));
        Vector3 angleToDirection = Quaternion.AngleAxis(_roty, Vector3.forward) * Vector3.right;
        direction2[_index] = new Vector3(angleToDirection.y, 0, angleToDirection.x);
        tf_players[_index].localRotation = Quaternion.Lerp(tf_players[_index].localRotation,
    Quaternion.LookRotation(direction2[_index]), Time.smoothDeltaTime * 15);
        //서버로부터 받은 플레이어 오일러각도를 다시 방향벡터로 바꿔서 저장해둠. 나중에 방향쓸일이 있을수도있으므로.
        //Vector3 angleToDirection = Quaternion.AngleAxis(eulerAngle[_index], Vector3.forward) * Vector3.right;
        //direction[_index] = new Vector3(angleToDirection.y, 0, angleToDirection.x);
        // obj_otherPlayers[_index].transform.localEulerAngles = new Vector3(0, eulerAngle2[_index], 0);
        // obj_otherPlayers[_index].transform.Translate(direction[_index] * (speed[_index] * 0.3f) * Time.smoothDeltaTime, Space.World);
    }
}
*/
