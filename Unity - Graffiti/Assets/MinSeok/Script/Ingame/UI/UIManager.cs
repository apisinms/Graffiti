using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public int myIndex { get; set; }
    public int myTeamIndex { get; set; }
    public int[] enemyIndex { get; set; }

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

    #region NICKNAME
    public GameObject obj_prefebNickname;
    public _TXT_NICKNAME_INFO[] nickname;

    public struct _TXT_NICKNAME_INFO
    {
        public GameObject obj_parent;
        public Text txt_nickname { get; set; }
    }
    #endregion

    private void Awake()
    {
        if (instance == null)
            instance = this;

        nickname = new _TXT_NICKNAME_INFO[C_Global.MAX_PLAYER];
        enemyHP = new _IMG_HP_INFO[C_Global.MAX_PLAYER - 2];
        enemyIndex = new int[C_Global.MAX_PLAYER - 2];

        myIndex = GameManager.instance.myIndex;
        myTeamIndex = PlayersManager.instance.myTeamIndex;
        for (int i = 0; i < enemyIndex.Length; i++)
            enemyIndex[i] = PlayersManager.instance.enemyIndex[i];

        for (int i = 0; i < enemyHP.Length; i++)
        {
            enemyHP[i].obj_parent = Instantiate(obj_prefebsHP[2], GameObject.FindGameObjectWithTag("Canvas").transform);
            enemyHP[i].img_back = enemyHP[i].obj_parent.transform.GetChild(0).GetComponent<Image>();
            enemyHP[i].img_middle = enemyHP[i].obj_parent.transform.GetChild(1).GetComponent<Image>();
            enemyHP[i].img_front = enemyHP[i].obj_parent.transform.GetChild(2).GetComponent<Image>();
        }

        teamHP.obj_parent = Instantiate(obj_prefebsHP[1], GameObject.FindGameObjectWithTag("Canvas").transform);
        teamHP.img_back = teamHP.obj_parent.transform.GetChild(0).GetComponent<Image>();
        teamHP.img_middle = teamHP.obj_parent.transform.GetChild(1).GetComponent<Image>();
        teamHP.img_front = teamHP.obj_parent.transform.GetChild(2).GetComponent<Image>();

        myHP.obj_parent = Instantiate(obj_prefebsHP[0], GameObject.FindGameObjectWithTag("Canvas").transform);
        myHP.img_back = myHP.obj_parent.transform.GetChild(0).GetComponent<Image>();
        myHP.img_middle = myHP.obj_parent.transform.GetChild(1).GetComponent<Image>();
        myHP.img_front = myHP.obj_parent.transform.GetChild(2).GetComponent<Image>();

        for (int i = 0; i < nickname.Length; i++)
        {
            nickname[i].obj_parent = Instantiate(obj_prefebNickname, GameObject.FindGameObjectWithTag("Canvas").transform);
            //nickname[i].txt_nickname.text = PlayersManager.instance.nickname[i] + "입니다.";
        }
    }
    private void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0; i<nickname.Length; i++)
        {
            nickname[i].obj_parent.transform.position = PlayersManager.instance.tf_players[i].transform.position + new Vector3(0, 2.2f, 1.3f);
        }

        myHP.obj_parent.transform.position = PlayersManager.instance.tf_players[myIndex].transform.position + new Vector3(0, 2.0f, 0.9f);//Camera.main.WorldToScreenPoint(PlayersManager.instance.tf_players[myIndex].transform.position);
        teamHP.obj_parent.transform.position = PlayersManager.instance.tf_players[myTeamIndex].transform.position + new Vector3(0, 2.0f, 0.9f);//Camera.main.WorldToScreenPoint(PlayersManager.instance.tf_players[myTeamIndex].transform.position);
        for (int i = 0; i < enemyIndex.Length; i++)
            enemyHP[i].obj_parent.transform.position = PlayersManager.instance.tf_players[enemyIndex[i]].transform.position + new Vector3(0, 2.0f, 0.9f);//Camera.main.WorldToScreenPoint(PlayersManager.instance.tf_players[enemyIndex[i]].transform.position);
    }
}
