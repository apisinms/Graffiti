using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickManager : MonoBehaviour
{
    public AnimatorManager animator { get; set; }
    public Image img_joystick_back;
    public Image img_joystick_stick;

    public GameObject player;


    private Transform tf_player;
    private float maxMoveArea;           // 스틱이 움직일수 있는 범위.
    private Vector3 stickDir, playerDir;         // 조이스틱의 벡터(방향), 플레이어의 방향
    private Vector3 stickFirstPos; //스틱의 초기좌표 
    private bool moveFlag;

    void Awake()
    {
        tf_player = GameObject.FindGameObjectWithTag(PlayerAttribute.instance.playerTag[0]).transform;
        animator = GameObject.FindGameObjectWithTag(PlayerAttribute.instance.playerTag[0]).GetComponent<AnimatorManager>();
    }
    void Start()
    {
        maxMoveArea = img_joystick_back.rectTransform.sizeDelta.y * 0.4f; //스틱이 움직일수있는 수평범위. ( * 0.5f면 정확히 조이스틱배경의 반지름만큼)
        stickFirstPos = img_joystick_stick.rectTransform.position; //스틱의 초기좌표.

        // 캔버스 크기에대한 반지름 조절.
      //  float can = transform.parent.GetComponent<RectTransform>().localScale.x;
     //   moveArea *= can;

       
        moveFlag = false;
    }
    void Update()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(player.transform.position);

        if (pos.x < 0f) pos.x = 0f;

        if (pos.x > 1f) pos.x = 1f;

        if (pos.y < 0f) pos.y = 0f;

        if (pos.y > 1f) pos.y = 1f;

        player.transform.position = Camera.main.ViewportToWorldPoint(pos);


        if (moveFlag == true)
        {          
            animator.am_playerMovement.SetBool("idle_to_run", true);
            tf_player.transform.Translate(playerDir * PlayerAttribute.instance.speed * Time.smoothDeltaTime, Space.World);
        }
        else
            animator.am_playerMovement.SetBool("idle_to_run", false);
    }


    // 드래그
    public void Drag(BaseEventData _Data)
    {
        moveFlag = true;

        PointerEventData data = _Data as PointerEventData;
        Vector3 pos = data.position; //드래그 한곳의 위치.

        // 스틱 이동방향 추출 .(오른쪽,왼쪽,위,아래)
        stickDir = (pos - stickFirstPos).normalized;
       // playerDir = (stickDir.x * Vector3.right) + (stickDir.y * Vector3.forward); //동시에 플레이어의 이동방향결정
        playerDir = new Vector3(stickDir.x, 0, stickDir.y);

        // 스틱의 처음 위치와 드래그중인 위치의 거리차를 구함
        float distance = Vector3.Distance(pos, stickFirstPos);

        // 이동가능범위 보다 작을때만 스틱의 이동. 아니면 최대범위만큼에서 고정.
        if (distance < maxMoveArea)
            img_joystick_stick.rectTransform.position = stickFirstPos + (stickDir * distance);
        else
            img_joystick_stick.rectTransform.position = stickFirstPos + (stickDir * maxMoveArea);

        //tf_player.transform.localRotation = Quaternion.Euler(0, Mathf.Atan2(stickDir.x, stickDir.y) * Mathf.Rad2Deg, 0);
         tf_player.transform.localRotation = Quaternion.LookRotation(playerDir);
        //tf_player.eulerAngles = new Vector3(0, Mathf.Atan2(playerDir.x, playerDir.y) * Mathf.Rad2Deg, 0); //플레이어의 방향을 바꿔줌.
    }

    // 드래그 끝.
    public void DragStart()
    {

        Debug.Log("드래그 스타트.");
    }

    // 드래그 끝.
    public void DragEnd()
    {
        img_joystick_stick.transform.position = stickFirstPos;
        stickDir = Vector3.zero; // 방향을 0으로.

        moveFlag = false;
    }

}
