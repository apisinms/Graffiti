using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class LeftJoystick : MonoBehaviour, IJoystickControll
{
    public Image img_joystick_back;
    public Image img_joystick_stick;
	private static bool isLeftDrag;
    public static bool LeftTouch { get { return isLeftDrag; } }

    protected struct _Joystick
    {
        public float maxMoveArea;    // 스틱이 움직일수 있는 범위.
        public Vector3 stickDir;        //스틱의 방향
        public Vector3 stickFirstPos; //스틱의 초기좌표 
    };

    private _Joystick left_joystick; // 왼쪽 오른쪽 2개의 조이스틱
    private int myIndex;

    void Awake()
    {
        myIndex = GameManager.instance.myIndex;
        left_joystick.maxMoveArea = img_joystick_back.rectTransform.sizeDelta.y * 0.8f; //스틱이 움직일수있는 수평범위. ( * 0.5f면 정확히 조이스틱배경의 반지름만큼)
        left_joystick.stickFirstPos = img_joystick_stick.rectTransform.position;

        float can = transform.parent.GetComponent<RectTransform>().localScale.x; // 캔버스 크기에대한 반지름 조절.
        left_joystick.maxMoveArea *= can;

		isLeftDrag = false;
	}

    public void DragStart()
    {
        StateManager.instance.Circuit(true);

#if NETWORK
		// 처음 터치할 때만
		if (LeftJoystick.LeftTouch == false && RightJoystick.RightTouch == false)
			BridgeClientToServer.instance.StartMoveCoroutine();
#endif
        isLeftDrag = true;
    }

    public void Drag(BaseEventData _Data)
    {
        PointerEventData data = _Data as PointerEventData;
        Vector3 pos = data.position; //드래그 한곳의 위치.

        left_joystick.stickDir = (pos - left_joystick.stickFirstPos).normalized; // 스틱 이동방향 추출 .(오른쪽,왼쪽,위,아래)
        //Debug.Log(PlayersManager.instance.obj_players[PlayersManager.instance.myIndex].transform.eulerAngles.y);

        // playerDir = (stickDir.x * Vector3.right) + (stickDir.y * Vector3.forward); //동시에 플레이어의 이동방향결정
        PlayersManager.instance.direction[myIndex] = new Vector3(left_joystick.stickDir.x, 0, left_joystick.stickDir.y);

        float distance = Vector3.Distance(pos, left_joystick.stickFirstPos); // 스틱의 처음 위치와 드래그중인 위치의 거리차를 구함

        if (distance < left_joystick.maxMoveArea) // 이동가능범위 보다 작을때만 스틱의 이동. 아니면 최대범위만큼에서 고정.
        {
            img_joystick_stick.rectTransform.position = left_joystick.stickFirstPos + (left_joystick.stickDir * distance);
            //조이스틱 땡긴범위의 비율만큼 이동속도를 차등할것임.
            PlayersManager.instance.speed[myIndex] = (PlayersManager.instance.maxSpeed * distance / left_joystick.maxMoveArea);
        }
        else
        {
            img_joystick_stick.rectTransform.position = left_joystick.stickFirstPos + (left_joystick.stickDir * left_joystick.maxMoveArea);
            PlayersManager.instance.speed[myIndex] = PlayersManager.instance.maxSpeed; //조이스틱을 끝까지밀면 맥스스피드.
        }
        
        // PlayerManager.instance.tmp = joystick[0].stickDir;

        //tf_player.transform.localRotation = Quaternion.Euler(0, Mathf.Atan2(stickDir.x, stickDir.y) * Mathf.Rad2Deg, 0);
        //obj_myPlayer.transform.localRotation = Quaternion.LookRotation(myDirection);
        //tf_player.eulerAngles = new Vector3(0, Mathf.Atan2(playerDir.x, playerDir.y) * Mathf.Rad2Deg, 0); //플레이어의 방향을 바꿔줌.
    }

    public void DragEnd()
    {
        img_joystick_stick.transform.position = left_joystick.stickFirstPos;
        left_joystick.stickDir = Vector3.zero; // 방향을 0으로.
        StateManager.instance.Circuit(false);

        /*
          if (PlayersManager.instance.stateInfo[myIndex].actionState > (int)_ACTION_STATE.IDLE)
          {
              PlayersManager.instance.stateInfo[myIndex].actionState -= (int)_ACTION_STATE.CIRCUIT;
              img_joystick_stick.transform.position = left_joystick.stickFirstPos;
              left_joystick.stickDir = Vector3.zero; // 방향을 0으로.
          }
          */
        isLeftDrag = false;
#if NETWORK
      // 둘 다 터치 뗐을 때만
      if (LeftJoystick.LeftTouch == false && RightJoystick.RightTouch == false)
			BridgeClientToServer.instance.StopMoveCoroutine();
#endif
	}

	public void ResetDrag()
	{
		img_joystick_stick.transform.position = left_joystick.stickFirstPos;
		left_joystick.stickDir = Vector3.zero; // 방향을 0으로.
		StateManager.instance.Circuit(false);
	}
}
