using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBtn : MonoBehaviour
{
    public void ExitButtonPress()
    {
        AudioManager.Instance.Play(0); //클릭음
        NetworkManager.instance.Disconnect();
        Application.Quit();
    }
}
