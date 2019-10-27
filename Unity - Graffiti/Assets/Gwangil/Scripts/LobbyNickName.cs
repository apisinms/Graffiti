using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyNickName : MonoBehaviour
{
    public TextMeshProUGUI txt_myNickname;

    // Start is called before the first frame update
    void Start()
    {
        txt_myNickname.text = "김민석";
    }
}
