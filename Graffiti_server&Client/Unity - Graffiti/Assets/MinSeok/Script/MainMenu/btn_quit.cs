using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class btn_quit : MonoBehaviour
{

    public void BtnQuit() //로그인 패널에서 종료를 누르면
    {
		NetworkManager.instance.Disconnect();
        Application.Quit();
    }
}
