using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class OtherPlayerManager : MonoBehaviour
{
    public static OtherPlayerManager instance;
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

        int j = 0;
        for (int i = 0; i < GameManager.instance.obj_players.Length; i++)
        {         
            if (GameManager.instance.myIndex != i)
            {
                obj_otherPlayers[j] = GameManager.instance.obj_players[i];
                animePlayer[j] = GameManager.instance.obj_players[i].GetComponent<Animator>();
                j++;
            }
        }

        /*
        for (int i = 0; i < animePlayer.Length; i++)
            animePlayer[i] = obj_otherPlayers[i].GetComponent<Animator>();
            */
        //이것도 서버에서 받아온 데이터로 초기화해야함.
        /* for(int i=0; i<NUM; i++)
        {
            attributeState[i] = _ATTRIBUTE_STATE.ALIVE;
            actionState[i] = _ACTION_STATE.IDLE;
        } */
    }

    private void Start()
    {
        
      // for (int i = 0; i < animePlayer.Length; i++)
          //  Debug.Log(animePlayer[i].name); 

    }
}
