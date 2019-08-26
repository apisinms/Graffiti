using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class LeftJoystick : MonoBehaviour, JoystickControll
{
    public Image img_joystick_back;
    public Image img_joystick_stick;

    protected struct _Joystick
    {
        public float maxMoveArea;    // 스틱이 움직일수 있는 범위.
        public Vector3 stickDir;        //스틱의 방향
        public Vector3 stickFirstPos; //스틱의 초기좌표 
    };

    private _Joystick left_joystick; // 왼쪽 오른쪽 2개의 조이스틱

    void Awake()
    {
        left_joystick.maxMoveArea = img_joystick_back.rectTransform.sizeDelta.y * 0.4f; //스틱이 움직일수있는 수평범위. ( * 0.5f면 정확히 조이스틱배경의 반지름만큼)
        left_joystick.stickFirstPos = img_joystick_stick.rectTransform.position;

          // 캔버스 크기에대한 반지름 조절.
        float can = transform.parent.GetComponent<RectTransform>().localScale.x;
        left_joystick.maxMoveArea *= can; 
    }

    public void DragStart()
    {
        PlayersManager.instance.actionState[PlayersManager.instance.myIndex] += (int)_ACTION_STATE.CIRCUIT;
    }

    public  void Drag(BaseEventData _Data)
    {
        PointerEventData data = _Data as PointerEventData;
        Vector3 pos = data.position; //드래그 한곳의 위치.

        // 스틱 이동방향 추출 .(오른쪽,왼쪽,위,아래)
        left_joystick.stickDir = (pos - left_joystick.stickFirstPos).normalized;

        // playerDir = (stickDir.x * Vector3.right) + (stickDir.y * Vector3.forward); //동시에 플레이어의 이동방향결정
        PlayersManager.instance.direction[PlayersManager.instance.myIndex] = new Vector3(left_joystick.stickDir.x, 0, left_joystick.stickDir.y);

        // 스틱의 처음 위치와 드래그중인 위치의 거리차를 구함
        float distance = Vector3.Distance(pos, left_joystick.stickFirstPos);

        // 이동가능범위 보다 작을때만 스틱의 이동. 아니면 최대범위만큼에서 고정.
        if (distance < left_joystick.maxMoveArea)
            img_joystick_stick.rectTransform.position = left_joystick.stickFirstPos + (left_joystick.stickDir * distance);
        else
            img_joystick_stick.rectTransform.position = left_joystick.stickFirstPos + (left_joystick.stickDir * left_joystick.maxMoveArea);

        // PlayerManager.instance.tmp = joystick[0].stickDir;

        //tf_player.transform.localRotation = Quaternion.Euler(0, Mathf.Atan2(stickDir.x, stickDir.y) * Mathf.Rad2Deg, 0);
        //obj_myPlayer.transform.localRotation = Quaternion.LookRotation(myDirection);
        //tf_player.eulerAngles = new Vector3(0, Mathf.Atan2(playerDir.x, playerDir.y) * Mathf.Rad2Deg, 0); //플레이어의 방향을 바꿔줌.
    }

    public  void DragEnd()
    {
        img_joystick_stick.transform.position = left_joystick.stickFirstPos;
        left_joystick.stickDir = Vector3.zero; // 방향을 0으로.
        PlayersManager.instance.actionState[PlayersManager.instance.myIndex] -= (int)_ACTION_STATE.CIRCUIT;
    }
}
