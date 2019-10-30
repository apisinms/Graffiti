using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum _CAPTURE_RESULT //중립, 획득, 뺏김
{
    N = -1, 
    GET,
    LOSE,
}

public class CaptureManager : MonoBehaviour
{
    public static CaptureManager instance;
    public int MAX_TERRITORY_NUM = 5;

    public GameObject[] obj_territory { get; set; }
    public string[] territoryTag { get; set; }
    public _CAPTURE_RESULT[] captureResult_team; //우리팀 기준에서의 점령지 결과
    public _CAPTURE_RESULT[] captureResult_enemy; //적팀 기준에서의 점령지 결과

    #region OUTLINE_AND_HALO
    public Outline[] territoryOutline;
    #endregion

    private void Awake()
    {
        if (instance == null)
            instance = this;
      
        obj_territory = new GameObject[MAX_TERRITORY_NUM];
        territoryTag = new string[MAX_TERRITORY_NUM];
        captureResult_team = new _CAPTURE_RESULT[MAX_TERRITORY_NUM];
        captureResult_enemy = new _CAPTURE_RESULT[MAX_TERRITORY_NUM];

        for (int i = 0; i < territoryTag.Length; i++)
        {
            territoryTag[i] = "Territory" + (i + 1).ToString();
            obj_territory[i] = GameObject.FindGameObjectWithTag(territoryTag[i]);
            captureResult_team[i] = _CAPTURE_RESULT.N;
            captureResult_enemy[i] = _CAPTURE_RESULT.N;
        }
    }

    private void Start()
    {
        Initialization_Outline();
    }

    private void Initialization_Outline()
    {
        territoryOutline = new Outline[MAX_TERRITORY_NUM];

        for(int i=0; i<MAX_TERRITORY_NUM; i++)
            territoryOutline[i] = obj_territory[i].transform.GetChild(0).GetComponent<Outline>();
    }
}
