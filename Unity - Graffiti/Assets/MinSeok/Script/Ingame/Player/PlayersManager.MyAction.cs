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

    public Coroutine curCor { get; set; } //현재 실행중인 코루틴을 저장.
    #endregion

    public IEnumerator Action_Death()
    {
        while (true)
        {
            Anime_Death(myIndex);
            UIManager.instance.UpdateAimDirectionImg(false);
            yield return null;
        }
    }

    public IEnumerator Action_Idle()
    {
        while (true)
        {
            Anime_Idle(myIndex);
            UIManager.instance.UpdateAimDirectionImg(false);
            yield return null;
        }
    }

    public IEnumerator Action_Circuit()
    {
        while (true)
        {
            Anime_Circuit(myIndex);
            BlockCollisionEachOther();
            UIManager.instance.UpdateAimDirectionImg(false);

            tf_players[myIndex].localRotation = Quaternion.LookRotation(direction[myIndex]);
            tf_players[myIndex].Translate(direction[myIndex] * speed[myIndex] * Time.smoothDeltaTime, Space.World);
            yield return null;

        }
    }
    public IEnumerator Action_Aim()
    {
        while (true)
        {
            Anime_Aiming_Idle(myIndex);
            UIManager.instance.UpdateAimDirectionImg(true); //에임 조준선

            tf_players[myIndex].localRotation = Quaternion.Lerp(tf_players[myIndex].localRotation, Quaternion.LookRotation(direction2[myIndex]), Time.smoothDeltaTime * 6.0f);
            //tf_players[myIndex].localRotation = Quaternion.LookRotation(direction2[myIndex]);
            yield return null;
        }
    }

    public IEnumerator Action_AimCircuit()
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
             UIManager.instance.UpdateAimDirectionImg(true);

             //tf_players[myIndex].localRotation = Quaternion.LookRotation(direction2[myIndex]);
             tf_players[myIndex].localRotation = Quaternion.Lerp(tf_players[myIndex].localRotation, Quaternion.LookRotation(direction2[myIndex]), Time.smoothDeltaTime * 6.0f);
             tf_players[myIndex].Translate(direction[myIndex] * (speed[myIndex] * 0.35f) * Time.smoothDeltaTime, Space.World);
             yield return null;
         }
     }

       public void BlockCollisionEachOther()
       {
           for (int i = 0; i < GameManager.instance.gameInfo.maxPlayer; i++)
           {
               _ACTION_STATE actionState = _ACTION_STATE.IDLE;
#if NETWORK
            actionState = (_ACTION_STATE)networkManager.GetActionState(i);
#endif
                if (myIndex == i || obj_players[i].activeSelf == false || actionState == _ACTION_STATE.DEATH)
                    continue;

                if (Vector3.Distance(tf_players[myIndex].position, tf_players[i].position) <= 1.05f) //나랑 다른플레이어 거리로 충돌을 구함.
                {
                    tf_players[myIndex].localPosition = new Vector3(lastPosX[i], 0, lastPosZ[i]); //else에서 받은 충돌전 마지막 좌표로 플레이어를 고정시킴.
                    break;
                }
                else //충돌아니면 내좌표를 계속받음
                {
                    lastPosX[i] = tf_players[myIndex].localPosition.x;
                    lastPosZ[i] = tf_players[myIndex].localPosition.z;
                }

                /*
                if (tf_players[myIndex].localPosition.x <= (tf_players[i].localPosition.x - 0.18f) ||
                    tf_players[myIndex].localPosition.x >= (tf_players[i].localPosition.x + 0.18f) ||
                    tf_players[myIndex].localPosition.z <= (tf_players[i].localPosition.z - 0.18f) ||
                    tf_players[myIndex].localPosition.z >= (tf_players[i].localPosition.z + 0.18f)) */
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