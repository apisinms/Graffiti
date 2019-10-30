using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureTriggerCollision : MonoBehaviour
{
    private int triggerIndex;
    private bool isTriggerProcessing;

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i<CaptureManager.instance.MAX_TERRITORY_NUM; i++)
        {
            if (this.transform.parent.CompareTag(CaptureManager.instance.territoryTag[i]))
            {
                triggerIndex = i; //점령지 5개각각의 인덱스를 태그로판별하여 초기화
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        int idx = 0;
        switch(other.tag)
        {
            case "Player1":
                idx = 0;
                break;
            case "Player2":
                idx = 1;
                break;
            case "Player3":
                idx = 2;
                break;
            case "Player4":
                idx = 3;
                break;
        }

        if(idx == GameManager.instance.playersIndex[0] || idx == GameManager.instance.playersIndex[1])
        {
            //점령지발판의 중복방지, 이미 우리가 점령한 지역이면 리턴
            if (isTriggerProcessing == true || CaptureManager.instance.captureResult_team[triggerIndex] == _CAPTURE_RESULT.GET)
                return;
        }
        else
        {
            if (isTriggerProcessing == true || CaptureManager.instance.captureResult_enemy[triggerIndex] == _CAPTURE_RESULT.GET)
                return;
        }

        isTriggerProcessing = true;

        Debug.Log("충돌");

        if (UIManager.instance.isStartCaptureCor[triggerIndex] != null)
            UIManager.instance.StopCoroutine(UIManager.instance.isStartCaptureCor[triggerIndex]);
        UIManager.instance.isStartCaptureCor[triggerIndex] = UIManager.instance.StartCoroutine(UIManager.instance.DecreaseCaptureGageImg(4.0f, triggerIndex, other.tag)); //내 점령지 인덱스를 넘김
    }

    private void OnTriggerExit(Collider other)
    {
        if (isTriggerProcessing == false) //점령지발판의 중복방지
            return;

        isTriggerProcessing = false;
        UIManager.instance.captureGage[triggerIndex].img_capture.fillAmount = 1;
        UIManager.instance.captureGage[triggerIndex].obj_parent.SetActive(false);
        UIManager.instance.StopCoroutine(UIManager.instance.isStartCaptureCor[triggerIndex]);
    }
}
