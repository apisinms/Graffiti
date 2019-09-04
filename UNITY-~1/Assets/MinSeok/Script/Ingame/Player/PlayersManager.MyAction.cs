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
        obj_players[myIndex].transform.localRotation = Quaternion.LookRotation(direction[myIndex]);
        obj_players[myIndex].transform.Translate(direction[myIndex] * speed[myIndex] * Time.smoothDeltaTime, Space.World);
    }

    public void Action_AimingNormal() //제자리 조준
    {
        Anime_Aiming_Idle(myIndex);
        obj_players[myIndex].transform.localRotation = Quaternion.LookRotation(direction2[myIndex]);
    }

    public void Action_AimingWithCircuit()  // 순회와 조준동시
    {  
        //좌측조이스틱 위로일때.         우측조이스틱의 방향에따라서 애니메이션을 달리함.
        if ((new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y >= -30.0f) &&
            (new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y <= 30.0f))
        {
            Anime_AimingWithCircuit(myIndex, 1);
        }
        //우측일때
        else if ((new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y >= 30.0f) &&
            (new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y <= 150.0f))
        {
            Anime_AimingWithCircuit(myIndex, 2);
        }
        //아래일때
        else if ((new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y >= 150.0f &&
            new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y <= 180.0f) ||
            (new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y >= -180.0f &&
            new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y <= -150.0f))
        {
            Anime_AimingWithCircuit(myIndex, 3);
        }
        //좌측일때
        else if ((new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y >= -150.0f) &&
            (new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y <= -30.0f))
        {
            Anime_AimingWithCircuit(myIndex, 4);
        } 
     
        obj_players[myIndex].transform.localRotation = Quaternion.LookRotation(direction2[myIndex]);
        obj_players[myIndex].transform.Translate(direction[myIndex] * (speed[myIndex] * 0.35f) * Time.smoothDeltaTime, Space.World);
    }

}






/*
          //좌측조이스틱 위로일때 우측조이스틱의 방향에따라서 애니메이션을 달리함.
        if ((new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y >= -30.0f) &&
            (new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y <= 30.0f))
        {
            am_animePlayer[myIndex].SetInteger("DirectionLeftstick", 1);
       
            if (obj_players[myIndex].transform.localEulerAngles.y >= -30.0f && obj_players[myIndex].transform.localEulerAngles.y <= 70.0f)
                //Anime_Aiming_Forward(myIndex); 
            else if (obj_players[myIndex].transform.localEulerAngles.y >= 70.0f && obj_players[myIndex].transform.localEulerAngles.y <= 170.0f)
                //Anime_Aiming_Left(myIndex);
            else if (obj_players[myIndex].transform.localEulerAngles.y >= 170.0f && obj_players[myIndex].transform.localEulerAngles.y <= 240.0f)
                //Anime_Aiming_Back(myIndex);
            else if (obj_players[myIndex].transform.localEulerAngles.y >= 240.0f && obj_players[myIndex].transform.localEulerAngles.y <= 330.0f)
                //Anime_Aiming_Right(myIndex);
             

        }//우측일때
        else if ((new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y >= 30.0f) &&
            (new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y <= 150.0f))
        {
            am_animePlayer[myIndex].SetInteger("DirectionLeftstick", 2);

       
            if (obj_players[myIndex].transform.localEulerAngles.y >= -30.0f && obj_players[myIndex].transform.localEulerAngles.y <= 70.0f)
                //Anime_Aiming_Right(myIndex);
            else if (obj_players[myIndex].transform.localEulerAngles.y >= 70.0f && obj_players[myIndex].transform.localEulerAngles.y <= 170.0f)
                //Anime_Aiming_Forward(myIndex);
            else if (obj_players[myIndex].transform.localEulerAngles.y >= 170.0f && obj_players[myIndex].transform.localEulerAngles.y <= 240.0f)
                //Anime_Aiming_Left(myIndex);
            else if (obj_players[myIndex].transform.localEulerAngles.y >= 240.0f && obj_players[myIndex].transform.localEulerAngles.y <= 330.0f)
                //Anime_Aiming_Back(myIndex);
             
        }
        //아래일때
        else if ((new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y >= 150.0f &&
            new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y <= 180.0f) ||
            (new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y >= -180.0f &&
            new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y <= -150.0f))
        {
            am_animePlayer[myIndex].SetInteger("DirectionLeftstick", 3);

      
            if (obj_players[myIndex].transform.localEulerAngles.y >= -30.0f && obj_players[myIndex].transform.localEulerAngles.y <= 70.0f)
                //Anime_Aiming_Back(myIndex);
            else if (obj_players[myIndex].transform.localEulerAngles.y >= 70.0f && obj_players[myIndex].transform.localEulerAngles.y <= 170.0f)
                //Anime_Aiming_Right(myIndex);
            else if (obj_players[myIndex].transform.localEulerAngles.y >= 170.0f && obj_players[myIndex].transform.localEulerAngles.y <= 240.0f)
                //Anime_Aiming_Forward(myIndex);
            else if (obj_players[myIndex].transform.localEulerAngles.y >= 240.0f && obj_players[myIndex].transform.localEulerAngles.y <= 330.0f)
               // Anime_Aiming_Left(myIndex);
            
        } //좌측일때
        else if ((new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y >= -150.0f) &&
            (new Vector3(0, Mathf.Atan2(direction[myIndex].x, direction[myIndex].z) * Mathf.Rad2Deg, 0).y <= -30.0f))
        {
            am_animePlayer[myIndex].SetBool("isAimingAndCurcuit", true);
am_animePlayer[myIndex].SetInteger("DirectionLeftstick", 4);
am_animePlayer[myIndex].SetFloat("Direction_rightX", direction2[myIndex].x);
am_animePlayer[myIndex].SetFloat("Direction_rightY", direction2[myIndex].z);


if (obj_players[myIndex].transform.localEulerAngles.y >= -30.0f && obj_players[myIndex].transform.localEulerAngles.y <= 70.0f)
    //Anime_Aiming_Left(myIndex);
else if (obj_players[myIndex].transform.localEulerAngles.y >= 70.0f && obj_players[myIndex].transform.localEulerAngles.y <= 170.0f)
    //Anime_Aiming_Back(myIndex);
else if (obj_players[myIndex].transform.localEulerAngles.y >= 170.0f && obj_players[myIndex].transform.localEulerAngles.y <= 240.0f)
    //Anime_Aiming_Right(myIndex);
else if (obj_players[myIndex].transform.localEulerAngles.y >= 240.0f && obj_players[myIndex].transform.localEulerAngles.y <= 330.0f)
    //Anime_Aiming_Forward(myIndex);
    */