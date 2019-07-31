using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class btn_quit : MonoBehaviour {

    public void BtnQuit() //로그인 패널에서 종료를 누르면
    {
        //클로즈 소켓 이후 클라이언트 종료
        Application.Quit();
    }
}
