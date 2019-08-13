using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickManager2 : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public AnimatorManager sample { get; set; }

    public Image img_joystickBgrd;
    public Image img_joystickThumb;
    public GameObject player2;

    [System.NonSerialized]
    public Vector3 inputDirection = new Vector3(0, 0, 0);

    bool start = false;
    Vector3 direction = Vector3.zero;
    float keyHorizontal;
    float keyVertical;
    // Start is called before the first frame update
    void Awake()
    {
        sample = GameObject.FindGameObjectWithTag("Player1").GetComponent<AnimatorManager>();
    }


    void Update()
    {

        //       if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
        //           || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        //       {
        //  ClientSectorManager.instance.ProcessWhereAmI();
        //    am_playerMovement.SetBool("idle_to_run", true);

        //	if (NetworkManager.instance.MyPlayerNum == localNum)
        //	{
        //      keyHorizontal = direction.x = Input.GetAxis("Horizontal");
        //      keyVertical = direction.z = Input.GetAxis("Vertical");

        if (start == true)
        {

            sample.am_playerMovement.SetBool("idle_to_run", true);
            //direction이 Input.GetAxis의 반환값을 대신함. 겟액시스를 쓰지않음.
            if (direction.magnitude > 1)
                direction.Normalize();

            if (inputDirection != Vector3.zero)
                direction = inputDirection;

          //  Debug.Log(direction);
            //   player2.transform.LookAt(new Vector3(0, direction.z, 0));
            //  player2.transform.Rotate(new Vector3(0, direction.z, 0), Space.World);
            player2.transform.Translate(Vector3.right * 0.5f * (PlayerAttribute.instance.speed) * Time.smoothDeltaTime * direction.x, Space.World);
            player2.transform.Translate(Vector3.forward * PlayerAttribute.instance.speed * Time.smoothDeltaTime * direction.z, Space.World);
        }
        else
        {
            sample.am_playerMovement.SetBool("idle_to_run", false);
        }
        //		NetworkManager.instance.MayIIMove(transform.position.x, transform.position.z);
        //	}
        //       }
        //       else
        //        {
        //           am_playerMovement.SetBool("idle_to_run", false);
        //        }


    }

    public void OnDrag(PointerEventData _eventData)
    {
        Vector2 pos = new Vector2(0, 0);
        //버튼의 rect좌표와 터치한부분의 스크린좌표, ui를 쪼고있는 카메라오브젝트를 넣어서 
        //터치부분의 스크린포인트를 로컬좌표로 반환해줌.
        bool isSuccess = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            img_joystickBgrd.rectTransform, _eventData.position, _eventData.pressEventCamera, out pos);
    
        if(isSuccess == true)
        {
            pos.x = (pos.x / img_joystickBgrd.rectTransform.sizeDelta.x);
            pos.y = (pos.y / img_joystickBgrd.rectTransform.sizeDelta.y);

            float x = (img_joystickBgrd.rectTransform.pivot.x == 1) ? pos.x * 2 + 1 : pos.x * 2 - 1;
            float y = (img_joystickBgrd.rectTransform.pivot.y == 1) ? pos.y * 2 + 1 : pos.y * 2 - 1;

  
            inputDirection = new Vector3(x, 0, y);

            inputDirection = (inputDirection.magnitude > 1) ? inputDirection.normalized : inputDirection; 

            img_joystickThumb.rectTransform.anchoredPosition =
                new Vector3(inputDirection.x * (img_joystickBgrd.rectTransform.sizeDelta.x / 3.0f),
                inputDirection.z * (img_joystickThumb.rectTransform.sizeDelta.y / 1.8f));

            player2.transform.eulerAngles = new Vector3(0, Mathf.Atan2(pos.x, pos.y) * Mathf.Rad2Deg, 0);
        }

       
       // Player2.eulerAngles = new Vector3(0, Mathf.Atan2(JoyVec.x, JoyVec.y) * Mathf.Rad2Deg, 0);
    }

    public void OnPointerDown(PointerEventData _eventData)
    {

        Debug.Log("첫번째 조이스틱");
        start = true;
      
    }

    public void OnPointerUp(PointerEventData _eventData)
    {
        start = false;
        //조이스틱 땔떼 위치 원상복구
        direction = inputDirection = Vector3.zero;
        img_joystickThumb.rectTransform.anchoredPosition = Vector3.zero;
    }
}
