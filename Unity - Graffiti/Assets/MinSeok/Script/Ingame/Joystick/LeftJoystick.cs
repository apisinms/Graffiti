using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LeftJoystick : JoystickManager
{

    // Update is called once per frame
    public void Update()
    {
        PlayerManager.instance.Move();
    }

    public override void Drag(BaseEventData _Data)
    {
        if (PlayerManager.instance.moveFlag == false)
            PlayerManager.instance.moveFlag = true;

        PointerEventData data = _Data as PointerEventData;
        Vector3 pos = data.position; //드래그 한곳의 위치.

        // 스틱 이동방향 추출 .(오른쪽,왼쪽,위,아래)
        joystick[0].stickDir = (pos - joystick[0].stickFirstPos).normalized;

        // playerDir = (stickDir.x * Vector3.right) + (stickDir.y * Vector3.forward); //동시에 플레이어의 이동방향결정
        PlayerManager.instance.myDirection = new Vector3(joystick[0].stickDir.x, 0, joystick[0].stickDir.y);

        // 스틱의 처음 위치와 드래그중인 위치의 거리차를 구함
        float distance = Vector3.Distance(pos, joystick[0].stickFirstPos);

        // 이동가능범위 보다 작을때만 스틱의 이동. 아니면 최대범위만큼에서 고정.
        if (distance < joystick[0].maxMoveArea)
            img_joystick_stick.rectTransform.position = joystick[0].stickFirstPos + (joystick[0].stickDir * distance);
        else
            img_joystick_stick.rectTransform.position = joystick[0].stickFirstPos + (joystick[0].stickDir * joystick[0].maxMoveArea);

        PlayerManager.instance.tmp = joystick[0].stickDir;

        //tf_player.transform.localRotation = Quaternion.Euler(0, Mathf.Atan2(stickDir.x, stickDir.y) * Mathf.Rad2Deg, 0);

        //tf_player.eulerAngles = new Vector3(0, Mathf.Atan2(playerDir.x, playerDir.y) * Mathf.Rad2Deg, 0); //플레이어의 방향을 바꿔줌.
    }

    public override void DragStart()
    {
        Debug.Log("좌측 드래그시작");
    }

    public override void DragEnd()
    {
        img_joystick_stick.transform.position = joystick[0].stickFirstPos;
        joystick[0].stickDir = Vector3.zero; // 방향을 0으로.
        PlayerManager.instance.moveFlag = false;
    }
}
