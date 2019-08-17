using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class RightJoystick : UnityEngine.MonoBehaviour, JoystickControll
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

    void Awake()
    {
        right_joystick.maxMoveArea = img_joystick_back.rectTransform.sizeDelta.y * 0.4f; //스틱이 움직일수있는 수평범위. ( * 0.5f면 정확히 조이스틱배경의 반지름만큼)
        right_joystick.stickFirstPos = img_joystick_stick.rectTransform.position;   
        // 캔버스 크기에대한 반지름 조절.
        //  float can = transform.parent.GetComponent<RectTransform>().localScale.x;
        //   moveArea *= can;
    }


    public void DragStart()
    {
        PlayerManager.instance.myActionState += (int)_ACTION_STATE.AIMING;
    }

    public void Drag(BaseEventData _Data)
    {
        PointerEventData data = _Data as PointerEventData;
        Vector3 pos = data.position; //드래그 한곳의 위치.

        // 스틱 이동방향 추출 .(오른쪽,왼쪽,위,아래)
        right_joystick.stickDir = (pos - right_joystick.stickFirstPos).normalized;

        PlayerManager.instance.myDirection = new Vector3(right_joystick.stickDir.x, 0, right_joystick.stickDir.y);


        // 스틱의 처음 위치와 드래그중인 위치의 거리차를 구함
        float distance = Vector3.Distance(pos, right_joystick.stickFirstPos);

        // 이동가능범위 보다 작을때만 스틱의 이동. 아니면 최대범위만큼에서 고정.
        if (distance < right_joystick.maxMoveArea)
            img_joystick_stick.rectTransform.position = right_joystick.stickFirstPos + (right_joystick.stickDir * distance);
        else
            img_joystick_stick.rectTransform.position = right_joystick.stickFirstPos + (right_joystick.stickDir * right_joystick.maxMoveArea);

    }

    public void DragEnd()
    {
        img_joystick_stick.transform.position = right_joystick.stickFirstPos;
        right_joystick.stickDir = Vector3.zero; // 방향을 0으로.
        PlayerManager.instance.myActionState -= _ACTION_STATE.AIMING;
    }

}
