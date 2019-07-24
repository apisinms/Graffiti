using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class btn_quit : MonoBehaviour
{
    public void BtnQuit() //로그인 패널에서 종료를 누르면
    {
        string retMsg = NetworkManager.GetInstance.MayILogout();

        if (NetworkManager.GetInstance.CheckLogoutFail() == true)
            Debug.Log(retMsg);

        if (NetworkManager.GetInstance.CheckLogoutSuccess() == true)
            SceneManager.LoadScene("SampleScene"); //첫화면 입장.
    }
}
