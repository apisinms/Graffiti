using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어 애니메이터가 들어있다. 
 * 플레이어 애니메이션 재생/정지 함수가 들어있다.
 */

public partial class PlayersManager : MonoBehaviour
{
    public void Anime_Idle(int _index) { am_animePlayer[_index].SetTrigger("Idle"); }
    public void Anime_Circuit(int _index) { am_animePlayer[_index].SetTrigger("Curcuit"); }
}
