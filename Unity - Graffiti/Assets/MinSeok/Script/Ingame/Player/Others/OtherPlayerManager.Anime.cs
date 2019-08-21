using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class OtherPlayerManager : MonoBehaviour
{
    public Animator[] animePlayer { get; set; }

    public void Anime_Idle(int _index) { animePlayer[_index].SetTrigger("Idle"); }
    public void Anime_Circuit(int _index) { animePlayer[_index].SetTrigger("Curcuit"); }
}
