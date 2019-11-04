using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SetResultEnd : MonoBehaviour
{
    public TMP_Text resultRed;
    public TMP_Text resultBlue;

    void Start()
    {
        // 2초후 ActiveTrue
        Invoke("SetResult", 2.0f);
    }

    public void SetResult()
    {
        //결과를 표시하기 위한 작업


        // ActiveTrue
        resultRed.gameObject.SetActive(true);
        resultBlue.gameObject.SetActive(true);
    }
}
