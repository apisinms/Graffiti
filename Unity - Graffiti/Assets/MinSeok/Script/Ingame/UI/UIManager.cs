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
	public Coroutine curHpCor { get; set; }

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

	#region RELOAD_GAGE
	public GameObject obj_prefebReloadGage;
	public _IMG_RELOAD_INFO[] reloadGage;
	public Vector3 reloadAddPos { get; set; }

	public struct _IMG_RELOAD_INFO
	{
		public GameObject obj_parent;
		public Image img_reload { get; set; }
		public Image img_back { get; set; }
		public Text txt_reloadTime { get; set; }
	}
	#endregion

	#region WEAPON_INFO
	public GameObject obj_prefebWeaponInfo;
	public Sprite[] spr_mainW;
	public _IMG_WEAPON_INFO weaponInfo;
	public Vector3 weaponAddPos { get; set; }

	public struct _IMG_WEAPON_INFO
	{
		public GameObject obj_parent;
		public Image img_mainW { get; set; }
		public Text txt_ammoState { get; set; }
	}
    #endregion

    #region RESPAWN_GAGE
    public GameObject obj_prefebRespawnGage;
    public _IMG_RESPAWN_INFO respawn;
    private bool isStartRespawnGageCor;

    public struct _IMG_RESPAWN_INFO
    {
        public GameObject obj_parent;
        public Image img_respawnGage { get; set; }
        public Text txt_deathFrom { get; set; }
    }
    #endregion

    #region DeadUI
    GameObject leftJoystick;
	GameObject rightJoystick;
	GameObject deadPanel;
    GameObject reloadBtn;
    #endregion

    private void Awake()
	{
		if (instance == null)
			instance = this;

		myIndex = GameManager.instance.myIndex;

        playersIndex = new int[GameManager.instance.gameInfo.maxPlayer];
        for (int i = 0; i < GameManager.instance.gameInfo.maxPlayer; i++)
            playersIndex[i] = GameManager.instance.playersIndex[i];

        Initialization_HP();
		Initialization_Nickname();
		Initialization_Circle();
		Initialization_Line();
		Initialization_ReloadGage();
        Initialization_RespawnGage();

        Initialization_WeaponInfo_1();
#if !NETWORK
        Initialization_WeaponInfo_2();
#endif
    }

	private void Start()
	{
		leftJoystick = GameObject.Find("Left");
		rightJoystick = GameObject.Find("Right");
        reloadBtn = GameObject.Find("obj_reloadBtn");

        //GameObject.Find("Canvas_overlay").transform.Find("Panel_Dead").gameObject.SetActive(true);
        //deadPanel = GameObject.Find("Panel_Dead");
        //GameObject.Find("Canvas_overlay").transform.Find("Panel_Dead").gameObject.SetActive(false);
    }

	void Update()
	{
		for (int i = 0; i < GameManager.instance.gameInfo.maxPlayer; i++)
		{
			nickname[playersIndex[i]].obj_parent.transform.position = PlayersManager.instance.tf_players[playersIndex[i]].transform.position + hpAddPos;
			hp[playersIndex[i]].obj_parent.transform.position = PlayersManager.instance.tf_players[playersIndex[i]].transform.position + nickAddPos;//Camera.main.WorldToScreenPoint(PlayersManager.instance.tf_players[enemyIndex[i]].transform.position);
			circle[playersIndex[i]].obj_parent.transform.position = PlayersManager.instance.tf_players[playersIndex[i]].transform.position + circleAddPos;
            reloadGage[i].obj_parent.transform.position = PlayersManager.instance.tf_players[i].transform.position + reloadAddPos;
        }
		line.obj_parent.transform.position = PlayersManager.instance.tf_players[myIndex].transform.position + lineAddPos;		
		weaponInfo.obj_parent.transform.position = PlayersManager.instance.tf_players[myIndex].transform.position + weaponAddPos;
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
        hp = new _IMG_HP_INFO[GameManager.instance.gameInfo.maxPlayer];
        hpAddPos = new Vector3(0, 2.1f, 1.3f);
        curHpCor = null;

        for (int i = 0; i < hp.Length; i++)
        {
            if (i == playersIndex[0]) // == myIndex
            {
                hp[i].obj_parent = Instantiate(obj_prefebsHP[0], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
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
                                        hp[i].obj_parent = Instantiate(obj_prefebsHP[1], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
                                    }

                                }
                                break;

                            case 3:
                                {
                                    if (i == playersIndex[1]) // == teamIndex
                                    {
                                        hp[i].obj_parent = Instantiate(obj_prefebsHP[1], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
                                    }

                                    else if (i == playersIndex[2]) // == enemyIndex[0]
                                    {
                                        hp[i].obj_parent = Instantiate(obj_prefebsHP[2], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
                                    }
                                }
                                break;

                            case 4:
                                {
                                    if (i == playersIndex[1]) // == teamIndex
                                    {
                                        hp[i].obj_parent = Instantiate(obj_prefebsHP[1], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
                                    }

                                    else if (i == playersIndex[2] || i == playersIndex[3]) // == enemyIndex[0]
                                    {
                                        hp[i].obj_parent = Instantiate(obj_prefebsHP[2], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
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
                                    if (i == playersIndex[1]) // == enemy
                                    {
                                        hp[i].obj_parent = Instantiate(obj_prefebsHP[2], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }

            hp[i].img_back = hp[i].obj_parent.transform.GetChild(0).GetComponent<Image>();
            hp[i].img_middle = hp[i].obj_parent.transform.GetChild(1).GetComponent<Image>();
            hp[i].img_front = hp[i].obj_parent.transform.GetChild(2).GetComponent<Image>();
        }
    }

    private void Initialization_Nickname()
	{
		nickname = new _TXT_NICKNAME_INFO[GameManager.instance.gameInfo.maxPlayer];
		nickAddPos = new Vector3(0, 2.0f, 0.9f);

		for (int i = 0; i < nickname.Length; i++)
		{
			nickname[i].obj_parent = Instantiate(obj_prefebNickname, GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
			nickname[i].txt_nickname = nickname[i].obj_parent.transform.GetChild(0).GetComponent<Text>();
		}
	}

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

    private void Initialization_ReloadGage()
    {
        reloadGage = new _IMG_RELOAD_INFO[GameManager.instance.gameInfo.maxPlayer];
        reloadAddPos = new Vector3(0, 2.5f, 2.2f);

        for(int i=0; i<reloadGage.Length; i++)
        {
            reloadGage[i].obj_parent = Instantiate(obj_prefebReloadGage, GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
            reloadGage[i].img_reload = reloadGage[i].obj_parent.transform.GetChild(0).GetComponent<Image>();
            reloadGage[i].img_back = reloadGage[i].obj_parent.transform.GetChild(1).GetComponent<Image>();
            reloadGage[i].txt_reloadTime = reloadGage[i].obj_parent.transform.GetChild(2).GetComponent<Text>();
            reloadGage[i].obj_parent.SetActive(false);
        }
    }

    private void Initialization_Line()
	{
		lineAddPos = new Vector3(0, 0.1f, 0);

		line.obj_parent = Instantiate(obj_prefebLine, GameObject.FindGameObjectWithTag("Canvas_worldSpace2").transform);
		line.img_aimDirectionLine = line.obj_parent.transform.GetChild(0).GetComponent<Image>();
		line.obj_parent.SetActive(false);
	}

    private void Initialization_RespawnGage()
    {
        respawn.obj_parent = GameObject.FindGameObjectWithTag("Canvas_overlay").transform.GetChild(6).gameObject;
        respawn.img_respawnGage = respawn.obj_parent.transform.GetChild(2).GetComponent<Image>();
        respawn.txt_deathFrom = respawn.obj_parent.transform.GetChild(3).GetComponent<Text>();
        line.obj_parent.SetActive(false);
    }

    public void Initialization_WeaponInfo_1()
	{
		weaponAddPos = new Vector3(0.3f, 1.8f, -1.85f);

		weaponInfo.obj_parent = Instantiate(obj_prefebWeaponInfo, GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
		weaponInfo.img_mainW = weaponInfo.obj_parent.transform.GetChild(1).GetComponent<Image>();
		weaponInfo.txt_ammoState = weaponInfo.obj_parent.transform.GetChild(2).GetComponent<Text>();
	}

    public void Initialization_WeaponInfo_2()
    {
#if NETWORK
        switch (WeaponManager.instance.mainWeapon[myIndex])
        {
            case _WEAPONS.AR:
                weaponInfo.img_mainW.sprite = spr_mainW[0];
                weaponInfo.txt_ammoState.text = WeaponManager.instance.weaponInfoAR.maxAmmo.ToString();//weaponInfoAR.maxAmmo.ToString();
                break;
            case _WEAPONS.SG:
                weaponInfo.img_mainW.sprite = spr_mainW[1];
                weaponInfo.txt_ammoState.text = WeaponManager.instance.weaponInfoSG.maxAmmo.ToString();//WeaponManager.instance.weaponInfoSG.maxAmmo.ToString();
                break;
            case _WEAPONS.SMG:
                weaponInfo.img_mainW.sprite = spr_mainW[2];
                weaponInfo.txt_ammoState.text = WeaponManager.instance.weaponInfoSMG.maxAmmo.ToString();// WeaponManager.instance.weaponInfoSMG.maxAmmo.ToString();
                break;
        }
#else
        switch (WeaponManager.instance.mainWeapon[myIndex])
        {
            case _WEAPONS.AR:
                weaponInfo.img_mainW.sprite = spr_mainW[0];
                weaponInfo.txt_ammoState.text = 21.ToString();//weaponInfoAR.maxAmmo.ToString();
                break;
            case _WEAPONS.SG:
                weaponInfo.img_mainW.sprite = spr_mainW[1];
                weaponInfo.txt_ammoState.text = 2.ToString();//WeaponManager.instance.weaponInfoSG.maxAmmo.ToString();
                break;
            case _WEAPONS.SMG:
                weaponInfo.img_mainW.sprite = spr_mainW[2];
                weaponInfo.txt_ammoState.text = 17.ToString();// WeaponManager.instance.weaponInfoSMG.maxAmmo.ToString();
                break;
        }
#endif
    }

    public IEnumerator DecreaseMiddleHPImg(int _index, float _curHP) //중간 체력 img는 효과를 적용할것이다.
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

    public void ReloadAmmo_Btn() //재장전 이미지 버튼
    {
        WeaponManager.instance.ReloadAmmo(myIndex); //총알개수 체크후 바로 다이렉트 재장전

#if NETWORK
        NetworkManager.instance.SendIngamePacket();
#endif   
    }

    public void SetAmmoStateTxt(int _num)
	{
		weaponInfo.txt_ammoState.text = _num.ToString();
	}

	public IEnumerator DecreaseReloadGageImg(float _time, int _index)
	{
		reloadGage[_index].obj_parent.SetActive(true);
		reloadGage[_index].txt_reloadTime.text = (_time).ToString();

		while (true)
		{
			if (reloadGage[_index].img_reload.fillAmount <= 0)
			{
				reloadGage[_index].img_reload.fillAmount = 1.0f;
				reloadGage[_index].obj_parent.SetActive(false);
                BridgeClientToServer.instance.isStartReloadGageCor[_index] = false; //중복실행방지
				yield break;
			}

			reloadGage[_index].img_reload.fillAmount -= Time.smoothDeltaTime / _time;
			reloadGage[_index].txt_reloadTime.text = (reloadGage[_index].img_reload.fillAmount * _time).ToString();
			yield return null;
		}
	}


    public IEnumerator DecreaseRespawnGageImg(float _time)
    {
        if (isStartRespawnGageCor == true)
            yield break;

        isStartRespawnGageCor = true;
        respawn.obj_parent.SetActive(true);

        while (true)
        {
            if (respawn.img_respawnGage.fillAmount <= 0)
            {
                respawn.img_respawnGage.fillAmount = 1.0f;
                respawn.obj_parent.SetActive(false);
                isStartRespawnGageCor = false;
                yield break;
            }

            respawn.img_respawnGage.fillAmount -= Time.smoothDeltaTime / _time;
            yield return null;
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

	public void SetNickname(string _nickname, int _index)
	{
		nickname[_index].txt_nickname.text = _nickname;
	}

	public int PlayerIndexToAbsoluteIndex(int _playerIndex)
	{
		int result = 0;
		for (int i = 0; i < GameManager.instance.gameInfo.maxPlayer; i++)
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
        reloadGage[_index].obj_parent.SetActive(false);
    }

    public void SetDeadUI(string _value)
    {
        // 조이스틱 위치 제자리로
        leftJoystick.GetComponent<LeftJoystick>().ResetDrag();
        rightJoystick.GetComponent<RightJoystick>().ResetDrag();

        leftJoystick.SetActive(false);
        rightJoystick.SetActive(false);
        reloadBtn.SetActive(false);

        respawn.txt_deathFrom.text = _value;
        StartCoroutine(DecreaseRespawnGageImg(GameManager.instance.gameInfo.respawnTime)); //리스폰 유아이 활성화
        //deadPanel.SetActive(true);
    }

    public void SetAliveUI()
    {
        leftJoystick.SetActive(true);
        rightJoystick.SetActive(true);
        reloadBtn.SetActive(true);
        //deadPanel.SetActive(false);
    }

    public void HealthUIChanger(int _idx, float _health)
	{
		hp[_idx].img_front.fillAmount = _health * 0.01f;
		StartCoroutine(DecreaseMiddleHPImg(_idx, _health * 0.01f));
	}
}