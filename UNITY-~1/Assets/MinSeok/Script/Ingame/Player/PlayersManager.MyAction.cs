using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어의 움직임관련 메서드가 들어있다.
 */

public partial class PlayersManager : MonoBehaviour
{
    #region PLAYERS_ACTION
    private float[] lastPosX { get; set; }
    private float[] lastPosZ { get; set; }

    public Coroutine curCor; //현재 실행중인 코루틴을 저장.
    public Coroutine prevCor; //이전실행되었던 코루틴저장. 중복방지.
    public IEnumerator cor_ActionIdle { get; set; }
    public IEnumerator cor_ActionCircuit { get; set; }
    public IEnumerator cor_ActionAim { get; set; }
    public IEnumerator cor_ActionAimCurcuit { get; set; }
    #endregion

    public IEnumerator ActionIdle()
    {
        while (true)
        {
            Anime_Idle(myIndex);
            yield return null;
        }
    }

    public IEnumerator ActionCircuit()
    {
        while(true)
        {
            Anime_Circuit(myIndex);
            BlockCollisionEachOther();

            tf_players[myIndex].localRotation = Quaternion.LookRotation(direction[myIndex]);
            tf_players[myIndex].Translate(direction[myIndex] * speed[myIndex] * Time.smoothDeltaTime, Space.World);
            yield return null;
        }
    }

    public IEnumerator ActionAim()
    {
        while (true)
        {
            Anime_Aiming_Idle(myIndex);
            tf_players[myIndex].localRotation = Quaternion.LookRotation(direction2[myIndex]);
            yield return null;
        }
    }

    public IEnumerator ActionAimCircuit()
    {
        while (true)
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

            BlockCollisionEachOther();
            tf_players[myIndex].localRotation = Quaternion.LookRotation(direction2[myIndex]);
            tf_players[myIndex].Translate(direction[myIndex] * (speed[myIndex] * 0.35f) * Time.smoothDeltaTime, Space.World);
            yield return null;
        }
    }


  
    public void BlockCollisionEachOther()
    {
        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            if (myIndex == i)
                continue;

            if (tf_players[myIndex].localPosition.x <= (tf_players[i].localPosition.x - 0.18f) ||
                tf_players[myIndex].localPosition.x >= (tf_players[i].localPosition.x + 0.18f) ||
                tf_players[myIndex].localPosition.z <= (tf_players[i].localPosition.z - 0.18f) ||
                tf_players[myIndex].localPosition.z >= (tf_players[i].localPosition.z + 0.18f))
            {
                lastPosX[i] = tf_players[myIndex].localPosition.x;
                lastPosZ[i] = tf_players[myIndex].localPosition.z;
            }
            else
            {
                tf_players[myIndex].localPosition = new Vector3(lastPosX[i], 0, lastPosZ[i]);
                break;
            }
        }
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