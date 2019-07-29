﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class btn_start : MonoBehaviour
{

	public GameObject obj_loadingBar;
	Animator am_loadingBar;
	public Text txt_startBtn;
	int flag = 1;

	void Start()
	{
		am_loadingBar = obj_loadingBar.GetComponent<Animator>();

		
	}

	public void BtnStart() //매칭버튼 눌렀을때.
	{
		flag = 1 - flag;

		if (flag == 0)
		{
			txt_startBtn.text = "취소";
			obj_loadingBar.SetActive(true);
			am_loadingBar.SetBool("isStart", true);

			// 매칭이 가능한지 서버로 전송한다.
			NetworkManager.instance.MayIMatch();

			InvokeRepeating("CheckMatch", 0.01f, 0.01f);

			//DelayMatching();
			//Invoke("DelayMatching", 3.0f); //3초뒤 무기선택으로 전환
		}
		else if(flag == 1) //3초전에 취소눌렀을경우 원상복구
		{
			CancelInvoke();
			txt_startBtn.text = "매칭";
			am_loadingBar.SetBool("isStart", false);
			obj_loadingBar.SetActive(false);
		}
	}

	void CheckMatch()
	{
		// 아직 매칭이 안됐으면 그냥 나온다.
		if (NetworkManager.instance.CheckMatched() == false)
			return;

		// 매칭에 성공했다면 반복호출되던 Invoke를 취소하고 무기선택화면으로 넘어간다.
		if (NetworkManager.instance.CheckMatchSuccess() == true)
			SceneManager.LoadScene("SelectWeapons");

		// 매칭에 실패했다면 원상복구한다.
		else
		{
			txt_startBtn.text = "매칭";
			am_loadingBar.SetBool("isStart", false);
			obj_loadingBar.SetActive(false);
		}

		CancelInvoke("CheckMatch");
	}

	//void DelayMatching()
	//{
	//	SceneManager.LoadScene("SelectWeapons");
	//}
}
