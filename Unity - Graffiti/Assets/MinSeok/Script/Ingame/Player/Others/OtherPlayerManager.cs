using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class OtherPlayerManager : MonoBehaviour
{
    public static OtherPlayerManager instance;
    // public const int NUM = 3;
    public const int NUM = 3;

    public GameObject[] obj_otherPlayers { get; set; }
    public _ATTRIBUTE_STATE[] attributeState { get; set; }
    public float[] speed { get; set; }
    public float[] hp { get; set; }
    public float[] eulerAngle { get; set; }
    public float[] eulerAngle2 { get; set; }
    public Vector3[] direction { get; set; }

    void Awake()
    {
        if (instance == null)
            instance = this;

        Initialization(NUM);

    }


    void Initialization(int _num)
    {
        //서버에서 받은데이터로 초기화해야함.
        obj_otherPlayers = new GameObject[NUM];
        animePlayer = new Animator[NUM];
        speed = new float[NUM];
        hp = new float[NUM];
        eulerAngle = new float[NUM];
        eulerAngle2 = new float[NUM];
        direction = new Vector3[NUM];
        attributeState = new _ATTRIBUTE_STATE[NUM];
        actionState = new _ACTION_STATE[NUM];

        //자기번호를 제외한 플레이어 오브젝트에 나머지 3명을 등록.
        int j = 0;
        for (int i = 0; i < GameManager.instance.playersTag.Length; i++)
        {
            if (GameManager.instance.playersTag[i].Equals(GameManager.instance.myTag) == false)
            {
                obj_otherPlayers[j] = GameObject.FindGameObjectWithTag(GameManager.instance.playersTag[i]);
                j++;
            }
        }

        for (int i = 0; i < animePlayer.Length; i++)
            animePlayer[i] = obj_otherPlayers[i].GetComponent<Animator>();
    }

}