using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class btn_option : UnityEngine.MonoBehaviour {

    void Awake()
    {
		NetworkManager.instance.selectMatch = (int)C_Global.GameType._2vs2;	// 초기 선택은 2:2로

        GameObject.Find("UserNickName").GetComponent<TMPro.TMP_Text>().text = NetworkManager.instance.NickName;
    }

}
