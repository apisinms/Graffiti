using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어 애니메이터가 들어있다. 
 * 플레이어 애니메이션 재생/정지 함수가 들어있다.
 */

public partial class PlayersManager : MonoBehaviour
{
    #region PLAYERS_ANIMATOR
    public Animator[] am_animePlayer { get; set; }
    #endregion

    public void Anime_Spray(int _index)
    {
        if (am_animePlayer[_index].GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Spray"))
            return;
     
        am_animePlayer[_index].SetTrigger("Spray");
    }

    public void Anime_Death(int _index)
    {
        if (am_animePlayer[_index].GetBool("isAimingAndCurcuit") == true)
            am_animePlayer[_index].SetBool("isAimingAndCurcuit", false);

        if (am_animePlayer[_index].GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Death"))
            return;

        obj_players[_index].GetComponent<CapsuleCollider>().isTrigger = true;
        am_animePlayer[_index].SetFloat("DeathRandom", Random.Range(0, 3));
        am_animePlayer[_index].SetTrigger("Death");
    }

    public void Anime_Idle(int _index)
    {
        if (am_animePlayer[_index].GetBool("isAimingAndCurcuit") == true)
            am_animePlayer[_index].SetBool("isAimingAndCurcuit", false);

        if (am_animePlayer[_index].GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle"))
            return;

        obj_players[_index].GetComponent<CapsuleCollider>().isTrigger = false;
        am_animePlayer[_index].SetTrigger("Idle");
    }

    public void Anime_Circuit(int _index)
    {
        if (am_animePlayer[_index].GetBool("isAimingAndCurcuit") == true)
            am_animePlayer[_index].SetBool("isAimingAndCurcuit", false);

        am_animePlayer[_index].SetFloat("Speed", speed[_index]);
        am_animePlayer[_index].SetTrigger("Curcuit");
    }
    public void Anime_Aiming_Idle(int _index)
    {
        if (am_animePlayer[_index].GetBool("isAimingAndCurcuit") == true)
            am_animePlayer[_index].SetBool("isAimingAndCurcuit", false);

        am_animePlayer[_index].SetTrigger("Aiming_Idle");
    }
    public void Anime_AimingWithCircuit(int _index, int _directionNum)
    {
        if (am_animePlayer[_index].GetBool("isAimingAndCurcuit") == false)
            am_animePlayer[_index].SetBool("isAimingAndCurcuit", true);

        am_animePlayer[_index].SetInteger("DirectionLeftstick", _directionNum); //우측스틱방향. 즉 플레이어의 조준뱡향을 애니메이터에넘김
        am_animePlayer[_index].SetFloat("Direction_rightX", direction2[_index].x); //블렌드에서 쓸 스틱방향(조준방향)좌표를 넘김.
        am_animePlayer[_index].SetFloat("Direction_rightY", direction2[_index].z);
    }

}




/*
public void Anime_Aiming_Left(int _index)
{
    //am_animePlayer[_index].SetBool("Aiming", true);
    //am_animePlayer[_index].SetTrigger("Aiming_Left");
}
public void Anime_Aiming_Right(int _index)
{
    //am_animePlayer[_index].SetBool("Aiming", true);
    // am_animePlayer[_index].SetTrigger("Aiming_Right");
}
public void Anime_Aiming_Forward(int _index)
{
    //am_animePlayer[_index].SetBool("Aiming", true);
    //am_animePlayer[_index].SetTrigger("Aiming_Forward");
}
public void Anime_Aiming_Back(int _index)
{
   // am_animePlayer[_index].SetBool("Aiming", true);
    //am_animePlayer[_index].SetTrigger("Aiming_Back");      
}
*/
