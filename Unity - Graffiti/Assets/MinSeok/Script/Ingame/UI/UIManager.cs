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
    public Vector3 hpAddPos { get; set; }
    public Coroutine  curHpCor { get; set; }

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
    public Vector3 nickAddPos { get; set; }

    public struct _TXT_NICKNAME_INFO
    {
        public GameObject obj_parent;
        public Text txt_nickname { get; set; }
    }
    #endregion

    #region CIRCLE
    public GameObject[] obj_prefebsCircle;
    public _IMG_CIRCLE_INFO myCircle;
    public _IMG_CIRCLE_INFO teamCircle;
    public _IMG_CIRCLE_INFO[] enemyCircle;
    public Vector3 circleAddPos { get; set; }

    public struct _IMG_CIRCLE_INFO
    {
        public GameObject obj_parent;
        public Image img_circle { get; set; }
    }
    #endregion

    #region LINE_AIMDIRECTION
    public GameObject obj_prefebLine;
    public _IMG_LINE_INFO myLine;
    public Vector3 lineAddPos { get; set; }

    public struct _IMG_LINE_INFO
    {
        public GameObject obj_parent;
        public Image img_aimDirectionLine { get; set; }
    }
    #endregion

    private void Awake()
    {
        if (instance == null)
            instance = this;

        nickname = new _TXT_NICKNAME_INFO[C_Global.MAX_PLAYER];
        enemyHP = new _IMG_HP_INFO[C_Global.MAX_PLAYER - 2];
        enemyCircle = new _IMG_CIRCLE_INFO[C_Global.MAX_PLAYER - 2];
        enemyIndex = new int[C_Global.MAX_PLAYER - 2];
        hpAddPos = new Vector3(0, 2.2f, 1.3f);
        nickAddPos = new Vector3(0, 2.0f, 0.9f);
        circleAddPos = new Vector3(0, 0.2f, 0);
        lineAddPos = new Vector3(0, 0.1f, 0);
   
        curHpCor = null;

        myIndex = GameManager.instance.myIndex;
        myTeamIndex = GameManager.instance.myTeamIndex;
        for (int i = 0; i < enemyIndex.Length; i++)
            enemyIndex[i] = GameManager.instance.enemyIndex[i];

        for (int i = 0; i < enemyHP.Length; i++)
        {
            enemyHP[i].obj_parent = Instantiate(obj_prefebsHP[2], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
            enemyHP[i].img_back = enemyHP[i].obj_parent.transform.GetChild(0).GetComponent<Image>();
            enemyHP[i].img_middle = enemyHP[i].obj_parent.transform.GetChild(1).GetComponent<Image>();
            enemyHP[i].img_front = enemyHP[i].obj_parent.transform.GetChild(2).GetComponent<Image>();
        }

        teamHP.obj_parent = Instantiate(obj_prefebsHP[1], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
        teamHP.img_back = teamHP.obj_parent.transform.GetChild(0).GetComponent<Image>();
        teamHP.img_middle = teamHP.obj_parent.transform.GetChild(1).GetComponent<Image>();
        teamHP.img_front = teamHP.obj_parent.transform.GetChild(2).GetComponent<Image>();

        myHP.obj_parent = Instantiate(obj_prefebsHP[0], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
        myHP.img_back = myHP.obj_parent.transform.GetChild(0).GetComponent<Image>();
        myHP.img_middle = myHP.obj_parent.transform.GetChild(1).GetComponent<Image>();
        myHP.img_front = myHP.obj_parent.transform.GetChild(2).GetComponent<Image>();

        for (int i = 0; i < nickname.Length; i++)
        {
            nickname[i].obj_parent = Instantiate(obj_prefebNickname, GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
            nickname[i].txt_nickname = nickname[i].obj_parent.transform.GetChild(0).GetComponent<Text>();
        }


        for (int i = 0; i < enemyCircle.Length; i++)
        {
            enemyCircle[i].obj_parent = Instantiate(obj_prefebsCircle[2], GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);
            enemyCircle[i].img_circle = enemyCircle[i].obj_parent.transform.GetChild(0).GetComponent<Image>();
        }

        teamCircle.obj_parent = Instantiate(obj_prefebsCircle[1], GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);
        teamCircle.img_circle = teamCircle.obj_parent.transform.GetChild(0).GetComponent<Image>();

        myCircle.obj_parent = Instantiate(obj_prefebsCircle[0], GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);
        myCircle.img_circle = myCircle.obj_parent.transform.GetChild(0).GetComponent<Image>();

        myLine.obj_parent = Instantiate(obj_prefebLine, GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);
        myLine.img_aimDirectionLine = myLine.obj_parent.transform.GetChild(0).GetComponent<Image>();
        myLine.obj_parent.SetActive(false);
    }

    private void Start()
    {
        //nickname[0].txt_nickname.text = "asdasd";
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0; i<nickname.Length; i++)
        {
            nickname[i].obj_parent.transform.position = PlayersManager.instance.tf_players[i].transform.position + hpAddPos;
        }

        myHP.obj_parent.transform.position = PlayersManager.instance.tf_players[myIndex].transform.position + nickAddPos;//Camera.main.WorldToScreenPoint(PlayersManager.instance.tf_players[myIndex].transform.position);
        teamHP.obj_parent.transform.position = PlayersManager.instance.tf_players[myTeamIndex].transform.position + nickAddPos;//Camera.main.WorldToScreenPoint(PlayersManager.instance.tf_players[myTeamIndex].transform.position);
        for (int i = 0; i < enemyIndex.Length; i++)
            enemyHP[i].obj_parent.transform.position = PlayersManager.instance.tf_players[enemyIndex[i]].transform.position + nickAddPos;//Camera.main.WorldToScreenPoint(PlayersManager.instance.tf_players[enemyIndex[i]].transform.position);

        myCircle.obj_parent.transform.position = PlayersManager.instance.tf_players[myIndex].transform.position + circleAddPos;
        teamCircle.obj_parent.transform.position = PlayersManager.instance.tf_players[myTeamIndex].transform.position + circleAddPos;//Camera.main.WorldToScreenPoint(PlayersManager.instance.tf_players[myTeamIndex].transform.position);
        for (int i = 0; i < enemyIndex.Length; i++)
            enemyCircle[i].obj_parent.transform.position = PlayersManager.instance.tf_players[enemyIndex[i]].transform.position + circleAddPos;

        myLine.obj_parent.transform.position = PlayersManager.instance.tf_players[myIndex].transform.position + lineAddPos;
    }


    public IEnumerator DecreaseMiddleHP(int _index, float _damage) //중간 체력 img는 효과를 적용할것이다.
    {
        //Debug.Log("들어옴");
        yield return YieldInstructionCache.WaitForSeconds(0.6f);
      
        switch(_index)
        {
            case 0: //내피
                myHP.img_middle.fillAmount -= _damage;
                break;
            case 1: //팀피
                teamHP.img_middle.fillAmount -= _damage;            
                break;
            case 2: //적1
                enemyHP[0].img_middle.fillAmount -= _damage;
                break;
            case 3: //적2
                enemyHP[1].img_middle.fillAmount -= _damage;
                break;
        }
     //   Debug.Log("나감");
        yield break;
    }
  
    public void UpdateAimDirectionImg(bool _value)
    {
        if (_value == false && myLine.obj_parent.activeSelf == true)
        {
            myLine.obj_parent.SetActive(false);
            return;
        }
        else if(_value == true && myLine.obj_parent.activeSelf == false)
            myLine.obj_parent.SetActive(true);

        Quaternion tmp = Quaternion.LookRotation(PlayersManager.instance.tf_players[myIndex].transform.forward);
        myLine.obj_parent.transform.localRotation = tmp;
        myLine.obj_parent.transform.eulerAngles = new Vector3(90, 0, myLine.obj_parent.transform.localRotation.eulerAngles.y * -1);
    }

	public void SetNickname(string _nickName, int _idx)
	{
		nickname[_idx].txt_nickname.text = _nickName;
	}
}
