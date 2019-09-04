using System.Collections;
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
        myIndex = PlayersManager.instance.myIndex;
        right_joystick.maxMoveArea = img_joystick_back.rectTransform.sizeDelta.y * 1.0f; //스틱이 움직일수있는 수평범위. ( * 0.5f면 정확히 조이스틱배경의 반지름만큼)
        right_joystick.stickFirstPos = img_joystick_stick.rectTransform.position;  
        
        // 캔버스 크기에대한 반지름 조절.
        float can = transform.parent.GetComponent<RectTransform>().localScale.x;
        right_joystick.maxMoveArea *= can;
    }

    public void DragStart()
    {
        PlayersManager.instance.actionState[myIndex] += (int)_ACTION_STATE.AIMING;
       // PlayersManager.instance.StartMoveCoroutine();
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
 
        // 이동가능범위 보다 작을때만 스틱의 이동. 아니면 최대범위만큼에서 고정.
        if (distance >= 0 && distance <= right_joystick.maxMoveArea * 0.4f)
        {
            img_joystick_stick.transform.position = right_joystick.stickFirstPos;
            right_joystick.stickDir = Vector3.zero; // 방향을 0으로.
        }
        else if (distance >= right_joystick.maxMoveArea * 0.4f && distance <= right_joystick.maxMoveArea * 1.5f)
            img_joystick_stick.rectTransform.position = right_joystick.stickFirstPos + (right_joystick.stickDir * right_joystick.maxMoveArea * 0.4f);
        else if (distance >= right_joystick.maxMoveArea * 1.5f)
            img_joystick_stick.rectTransform.position = right_joystick.stickFirstPos + (right_joystick.stickDir * right_joystick.maxMoveArea);
  
    }

    public void DragEnd()
    {
        img_joystick_stick.transform.position = right_joystick.stickFirstPos;
        right_joystick.stickDir = Vector3.zero; // 방향을 0으로.
        PlayersManager.instance.actionState[myIndex] -= _ACTION_STATE.AIMING;
       // PlayersManager.instance.StopMoveCoroutine();
    }

}
