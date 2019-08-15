using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class JoystickManager : MonoBehaviour
{
    public Image img_joystick_back;
    public Image img_joystick_stick;

    protected struct _Joystick
    {
        public float maxMoveArea;    // 스틱이 움직일수 있는 범위.
        public Vector3 stickDir;        //스틱의 방향
        public Vector3 stickFirstPos; //스틱의 초기좌표 
    };

    protected _Joystick[] joystick = new _Joystick[2]; // 왼쪽 오른쪽 2개의 조이스틱

    void Awake()
    {
        for(int i=0; i<joystick.Length; i++)
        {
            joystick[i].maxMoveArea = img_joystick_back.rectTransform.sizeDelta.y * 0.4f; //스틱이 움직일수있는 수평범위. ( * 0.5f면 정확히 조이스틱배경의 반지름만큼)
            joystick[i].stickFirstPos = img_joystick_stick.rectTransform.position;
        }
        // 캔버스 크기에대한 반지름 조절.
        //  float can = transform.parent.GetComponent<RectTransform>().localScale.x;
        //   moveArea *= can;

    }
    public abstract void DragStart();
    public abstract void Drag(BaseEventData _Data);
    public abstract void DragEnd();
}
