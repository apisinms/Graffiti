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

    private void Awake()
    {
        /*
        switch ((C_Global.GameType)GameManager.instance.gameInfo.gameType)
        {
            case C_Global.GameType._2vs2:
                {
                    MAX_PLAYER_NUM = 4;
                }
                break;

            case C_Global.GameType._1vs1:
                {
                    MAX_PLAYER_NUM = 2;
                }
                break;
        } */
        MAX_PLAYER_NUM = 4;

        obj_players = new GameObject[MAX_PLAYER_NUM];
        am_players = new Animator[MAX_PLAYER_NUM];
        gameResult = new _GameResult[MAX_PLAYER_NUM];

        for (int i=0; i<obj_players.Length; i++)
        {
            obj_players[i] = GameObject.FindGameObjectWithTag("Player" + (i + 1).ToString());
            am_players[i] = obj_players[i].GetComponent<Animator>();
            gameResult[i].winLose = WIN_LOSE.NODATA;
        }
    }

    private void Start()
    {
        //서버로부터 이긴자, 진자의 인덱스, 또는 이긴팀 진팀을 각각받음
        //각각의 킬스코어 데스스코어 캡처스코어를 받아서 저장.
        //public _GameResult[] gameResult;      4명분
        gameResult[0].winLose = WIN_LOSE.WIN;
        gameResult[1].winLose = WIN_LOSE.WIN;
        gameResult[2].winLose = WIN_LOSE.LOSE;
        gameResult[3].winLose = WIN_LOSE.LOSE;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (gameResult[0].winLose == WIN_LOSE.WIN && gameResult[1].winLose == WIN_LOSE.WIN)
            {
                am_players[0].SetTrigger("Win_1");
                am_players[1].SetTrigger("Win_2");

                am_players[2].SetTrigger("Lose_1");
                am_players[3].SetTrigger("Lose_2");
            }
            else if (gameResult[2].winLose == WIN_LOSE.WIN && gameResult[3].winLose == WIN_LOSE.WIN)
            {
                am_players[0].SetTrigger("Lose_1");
                am_players[1].SetTrigger("Lose_2");

                am_players[2].SetTrigger("Win_1");
                am_players[3].SetTrigger("Win_2");
            }
        }
    }
}
