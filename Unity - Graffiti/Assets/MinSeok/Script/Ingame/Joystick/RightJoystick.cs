using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RightJoystick : JoystickManager
{
    public override void Drag(BaseEventData _Data)
    {
        PointerEventData data = _Data as PointerEventData;
        Vector3 pos = data.position; //드래그 한곳의 위치.

        // 스틱 이동방향 추출 .(오른쪽,왼쪽,위,아래)
        joystick[1].stickDir = (pos - joystick[1].stickFirstPos).normalized;

        // 스틱의 처음 위치와 드래그중인 위치의 거리차를 구함
        float distance = Vector3.Distance(pos, joystick[1].stickFirstPos);

        // 이동가능범위 보다 작을때만 스틱의 이동. 아니면 최대범위만큼에서 고정.
        if (distance < joystick[1].maxMoveArea)
            img_joystick_stick.rectTransform.position = joystick[1].stickFirstPos + (joystick[1].stickDir * distance);
        else
            img_joystick_stick.rectTransform.position = joystick[1].stickFirstPos + (joystick[1].stickDir * joystick[1].maxMoveArea);
    }

    public override void DragStart()
    {
        Debug.Log("우측 드래그시작");
    }

    public override void DragEnd()
    {
        img_joystick_stick.transform.position = joystick[0].stickFirstPos;
        joystick[1].stickDir = Vector3.zero; // 방향을 0으로.
    }

}
