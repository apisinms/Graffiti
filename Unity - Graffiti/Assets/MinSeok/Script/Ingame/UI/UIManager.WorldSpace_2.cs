using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//                   UIManager_WorldSpace_2
public partial class UIManager : MonoBehaviour
{
    #region CIRCLE
    public GameObject[] obj_prefebsCircle;
    public _IMG_CIRCLE_INFO[] circle;
    public Vector3 circleAddPos { get; set; }

    public struct _IMG_CIRCLE_INFO
    {
        public GameObject obj_parent;
        public Image img_circle { get; set; }
    }
    #endregion

    #region LINE_AIMDIRECTION
    public GameObject obj_prefebLine;
    public _IMG_LINE_INFO line;
    public Vector3 lineAddPos { get; set; }

    public struct _IMG_LINE_INFO
    {
        public GameObject obj_parent;
        public Image img_aimDirectionLine { get; set; }
    }
    #endregion

    #region LINE_DIRECTION
    public GameObject obj_prefebLine2;
    public _IMG_LINE2_INFO line2;
    public Vector3 line2AddPos { get; set; }

    public struct _IMG_LINE2_INFO
    {
        public GameObject obj_parent;
        public Image img_directionLine { get; set; }
    }
    #endregion

    #region GRAFFITY
    public GameObject[] obj_prefebGraffity_2vs2;
    public GameObject[] obj_prefebGraffity_1vs1;
    public _TXT_GRAFFITY_INFO[] graffity;
    public Sprite[] spr_graffity;
    public Vector3[] graffityAddPos { get; set; }
    public Coroutine[] curSprayingCor { get; set; }
    public bool[] isStartSprayingCor { get; set; }

    public struct _TXT_GRAFFITY_INFO
    {
        public GameObject obj_parent;
        public Image img_graffity;
    }
    #endregion


    private void Initialization_Circle()
    {
        circle = new _IMG_CIRCLE_INFO[GameManager.instance.gameInfo.maxPlayer];
        circleAddPos = new Vector3(0, 0.2f, 0);

        for (int i = 0; i < circle.Length; i++)
        {
            if (i == playersIndex[0]) // == myIndex
            {
                circle[i].obj_parent = Instantiate(obj_prefebsCircle[0], GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);
            }

            switch (GameManager.instance.gameInfo.gameType)
            {
                case (int)C_Global.GameType._2vs2:
                    {
                        switch (GameManager.instance.gameInfo.maxPlayer)
                        {
                            case 2:
                                {
                                    if (i == playersIndex[1]) // == teamIndex
                                    {
                                        circle[i].obj_parent = Instantiate(obj_prefebsCircle[1], GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);
                                    }
                                }
                                break;

                            case 3:
                                {
                                    if (i == playersIndex[1]) // == teamIndex
                                    {
                                        circle[i].obj_parent = Instantiate(obj_prefebsCircle[1], GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);
                                    }

                                    else if (i == playersIndex[2]) // == enemyIndex[0]
                                    {
                                        circle[i].obj_parent = Instantiate(obj_prefebsCircle[2], GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);
                                    }
                                }
                                break;

                            case 4:
                                {
                                    if (i == playersIndex[1]) // == teamIndex
                                    {
                                        circle[i].obj_parent = Instantiate(obj_prefebsCircle[1], GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);
                                    }

                                    else if (i == playersIndex[2] || i == playersIndex[3]) // == enemyIndex[0]
                                    {
                                        circle[i].obj_parent = Instantiate(obj_prefebsCircle[2], GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);
                                    }
                                }
                                break;
                        }
                    }
                    break;

                case (int)C_Global.GameType._1vs1:
                    {
                        switch (GameManager.instance.gameInfo.maxPlayer)
                        {
                            case 2:
                                {
                                    if (i == playersIndex[1]) // == enemyIndex[0]
                                    {
                                        circle[i].obj_parent = Instantiate(obj_prefebsCircle[2], GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }

            circle[i].img_circle = circle[i].obj_parent.transform.GetChild(0).GetComponent<Image>();
        }
    }

    private void Initialization_Line()
    {
        lineAddPos = new Vector3(0, 0.1f, 0);

        line.obj_parent = Instantiate(obj_prefebLine, GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);
        line.img_aimDirectionLine = line.obj_parent.transform.GetChild(0).GetComponent<Image>();
        line.obj_parent.SetActive(false);
    }

    private void Initialization_Line2()
    {
        line2AddPos = new Vector3(0, 0.1f, 0);

        line2.obj_parent = Instantiate(obj_prefebLine2, GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);
        line2.img_directionLine = line.obj_parent.transform.GetChild(0).GetComponent<Image>();
        line2.obj_parent.SetActive(true);
    }

    private void Initialization_Graffity()
    {
        graffity = new _TXT_GRAFFITY_INFO[CaptureManager.instance.MAX_TERRITORY_NUM];
        curSprayingCor = new Coroutine[CaptureManager.instance.MAX_TERRITORY_NUM];
        isStartSprayingCor = new bool[CaptureManager.instance.MAX_TERRITORY_NUM];
        graffityAddPos = new Vector3[CaptureManager.instance.MAX_TERRITORY_NUM];

		/*// 할당먼저
		if (CaptureManager.instance.MAX_TERRITORY_NUM == 5)
			obj_prefebGraffity_2vs2 = new GameObject[CaptureManager.instance.MAX_TERRITORY_NUM];
		else if (CaptureManager.instance.MAX_TERRITORY_NUM == 3)
			obj_prefebGraffity_1vs1 = new GameObject[CaptureManager.instance.MAX_TERRITORY_NUM];

		// 오브젝트 셋팅
		GameObject territory = null;
		for (int i = 0; i < CaptureManager.instance.MAX_TERRITORY_NUM; i++, territory = null)
		{
			territory = GameObject.FindGameObjectWithTag("Territory" + (i + 1).ToString());

			if(territory != null)
			{
				if (CaptureManager.instance.MAX_TERRITORY_NUM == 5)
					obj_prefebGraffity_2vs2[i] = territory;
				else if (CaptureManager.instance.MAX_TERRITORY_NUM == 3)
					obj_prefebGraffity_1vs1[i] = territory;
			}
		}*/

        for (int i = 0; i < graffity.Length; i++)
        {
            curSprayingCor[i] = null;

			if (CaptureManager.instance.MAX_TERRITORY_NUM == 5)
				graffity[i].obj_parent = Instantiate(obj_prefebGraffity_2vs2[i], GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);
			else if (CaptureManager.instance.MAX_TERRITORY_NUM == 3)
				graffity[i].obj_parent = Instantiate(obj_prefebGraffity_1vs1[i], GameObject.FindGameObjectWithTag("Canvas_worldSpace2").gameObject.transform);

            graffity[i].img_graffity = graffity[i].obj_parent.transform.GetChild(0).GetComponent<Image>();
            graffity[i].obj_parent.SetActive(true);
        }
    }

    public void UpdateAimDirectionImg(bool _value)
    {
        if (_value == false && line.obj_parent.activeSelf == true)
        {
            line.obj_parent.SetActive(false);
            return;
        }
        else if (_value == true && line.obj_parent.activeSelf == false)
            line.obj_parent.SetActive(true);

        Quaternion tmp = Quaternion.LookRotation(PlayersManager.instance.tf_players[myIndex].transform.forward);
        line.obj_parent.transform.localRotation = tmp;
        line.obj_parent.transform.eulerAngles = new Vector3(90, 0, line.obj_parent.transform.localRotation.eulerAngles.y * -1);
    }

    public void UpdateDirectionImg(bool _value)
    {
        if (_value == false && line2.obj_parent.activeSelf == true)
        {
            line2.obj_parent.SetActive(false);
            return;
        }
        else if (_value == true && line.obj_parent.activeSelf == false)
            line2.obj_parent.SetActive(true);

        Quaternion tmp = Quaternion.LookRotation(PlayersManager.instance.tf_players[myIndex].transform.forward);
        line2.obj_parent.transform.localRotation = tmp;
        line2.obj_parent.transform.eulerAngles = new Vector3(90, 0, line2.obj_parent.transform.localRotation.eulerAngles.y * -1);
    }

    public void StartGraffitySpraying(int _triggerIdx, string _tag, bool _value)
    {
        if (_value == true)
        {
            if (isStartSprayingCor[_triggerIdx] == false)
            {
                curSprayingCor[_triggerIdx] = StartCoroutine(Cor_StartGraffitySpraying(
					GameManager.instance.gameInfo.mainSprayingTime,
					_triggerIdx, 
					_tag)); //내 점령지 인덱스를 넘김
            }
        }
        else
        {
            if (isStartSprayingCor[_triggerIdx] == true)
            {
                isStartSprayingCor[_triggerIdx] = false;
                graffity[_triggerIdx].img_graffity.fillAmount = 0;
                StopCoroutine(curSprayingCor[_triggerIdx]);
            }
        }
    }

    public IEnumerator Cor_StartGraffitySpraying(float _time, int _triggerIdx, string _playerTag)
    {
        if (isStartSprayingCor[_triggerIdx] == true)
            yield break; ;

        isStartSprayingCor[_triggerIdx] = true;

        graffity[_triggerIdx].img_graffity.fillAmount = 0;

        int idx = 0;
        switch (_playerTag)
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
                    if (idx == 0 || idx == 1)
                        graffity[_triggerIdx].img_graffity.sprite = spr_graffity[0];
                    else if(idx == 2 || idx == 3)
                        graffity[_triggerIdx].img_graffity.sprite = spr_graffity[1];
                    /*
                    if (idx == GameManager.instance.playersIndex[0] || idx == GameManager.instance.playersIndex[1])
                        graffity[_triggerIdx].img_graffity.color = Color.white;
                    else
                        graffity[_triggerIdx].img_graffity.color = Color.red;
                      */
                }
                break;

            case C_Global.GameType._1vs1:
                {
                    if (idx == 0)
                        graffity[_triggerIdx].img_graffity.sprite = spr_graffity[0];
                    else
                        graffity[_triggerIdx].img_graffity.sprite = spr_graffity[1];
                    /*
                    if (idx == GameManager.instance.playersIndex[0])
                        graffity[_triggerIdx].img_graffity.color = Color.white;
                    else
                        graffity[_triggerIdx].img_graffity.color = Color.red;
                        */
                }
                break;
        }

        while (true)
        {
            if (graffity[_triggerIdx].img_graffity.fillAmount >= 1)
            {
                graffity[_triggerIdx].img_graffity.fillAmount = 1.0f;
                isStartSprayingCor[_triggerIdx] = false;
                yield break;
            }

            graffity[_triggerIdx].img_graffity.fillAmount += Time.smoothDeltaTime / _time;
            yield return null;
        }
    }
}
