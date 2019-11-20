using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersGameResult : MonoBehaviour
{
    public int MAX_PLAYER_NUM { get; set; }

    public enum WIN_LOSE
    {
        NODATA = -1,
        LOSE,     
        WIN      
    }

    public struct _GameResult
    {
        public WIN_LOSE winLose { get; set; }
        public int myKillScore { get; set; }
        public int myDeathScore { get; set; }
        public int myCaptureScore { get; set; }
    }

    public GameObject[] obj_players { get; set; }
    public Animator[] am_players { get; set; }
    public _GameResult[] gameResult;
}
