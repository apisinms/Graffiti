using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public int myIndex { get; set; }

    #region HP 
    public GameObject[] obj_prefebsHP;   //0나, 1팀, 2적
    public _IMG_HP_INFO myHP;
    public _IMG_HP_INFO teamHP;
    public _IMG_HP_INFO[] enemyHP;

    public struct _IMG_HP_INFO //체력바는 맨뒤 중간 맨앞 이미지 3개가 겹쳐진 구성임.
    {
        public GameObject obj_parent;
        public Image img_back { get; set; }
        public Image img_middle { get; set; }
        public Image img_front { get; set; }
    }
    #endregion

    private void Awake()
    {
        if (instance == null)
            instance = this;

        myIndex = GameManager.instance.myIndex;
        enemyHP = new _IMG_HP_INFO[2];

        myHP.obj_parent = Instantiate(obj_prefebsHP[0], GameObject.FindGameObjectWithTag("Canvas").transform);
        myHP.img_back = myHP.obj_parent.transform.GetChild(0).GetComponent<Image>();
        myHP.img_middle = myHP.obj_parent.transform.GetChild(1).GetComponent<Image>();
        myHP.img_front = myHP.obj_parent.transform.GetChild(2).GetComponent<Image>();

        teamHP.obj_parent = Instantiate(obj_prefebsHP[1], GameObject.FindGameObjectWithTag("Canvas").transform);
        teamHP.img_back = teamHP.obj_parent.transform.GetChild(0).GetComponent<Image>();
        teamHP.img_middle = teamHP.obj_parent.transform.GetChild(1).GetComponent<Image>();
        teamHP.img_front = teamHP.obj_parent.transform.GetChild(2).GetComponent<Image>();

        for (int i = 0; i < enemyHP.Length; i++)
        {
            enemyHP[i].obj_parent = Instantiate(obj_prefebsHP[2], GameObject.FindGameObjectWithTag("Canvas").transform);
            enemyHP[i].img_back = enemyHP[i].obj_parent.transform.GetChild(0).GetComponent<Image>();
            enemyHP[i].img_middle = enemyHP[i].obj_parent.transform.GetChild(1).GetComponent<Image>();
            enemyHP[i].img_front = enemyHP[i].obj_parent.transform.GetChild(2).GetComponent<Image>();
        }
    }

    private void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        //img.transform.position = Camera.main.WorldToScreenPoint(PlayersManager.instance.tf_players[PlayersManager.instance.myIndex].transform.position);
    }
}
