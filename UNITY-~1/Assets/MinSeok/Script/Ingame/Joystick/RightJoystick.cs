﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class RightJoystick : MonoBehaviour, JoystickControll
{
    public Image img_joystick_back;
    public Image img_joystick_stick;

    protected struct _Joystick
    {
        public float maxMoveArea;    // 스틱이 움직일수 있는 범위.
        public Vector3 stickDir;        //스틱의 방향
        public Vector3 stickFirstPos; //스틱의 초기좌표 
    };

    protected _Joystick right_joystick; // 왼쪽 오른쪽 2개의 조이스틱
    private int myIndex;

    void Awake()
    {
        myIndex = GameManager.instance.myIndex;
        right_joystick.maxMoveArea = img_joystick_back.rectTransform.sizeDelta.y * 1.0f; //스틱이 움직일수있는 수평범위. ( * 0.5f면 정확히 조이스틱배경의 반지름만큼)
        right_joystick.stickFirstPos = img_joystick_stick.rectTransform.position;  
        
        // 캔버스 크기에대한 반지름 조절.
        float can = transform.parent.GetComponent<RectTransform>().localScale.x;
        right_joystick.maxMoveArea *= can;
    }

    public void DragStart()
    {
        //PlayersManager.instance.ApplyActionState(_ACTION_STATE.AIM, true);
        //PlayersManager.instance.StartMoveCoroutine();
    }

    public void Drag(BaseEventData _Data)
    {
        PointerEventData data = _Data as PointerEventData;
        Vector3 pos = data.position; //드래그 한곳의 위치.

        // 스틱 이동방향 추출 .(오른쪽,왼쪽,위,아래)
        right_joystick.stickDir = (pos - right_joystick.stickFirstPos).normalized;

        PlayersManager.instance.direction2[myIndex] = new Vector3(right_joystick.stickDir.x, 0, right_joystick.stickDir.y);
        //Debug.Log(Quaternion.ToEulerAngles(Quaternion.LookRotation(PlayersManager.instance.direction2[PlayersManager.instance.myIndex]))) ;

        // 스틱의 처음 위치와 드래그중인 위치의 거리차를 구함
        float distance = Vector3.Distance(pos, right_joystick.stickFirstPos);

        switch(GetJoystickStep(distance))
        {
            case 0:
                img_joystick_stick.transform.position = right_joystick.stickFirstPos;
                right_joystick.stickDir = Vector3.zero; // 방향을 0으로.
                PlayersManager.instance.ApplyActionState(_ACTION_STATE.AIM, false);
                break;
            case 1:
                img_joystick_stick.rectTransform.position = right_joystick.stickFirstPos + (right_joystick.stickDir * right_joystick.maxMoveArea * 0.4f);
                PlayersManager.instance.ApplyActionState(_ACTION_STATE.SHOT, false);
                PlayersManager.instance.ApplyActionState(_ACTION_STATE.AIM, true);
                break;
            case 2:
                img_joystick_stick.rectTransform.position = right_joystick.stickFirstPos + (right_joystick.stickDir * right_joystick.maxMoveArea);
                PlayersManager.instance.ApplyActionState(_ACTION_STATE.AIM, false);
                PlayersManager.instance.ApplyActionState(_ACTION_STATE.SHOT, true);
                break;
        }
  
    }
    
    public int GetJoystickStep(float _distance)
    {
        //0단계때
        if (_distance > 0 && _distance <= right_joystick.maxMoveArea * 0.4f)
            return 0;
        else if (_distance >= right_joystick.maxMoveArea * 0.4f && _distance <= right_joystick.maxMoveArea * 1.5f) //1단계 조준단계
            return 1;
        else if (_distance >= right_joystick.maxMoveArea * 1.5f) //2단계 발사단계
            return 2;

        return -1;
    }
    public void DragEnd()
    {
        img_joystick_stick.transform.position = right_joystick.stickFirstPos;
        right_joystick.stickDir = Vector3.zero; // 방향을 0으로.

        PlayersManager.instance.ApplyActionState(_ACTION_STATE.AIM, false);
        PlayersManager.instance.ApplyActionState(_ACTION_STATE.SHOT, false);
        // PlayersManager.instance.StopMoveCoroutine();
    }

}
