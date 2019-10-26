using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class RightJoystick : MonoBehaviour, IJoystickControll
{
    public static RightJoystick instance;
    public Image img_joystick_back;
    public Image img_joystick_stick;
	private static bool isRightDrag;
    public static bool RightTouch { get { return isRightDrag; } }

    public struct _Joystick
    {
        public float maxMoveArea;    // 스틱이 움직일수 있는 범위.
        public Vector3 stickDir;        //스틱의 방향
        public Vector3 stickFirstPos; //스틱의 초기좌표 
        public Vector3 touchPos;
    };

    private bool isStep0, isStep1, isStep2;
    public _Joystick right_joystick; // 왼쪽 오른쪽 2개의 조이스틱
    private int myIndex;

    void Awake()
    {
        if (instance == null)
            instance = this;
       
        myIndex = GameManager.instance.myIndex;
        right_joystick.maxMoveArea = img_joystick_back.rectTransform.sizeDelta.y * 0.8f; //스틱이 움직일수있는 수평범위. ( * 0.5f면 정확히 조이스틱배경의 반지름만큼)
        right_joystick.stickFirstPos = img_joystick_stick.rectTransform.position;
        
        float can = transform.parent.GetComponent<RectTransform>().localScale.x;  // 캔버스 크기에대한 반지름 조절.
        right_joystick.maxMoveArea *= can;

        isRightDrag = false;
    }

    public void DragStart()
    {
#if NETWORK
		// 처음 터치할 때만
		if (LeftJoystick.LeftTouch == false && RightJoystick.RightTouch == false)
			BridgeClientToServer.instance.StartMoveCoroutine();
#endif
        isRightDrag = true;
    }

    private void Update()
    {
        //Debug.Log(PlayersManager.instance.actionState[myIndex]);
        //Debug.Log(PlayersManager.instance.direction2[myIndex]);
    }
    public void Drag(BaseEventData _Data)
    {
        if (PlayersManager.instance.actionState[myIndex] == _ACTION_STATE.DEATH)
            return;

        PointerEventData data = _Data as PointerEventData;
        right_joystick.touchPos = data.position; //드래그 한곳의 위치.

        right_joystick.stickDir = (right_joystick.touchPos - right_joystick.stickFirstPos).normalized; // 스틱 이동방향 추출 .(오른쪽,왼쪽,위,아래)
        PlayersManager.instance.direction2[myIndex] = new Vector3(right_joystick.stickDir.x, 0, right_joystick.stickDir.y);
        //Debug.Log(Quaternion.ToEulerAngles(Quaternion.LookRotation(PlayersManager.instance.direction2[PlayersManager.instance.myIndex]))) ;

        float distance = Vector3.Distance(right_joystick.touchPos, right_joystick.stickFirstPos); // 스틱의 처음 위치와 드래그중인 위치의 거리차를 구함

        switch (GetJoystickStep(distance))
        {
            case 0:
                if (isStep0 == false)
                {
                    StateManager.instance.Shot(false);
                    isStep0 = true;

                    #if NETWORK
                    NetworkManager.instance.SendIngamePacket();
                    #endif
                }
                isStep1 = false; isStep2 = false;

                img_joystick_stick.transform.position = right_joystick.stickFirstPos;
                right_joystick.stickDir = Vector3.zero; // 방향을 0으로.
                break;

            case 1:
                if (isStep1 == false)
                {
                    StateManager.instance.Aim(true);
                    StateManager.instance.Shot(false);
                    isStep1 = true;

#if NETWORK
					NetworkManager.instance.SendIngamePacket();
#endif
				}
                isStep0 = false; isStep2 = false;

                img_joystick_stick.rectTransform.position = right_joystick.stickFirstPos + (right_joystick.stickDir * right_joystick.maxMoveArea * 0.5f);
                break;

            case 2:
                if (isStep2 == false)
                {
                    StateManager.instance.Shot(true);
                    isStep2 = true;

#if NETWORK
					NetworkManager.instance.SendIngamePacket();
#endif
				}
                isStep0 = false; isStep1 = false;

                img_joystick_stick.rectTransform.position = right_joystick.stickFirstPos + (right_joystick.stickDir * right_joystick.maxMoveArea);
                break;
        }
  
    }
    
    public int GetJoystickStep(float _distance)
    {
        if (_distance >= 0 && _distance <= right_joystick.maxMoveArea * 0.5f) //0단계때
            return 0;
        else if (_distance >= right_joystick.maxMoveArea * 0.5f && _distance <= right_joystick.maxMoveArea * 1.2f) //1단계 조준단계
            return 1;
        else if (_distance >= right_joystick.maxMoveArea * 1.2f) //2단계 발사단계
            return 2;

        return -1;
    }
    public void DragEnd()
    {
        StateManager.instance.Aim(false);
        img_joystick_stick.transform.position = right_joystick.stickFirstPos;
        right_joystick.stickDir = Vector3.zero; // 방향을 0으로.
        isStep0 = true; isStep1 = false; isStep2 = false;

        isRightDrag = false;

#if NETWORK
      // 둘 다 터치 뗐을 때만
      if (LeftJoystick.LeftTouch == false && RightJoystick.RightTouch == false)
         BridgeClientToServer.instance.StopMoveCoroutine();
#endif
    }

	public void ResetDrag()
	{
		StateManager.instance.Aim(false);
		img_joystick_stick.transform.position = right_joystick.stickFirstPos;
		right_joystick.stickDir = Vector3.zero; // 방향을 0으로.
		isStep0 = true; isStep1 = false; isStep2 = false;
	}

}
