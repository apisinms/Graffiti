using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureTriggerCollision : MonoBehaviour
{
    private int triggerIndex; //나(트리거 자신)의 인덱스
    private bool isTriggerProcessing; //트리거엔 한명만 들어갈 수 있다.
    private int myIndex;
    private Coroutine curCheckIdleCor; //아이들인경우만 점령가능

    // Start is called before the first frame update
    void Start()
    {
        myIndex = GameManager.instance.myIndex;

        for (int i = 0; i < CaptureManager.instance.MAX_TERRITORY_NUM; i++)
        {
            if (this.transform.parent.CompareTag(CaptureManager.instance.territoryTag[i]))
            {
                triggerIndex = i; //점령지 5개각각의 인덱스를 태그로판별하여 초기화
                break;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        int idx = 0;
        switch (other.tag)
        {
            case "Player1":
                idx = 0;
                break;
            case "Player2":
                idx = 1;
                break;
            case "Player3":
                idx = 2;
                break;
            case "Player4":
                idx = 3;
                break;
        }

        switch ((C_Global.GameType)GameManager.instance.gameInfo.gameType)
        {
            case C_Global.GameType._2vs2:
                {
                    if (idx == GameManager.instance.playersIndex[0] || idx == GameManager.instance.playersIndex[1])
                    {
                        //점령지발판의 중복방지, 이미 우리가 점령한 지역이면 리턴
                        if (isTriggerProcessing == true || CaptureManager.instance.captureResult_team[triggerIndex] == _CAPTURE_RESULT.GET)
                            return;
                    }
                    else
                    {
                        if (isTriggerProcessing == true || CaptureManager.instance.captureResult_enemy[triggerIndex] == _CAPTURE_RESULT.GET)
                            return;
                    }
                }
                break;

            case C_Global.GameType._1vs1:
                {
                    if (idx == GameManager.instance.playersIndex[0])
                    {
                        //점령지발판의 중복방지, 이미 우리가 점령한 지역이면 리턴
                        if (isTriggerProcessing == true || CaptureManager.instance.captureResult_team[triggerIndex] == _CAPTURE_RESULT.GET)
                            return;
                    }
                    else
                    {
                        if (isTriggerProcessing == true || CaptureManager.instance.captureResult_enemy[triggerIndex] == _CAPTURE_RESULT.GET)
                            return;
                    }
                }
                break;
        }

        isTriggerProcessing = true;

        //들어온놈이 가만히 서있을때(아이들)를 검사할것임. 서있으면 점령시작 아니면 모든 점령 코루틴을 끔.
        curCheckIdleCor = StartCoroutine(Cor_CheckPlayerOnlyIdle(idx, other.tag));
    }


    IEnumerator Cor_CheckPlayerOnlyIdle(int _playerIdx, string _tag) //점령중인 플레이어 인덱스를 넘김
    {
        while (true)
        {
            switch ((C_Global.GameType)GameManager.instance.gameInfo.gameType)
            {
                case C_Global.GameType._2vs2:
                    {
                        //점령하는놈이 내기준 팀이냐 적이냐에따른. 이 체크코루틴은 점령이 끝나도 계속돌기때문에 점령했으면 멈춰야함.
                        if (_playerIdx == GameManager.instance.playersIndex[0] || _playerIdx == GameManager.instance.playersIndex[1])
                        {
                            //점령지발판의 중복방지, 이미 우리가 점령한 지역이면 리턴, 또는 적이 점령을 완료했으나 그 적이 다시 점령하려하면 리턴
                            if (CaptureManager.instance.captureResult_team[triggerIndex] == _CAPTURE_RESULT.GET)
                            {
                                isTriggerProcessing = false;
                                yield break;
                            }
                        }
                        else
                        {
                            if (CaptureManager.instance.captureResult_enemy[triggerIndex] == _CAPTURE_RESULT.GET)
                            {
                                isTriggerProcessing = false;
                                yield break;
                            }
                        }
                    }
                    break;

                case C_Global.GameType._1vs1:
                    {
                        //점령하는놈이 내기준 팀이냐 적이냐에따른. 이 체크코루틴은 점령이 끝나도 계속돌기때문에 점령했으면 멈춰야함.
                        if (_playerIdx == GameManager.instance.playersIndex[0])
                        {
                            //점령지발판의 중복방지, 이미 우리가 점령한 지역이면 리턴, 또는 적이 점령을 완료했으나 그 적이 다시 점령하려하면 리턴
                            if (CaptureManager.instance.captureResult_team[triggerIndex] == _CAPTURE_RESULT.GET)
                            {
                                isTriggerProcessing = false;
                                yield break;
                            }
                        }
                        else
                        {
                            if (CaptureManager.instance.captureResult_enemy[triggerIndex] == _CAPTURE_RESULT.GET)
                            {
                                isTriggerProcessing = false;
                                yield break;
                            }
                        }
                    }
                    break;
            }

            if (_playerIdx == myIndex) //점령시도가 "나"면
            {
                if (PlayersManager.instance.actionState[myIndex] == _ACTION_STATE.IDLE ||
                    PlayersManager.instance.actionState[myIndex] == _ACTION_STATE.SPRAY)
                {
                    UIManager.instance.DecreaseCaptureGageSubImg(triggerIndex, _tag, true);
                }
                else //조금이라도 움직이면 아이들이 아니므로 코루틴을 멈춤.
                {
                    UIManager.instance.DecreaseCaptureGageSubImg(triggerIndex, _tag, false);
                    UIManager.instance.DecreaseCaptureGageImg(triggerIndex, _tag, false);
                    UIManager.instance.StartGraffitySpraying(triggerIndex, _tag, false);

                    if (PlayersManager.instance.actionState[myIndex] == _ACTION_STATE.DEATH)
                    {
                        isTriggerProcessing = false;
                        yield break;
                    }
                }
            }
            else //다른애들이면 스테이트를 받아와야함
            {
                //아이들이면 서브게이지 부터채우고, 다채워지면 메인 점령게이지를 발동.
                _ACTION_STATE tmpState = (_ACTION_STATE)NetworkManager.instance.GetActionState(_playerIdx);
                if (tmpState == _ACTION_STATE.IDLE || tmpState == _ACTION_STATE.SPRAY)
                {
                    UIManager.instance.DecreaseCaptureGageSubImg(triggerIndex, _tag, true);
                }
                else //조금이라도 움직이면 아이들이 아니므로 코루틴을 멈춤.
                {
                    UIManager.instance.DecreaseCaptureGageSubImg(triggerIndex, _tag, false);
                    UIManager.instance.DecreaseCaptureGageImg(triggerIndex, _tag, false);
                    UIManager.instance.StartGraffitySpraying(triggerIndex, _tag, false);

                    if (tmpState == _ACTION_STATE.DEATH)
                    {
                        isTriggerProcessing = false;
                        yield break;
                    }
                }
            }

            yield return null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        int idx = 0;
        switch (other.tag)
        {
            case "Player1":
                idx = 0;
                break;
            case "Player2":
                idx = 1;
                break;
            case "Player3":
                idx = 2;
                break;
            case "Player4":
                idx = 3;
                break;
        }

        if (idx == myIndex) //아이들일때는 트리거 밖으로 나갈수가 없음
        {
            if (isTriggerProcessing == false || PlayersManager.instance.actionState[myIndex] == _ACTION_STATE.IDLE)
                return;
        }
        else
        {
            _ACTION_STATE tmpState = (_ACTION_STATE)NetworkManager.instance.GetActionState(idx);
            if (isTriggerProcessing == false || tmpState == _ACTION_STATE.IDLE)
                return;
        }

        isTriggerProcessing = false;

        StopCoroutine(curCheckIdleCor);
        UIManager.instance.DecreaseCaptureGageImg(triggerIndex, other.tag, false);
        UIManager.instance.DecreaseCaptureGageSubImg(triggerIndex, other.tag, false);
        UIManager.instance.StartGraffitySpraying(triggerIndex, other.tag, false);
    }
}