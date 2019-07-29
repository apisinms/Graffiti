using System.Collections;
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

			NetworkManager.GetInstance.MayIMatch(); // 매칭할 수 있는가

			InvokeRepeating("CheckMatch", 0.01f, 0.01f);	// 1.5초에 한 번씩 매칭이 됐는지 검사한다. 

			//// 여기에서 매칭에 성공하면 
			//if (NetworkManager.GetInstance.MayIMatch() == true)
			//	SceneManager.LoadScene("SelectWeapons");

			//DelayMatching();
			//Invoke("DelayMatching", 3.0f); //3초뒤 무기선택으로 전환
		}
		else if(flag == 1) //3초전에 취소눌렀을경우 원상복구
		{
			/// 취소 버튼 눌렀을 때에 대한 처리를 서버로 전송해야한다.
			
			CancelInvoke();
			txt_startBtn.text = "매칭";
			am_loadingBar.SetBool("isStart", false);
			obj_loadingBar.SetActive(false);
		}
	}

	void CheckMatch()
	{
		// 매칭 안잡혔으면 그냥 빠져나간다.
		if (NetworkManager.GetInstance.CheckMatched() == false)
			return;

		// 매칭 성공하면 무기 선택 화면으로
		if (NetworkManager.GetInstance.CheckMatchSuccess() == true)
		{
			CancelInvoke("CheckMatch");
			SceneManager.LoadScene("SelectWeapons");
		}

		// 매칭 실패하면 
		else
		{
			// 원상 복구
			CancelInvoke("CheckMatch");
			txt_startBtn.text = "매칭";
			am_loadingBar.SetBool("isStart", false);
			obj_loadingBar.SetActive(false);
		}

	}

	void DelayMatching()
	{
		SceneManager.LoadScene("SelectWeapons");
	}

}
