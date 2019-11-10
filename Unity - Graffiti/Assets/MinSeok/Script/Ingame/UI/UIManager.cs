using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class UIManager : MonoBehaviour
{
	public static UIManager instance;
	public int myIndex { get; set; }
	public int[] playersIndex { get; set; }

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

        Initialization_ReadyCount();
        Initialization_GameTimer();
        Initialization_Score();
        Initialization_KillDeath();
        Initialization_Marker();
        Initialization_HP();
		Initialization_Nickname();
		Initialization_Circle();
		Initialization_Line();
        Initialization_Line2();
        Initialization_ReloadGage();
        Initialization_CaptureGage();
        Initialization_KillLog();
        Initialization_RespawnGage();
        Initialization_Graffity();
        Initialization_Grenade();

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

        StartCoroutine(Cor_CheckKillLogQueue()); //킬로그검사 큐를 계속돌림

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

        for (int i = 0; i < CaptureManager.instance.MAX_TERRITORY_NUM; i++)
        {
            captureGage[i].obj_parent.transform.position = CaptureManager.instance.obj_territory[i].transform.position + captureAddPos;
            captureGageSub[i].obj_parent.transform.position = CaptureManager.instance.obj_territory[i].transform.position + captureAddPos;
            //graffity[i].obj_parent.transform.position = CaptureManager.instance.obj_territory[i].transform.position;// + graffityAddPos[3];
        }

        line.obj_parent.transform.position = PlayersManager.instance.tf_players[myIndex].transform.position + lineAddPos;
        line2.obj_parent.transform.position = PlayersManager.instance.tf_players[myIndex].transform.position + line2AddPos;
        marker.obj_parent.transform.position = PlayersManager.instance.tf_players[myIndex].transform.position + markerAddPos;
        weaponInfo.obj_parent.transform.position = PlayersManager.instance.tf_players[myIndex].transform.position + weaponAddPos;
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
        StartCoroutine(Cor_DecreaseRespawnGageImg(GameManager.instance.gameInfo.respawnTime)); //리스폰 유아이 활성화
        //deadPanel.SetActive(true);
    }

    public void SetAliveUI()
    {
        leftJoystick.SetActive(true);
        rightJoystick.SetActive(true);
        reloadBtn.SetActive(true);
        //deadPanel.SetActive(false);
    }
}
