using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class btn_start : MonoBehaviour {

    public GameObject obj_loadingBar;
    private Animator am_loadingBar;
    public Text txt_startBtn;
    private int matchingFlag = 1;

    void Start()
    {
        am_loadingBar = obj_loadingBar.GetComponent<Animator>();
    }

    public void BtnStart() //매칭버튼 눌렀을때.
    {
        matchingFlag = 1 - matchingFlag;

        if (matchingFlag == 0)
        {
            txt_startBtn.text = "취소";
            obj_loadingBar.SetActive(true);
            am_loadingBar.SetBool("isStart", true);

            Invoke("DelayMatching", 3.0f); //3초뒤 무기선택으로 전환
        }
        else if(matchingFlag == 1) //3초전에 취소눌렀을경우 원상복구
        {
            CancelInvoke();
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
