using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public int myIndex { get; set; }
    public int[] playersIndex { get; set; }

    #region HP
    public GameObject[] obj_prefebsHP;   //0나, 1팀, 2적
    public _IMG_HP_INFO[] hp;
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

    #region DeadUI
    GameObject leftJoystick;
    GameObject rightJoystick;
    GameObject deadPanel;
    #endregion

    private void Awake()
    {
        if (instance == null)
            instance = this;

        myIndex = GameManager.instance.myIndex;

        playersIndex = new int[C_Global.MAX_PLAYER];
        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
            playersIndex[i] = GameManager.instance.playersIndex[i];

        Initialization_HP();
        Initialization_Nickname();
        Initialization_Circle();
        Initialization_Line();
    }

    private void Start()
    {
        leftJoystick = GameObject.Find("Left");
        rightJoystick = GameObject.Find("Right");

        GameObject.Find("Canvas_overlay").transform.Find("Panel_Dead").gameObject.SetActive(true);
        deadPanel = GameObject.Find("Panel_Dead");
        GameObject.Find("Canvas_overlay").transform.Find("Panel_Dead").gameObject.SetActive(false);
    }

    void Update()
    {
        for (int i=0; i<C_Global.MAX_PLAYER; i++)
        {
            nickname[playersIndex[i]].obj_parent.transform.position = PlayersManager.instance.tf_players[playersIndex[i]].transform.position + hpAddPos;
            hp[playersIndex[i]].obj_parent.transform.position = PlayersManager.instance.tf_players[playersIndex[i]].transform.position + nickAddPos;//Camera.main.WorldToScreenPoint(PlayersManager.instance.tf_players[enemyIndex[i]].transform.position);
            circle[playersIndex[i]].obj_parent.transform.position = PlayersManager.instance.tf_players[playersIndex[i]].transform.position + circleAddPos;
        }
        line.obj_parent.transform.position = PlayersManager.instance.tf_players[myIndex].transform.position + lineAddPos;
        /*
        for(int i=0; i<nickname.Length; i++)
            nickname[playersIndex[i]].obj_parent.transform.position = PlayersManager.instance.tf_players[playersIndex[i]].transform.position + hpAddPos;

        for (int i = 0; i < hp.Length; i++)
            hp[playersIndex[i]].obj_parent.transform.position = PlayersManager.instance.tf_players[playersIndex[i]].transform.position + nickAddPos;//Camera.main.WorldToScreenPoint(PlayersManager.instance.tf_players[enemyIndex[i]].transform.position);

        for(int i=0; i<circle.Length; i++)
            circle[playersIndex[i]].obj_parent.transform.position = PlayersManager.instance.tf_players[playersIndex[i]].transform.position + circleAddPos;
        */
    }

    private void Initialization_HP()
    {
        hp = new _IMG_HP_INFO[C_Global.MAX_PLAYER];
        hpAddPos = new Vector3(0, 2.2f, 1.3f);
        curHpCor = null;

        for (int i = 0; i < hp.Length; i++)
        {
            if (i == playersIndex[0]) // == myIndex
                hp[i].obj_parent = Instantiate(obj_prefebsHP[0], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);

            else if (i == playersIndex[1]) // == teamIndex
                hp[i].obj_parent = Instantiate(obj_prefebsHP[1], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);

            else if (i == playersIndex[2] || i == playersIndex[3]) // == enemyIndex[0]
                hp[i].obj_parent = Instantiate(obj_prefebsHP[2], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);

            hp[i].img_back = hp[i].obj_parent.transform.GetChild(0).GetComponent<Image>();
            hp[i].img_middle = hp[i].obj_parent.transform.GetChild(1).GetComponent<Image>();
            hp[i].img_front = hp[i].obj_parent.transform.GetChild(2).GetComponent<Image>();
        }
    }
    private void Initialization_Nickname()
    {
        nickname = new _TXT_NICKNAME_INFO[C_Global.MAX_PLAYER];
        nickAddPos = new Vector3(0, 2.0f, 0.9f);

        for (int i = 0; i < nickname.Length; i++)
        {
            nickname[i].obj_parent = Instantiate(obj_prefebNickname, GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
            nickname[i].txt_nickname = nickname[i].obj_parent.transform.GetChild(0).GetComponent<Text>();
        }
    }
    private void Initialization_Circle()
    {
        circle = new _IMG_CIRCLE_INFO[C_Global.MAX_PLAYER];
        circleAddPos = new Vector3(0, 0.2f, 0);

        for (int i = 0; i < circle.Length; i++)
        {
            if (i == playersIndex[0]) // == myIndex
                circle[i].obj_parent = Instantiate(obj_prefebsCircle[0], GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);

            else if (i == playersIndex[1]) // == teamIndex
                circle[i].obj_parent = Instantiate(obj_prefebsCircle[1], GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);

            else if (i == playersIndex[2] || i == playersIndex[3]) // == enemyIndex[0]
                circle[i].obj_parent = Instantiate(obj_prefebsCircle[2], GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);

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

    public IEnumerator DecreaseMiddleHP(int _index, float _curHP) //중간 체력 img는 효과를 적용할것이다.
    {
        //Debug.Log("들어옴");
        yield return YieldInstructionCache.WaitForSeconds(0.6f);

#if NETWORK
        hp[_index].img_middle.fillAmount = _curHP;
#else
        hp[_index].img_middle.fillAmount -= _curHP;
#endif
        yield break;
    }

    public void UpdateAimDirectionImg(bool _value)
    {
        if (_value == false && line.obj_parent.activeSelf == true)
        {
            line.obj_parent.SetActive(false);
            return;
        }
        else if(_value == true && line.obj_parent.activeSelf == false)
            line.obj_parent.SetActive(true);

        Quaternion tmp = Quaternion.LookRotation(PlayersManager.instance.tf_players[myIndex].transform.forward);
        line.obj_parent.transform.localRotation = tmp;
        line.obj_parent.transform.eulerAngles = new Vector3(90, 0, line.obj_parent.transform.localRotation.eulerAngles.y * -1);
    }

	public void SetNickname(string _nickname, int _index)
	{
		nickname[_index].txt_nickname.text = _nickname;
	}

    public int PlayerIndexToAbsoluteIndex(int _playerIndex)
    {
        int result = 0;
        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            if (_playerIndex == playersIndex[i])
            {
                result = playersIndex[i];

                break;
            }
        }

        return result;
    }

    public void OnPlayerUI(int _index)
    {
        hp[_index].obj_parent.SetActive(true);
        nickname[_index].obj_parent.SetActive(true);
        circle[_index].obj_parent.SetActive(true);
    }

    public void OffPlayerUI(int _index)
    {
        hp[_index].obj_parent.SetActive(false);
        nickname[_index].obj_parent.SetActive(false);
        circle[_index].obj_parent.SetActive(false);
    }

    public void SetDeadUI()
    {
        leftJoystick.SetActive(false);
        rightJoystick.SetActive(false);
        deadPanel.SetActive(true);
    }
    public void SetAliveUI()
    {
        leftJoystick.SetActive(true);
        rightJoystick.SetActive(true);
        deadPanel.SetActive(false);
    }

    public void HealthUIChanger(int _idx, float _health)
    {
        hp[_idx].img_front.fillAmount = _health * 0.01f;
        StartCoroutine(DecreaseMiddleHP(_idx, _health * 0.01f));
    }
}
