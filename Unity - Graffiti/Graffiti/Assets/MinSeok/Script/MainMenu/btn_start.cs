using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class btn_start : MonoBehaviour {

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

            string retMsg = NetworkManager.GetInstance.MayIStart();
            // 여기에서 매칭 호출
            if (NetworkManager.GetInstance.CheckStartFail() == true)
                Debug.Log(retMsg);

            if (NetworkManager.GetInstance.CheckStartSuccess() == true)
                SceneManager.LoadScene("SelectWeapons");

        }
		else if(flag == 1) //3초전에 취소눌렀을경우 원상복구
		{
			txt_startBtn.text = "매칭";
			am_loadingBar.SetBool("isStart", false);
			obj_loadingBar.SetActive(false);
		}
	}

    //void DelayMatching()
    //{
    //    SceneManager.LoadScene("SelectWeapons");
    //}
}
