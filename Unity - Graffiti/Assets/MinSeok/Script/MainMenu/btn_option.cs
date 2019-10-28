using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class btn_option : UnityEngine.MonoBehaviour {

    void Awake()
    {
        GameObject.Find("UserNickName").GetComponent<TMPro.TMP_Text>().text = NetworkManager.instance.NickName;
    }

}
