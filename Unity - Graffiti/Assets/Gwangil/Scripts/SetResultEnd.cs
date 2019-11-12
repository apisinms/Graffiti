using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using KetosGames.SceneTransition;

public class SetResultEnd : MonoBehaviour
{
    public enum GAME_RESSULT
    {
        DRAW = -1,
        REDWIN,
        BLUEWIN
    }

    public TMP_Text resultRed;
    public TMP_Text resultBlue;

    public CountScore redScore;
    public CountScore blueScore;

    public GameObject[] player;
    public TMP_Text[] nickName;
    public TMP_Text[] killCount;
    public TMP_Text[] deathCount;
    public TMP_Text[] captureCount;
    public Animator[] playersAnim;
    public Animator[] am_sprEffect;

    private GAME_RESSULT result;
    int checkGameMode = 1;

    void Awake()
    {
        redScore.target = 0;
        blueScore.target = 0;

        am_sprEffect[0].transform.parent.gameObject.SetActive(false);
        am_sprEffect[2].transform.parent.gameObject.SetActive(false);

        if ((int)C_Global.GameType._1vs1 == EndSceneManager.Instance.gameType)
        {
            checkGameMode = 0;
        }

        for (int i = 0; i < EndSceneManager.Instance.playerNum.Length; i++)
        {
            nickName[EndSceneManager.Instance.playerNum[i] - checkGameMode].text =
                EndSceneManager.Instance.nickName[EndSceneManager.Instance.playerNum[i] - 1];

            killCount[EndSceneManager.Instance.playerNum[i] - checkGameMode].text =
                EndSceneManager.Instance.scores[EndSceneManager.Instance.playerNum[i] - 1].numOfKill.ToString();

            deathCount[EndSceneManager.Instance.playerNum[i] - checkGameMode].text =
                EndSceneManager.Instance.scores[EndSceneManager.Instance.playerNum[i] - 1].numOfDeath.ToString();

            captureCount[EndSceneManager.Instance.playerNum[i] - checkGameMode].text =
                EndSceneManager.Instance.scores[EndSceneManager.Instance.playerNum[i] - 1].captureCount.ToString();

            player[EndSceneManager.Instance.playerNum[i] - checkGameMode].SetActive(true);
            switch (EndSceneManager.Instance.gameType)
            {
                case (int)C_Global.GameType._1vs1:
                    if (EndSceneManager.Instance.playerNum[i] == 1)
                    {
                        redScore.target += EndSceneManager.Instance.scores[EndSceneManager.Instance.playerNum[i] - 1].killScore;
                        redScore.target += EndSceneManager.Instance.scores[EndSceneManager.Instance.playerNum[i] - 1].captureScore;
                    }
                    else
                    {
                        blueScore.target += EndSceneManager.Instance.scores[EndSceneManager.Instance.playerNum[i] - 1].killScore;
                        blueScore.target += EndSceneManager.Instance.scores[EndSceneManager.Instance.playerNum[i] - 1].captureScore;
                    }
                    break;
                case (int)C_Global.GameType._2vs2:
                    if (EndSceneManager.Instance.playerNum[i] <= 2)
                    {
                        redScore.target += EndSceneManager.Instance.scores[EndSceneManager.Instance.playerNum[i] - 1].killScore;
                        redScore.target += EndSceneManager.Instance.scores[EndSceneManager.Instance.playerNum[i] - 1].captureScore;
                    }
                    else
                    {
                        blueScore.target += EndSceneManager.Instance.scores[EndSceneManager.Instance.playerNum[i] - 1].killScore;
                        blueScore.target += EndSceneManager.Instance.scores[EndSceneManager.Instance.playerNum[i] - 1].captureScore;
                    }

                    break;
            }
        }


        // 2초후 ActiveTrue
        Invoke("SetResult", 2.0f);
    }

    public void SetResult()
    {
        if (redScore.target == blueScore.target)
        {
            result = GAME_RESSULT.DRAW;
        }

        switch (EndSceneManager.Instance.gameType)
        {
            case (int)C_Global.GameType._1vs1:
                if (EndSceneManager.Instance.playerNum.Length == 1)
                {
                    if (EndSceneManager.Instance.playerNum[0] == 1 )
                    {
                        result = GAME_RESSULT.REDWIN;                      
                    }
                    else if (EndSceneManager.Instance.playerNum[0] == 2)
                    {
                        result = GAME_RESSULT.BLUEWIN;
                    }
                }
                break;
            case (int)C_Global.GameType._2vs2:
                if (EndSceneManager.Instance.playerNum.Length == 2)
                {
                    if (EndSceneManager.Instance.playerNum[0] == 1 && EndSceneManager.Instance.playerNum[1] == 2)
                    {
                        result = GAME_RESSULT.REDWIN;
                    }
                    else if (EndSceneManager.Instance.playerNum[0] == 3 && EndSceneManager.Instance.playerNum[1] == 4)
                    {
                        result = GAME_RESSULT.BLUEWIN;
                    }
                }
                break;
        }


        if (redScore.target > blueScore.target)
        {
            result = GAME_RESSULT.REDWIN;
        }
        else if (redScore.target < blueScore.target)
        {
            result = GAME_RESSULT.BLUEWIN;
        }



        switch(result)
        {
            case GAME_RESSULT.REDWIN:
                {
                    Debug.Log(EndSceneManager.Instance.myIndex);

                    switch (EndSceneManager.Instance.gameType)
                    {
                        case (int)C_Global.GameType._1vs1:
                            {
                                if (EndSceneManager.Instance.myIndex == 0)
                                    AudioManager.Instance.Play(0);
                                else
                                    AudioManager.Instance.Play(1);
                            }
                            break;
                        case (int)C_Global.GameType._2vs2:
                            {
                                if (EndSceneManager.Instance.myIndex == 0 || EndSceneManager.Instance.myIndex == 1)
                                    AudioManager.Instance.Play(0);
                                else if (EndSceneManager.Instance.myIndex == 2 || EndSceneManager.Instance.myIndex == 3)
                                    AudioManager.Instance.Play(1);
                            }
                            break;
                    }

                    resultRed.text = "Win!!";
                    resultBlue.text = "Lose";

                    playersAnim[0].SetTrigger("Win_1");
                    playersAnim[1].SetTrigger("Win_2");
                    playersAnim[2].SetTrigger("Lose_1");
                    playersAnim[3].SetTrigger("Lose_2");

                    am_sprEffect[0].transform.parent.gameObject.SetActive(true);
                    am_sprEffect[2].transform.parent.gameObject.SetActive(true);
                    am_sprEffect[0].SetTrigger("Win_1");
                    am_sprEffect[1].SetTrigger("Win_1");
                    am_sprEffect[2].SetTrigger("Lose_1");
                    am_sprEffect[3].SetTrigger("Lose_1");
                }
                break;

            case GAME_RESSULT.DRAW:
                {
                    resultRed.text = "Draw";
                    resultBlue.text = "Draw";

                    playersAnim[0].SetTrigger("Lose_1");
                    playersAnim[1].SetTrigger("Lose_1");
                    playersAnim[2].SetTrigger("Lose_1");
                    playersAnim[3].SetTrigger("Lose_1");
                }
                break;

            case GAME_RESSULT.BLUEWIN:
                {
                    Debug.Log(EndSceneManager.Instance.myIndex);

                    switch (EndSceneManager.Instance.gameType)
                    {
                        case (int)C_Global.GameType._1vs1:
                            {
                                if (EndSceneManager.Instance.myIndex == 1)
                                    AudioManager.Instance.Play(0);
                                else
                                    AudioManager.Instance.Play(1);
                            }
                            break;
                        case (int)C_Global.GameType._2vs2:
                            {
                                if (EndSceneManager.Instance.myIndex == 0 || EndSceneManager.Instance.myIndex == 1)
                                    AudioManager.Instance.Play(1);
                                else if (EndSceneManager.Instance.myIndex == 2 || EndSceneManager.Instance.myIndex == 3)
                                    AudioManager.Instance.Play(0);
                            }
                            break;
                    }

                    resultRed.text = "Lose!!";
                    resultBlue.text = "Win";

                    playersAnim[0].SetTrigger("Lose_1");
                    playersAnim[1].SetTrigger("Lose_2");
                    playersAnim[2].SetTrigger("Win_1");
                    playersAnim[3].SetTrigger("Win_2");

                    am_sprEffect[0].transform.parent.gameObject.SetActive(true);
                    am_sprEffect[2].transform.parent.gameObject.SetActive(true);
                    am_sprEffect[0].SetTrigger("Lose_1");
                    am_sprEffect[1].SetTrigger("Lose_1");
                    am_sprEffect[2].SetTrigger("Win_1");
                    am_sprEffect[3].SetTrigger("Win_1");
                }
                break;
        }
        //결과를 표시하기 위한 작업
        resultRed.gameObject.SetActive(true);
        resultBlue.gameObject.SetActive(true);
    }

    public void PressNextBtn()
    {
        if(EndSceneManager.Instance != null)
        {
            EndSceneManager.Instance.GoToLobby();
        }
        SceneLoader.LoadScene("Lobby");
    }
}
