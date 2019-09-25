using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : UnityEngine.MonoBehaviour
{
    private GameObject obj_player;
    private Vector3 cameraPos;
    private Vector3 tmpPlayerPos;
    private int myIndex { get; set; }
    private _ACTION_STATE prevActionState { get; set; }
    private bool isOnLerp;

    void Awake()
    {
        obj_player = GameObject.FindGameObjectWithTag(GameManager.instance.myTag);
        myIndex = GameManager.instance.myIndex;
  
    }

    void Start()
    {
        //tmpPlayerPos = obj_player.transform.position;
        cameraPos.x = obj_player.transform.position.x;
        cameraPos.y = obj_player.transform.position.y + 15f;
        cameraPos.z = obj_player.transform.position.z - 6.0f;
        transform.position = cameraPos;
    }

    private void Update()
    {
        prevActionState = PlayersManager.instance.actionState[myIndex];
    }

    void LateUpdate()
    {
        if (PlayersManager.instance.attributeState[myIndex] == _ATTRIBUTE_STATE.ALIVE)
        {
            cameraPos.x = obj_player.transform.position.x;
            cameraPos.y = obj_player.transform.position.y + 15f;
            cameraPos.z = obj_player.transform.position.z - 6.0f;
            
            
            if (PlayersManager.instance.actionState[myIndex] == _ACTION_STATE.AIM ||
                PlayersManager.instance.actionState[myIndex] == _ACTION_STATE.CIR_AIM ||
                PlayersManager.instance.actionState[myIndex] == _ACTION_STATE.SHOT ||
                PlayersManager.instance.actionState[myIndex] == _ACTION_STATE.CIR_AIM_SHOT)
            {
                isOnLerp = true;
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, PlayersManager.instance.direction2[myIndex].x + cameraPos.x, Time.smoothDeltaTime * 7.5f),
                    15, Mathf.Lerp(transform.position.z, PlayersManager.instance.direction2[myIndex].z + (cameraPos.z - 1.5f), Time.smoothDeltaTime * 7.5f));
                //transform.position = new Vector3(PlayersManager.instance.direction2[myIndex].x + cameraPos.x, 15, PlayersManager.instance.direction2[myIndex].z + (cameraPos.z - 1.0f));
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, cameraPos, Time.smoothDeltaTime * 30f);

                
                if ((transform.position.x >= cameraPos.x - 0.05f && transform.position.x <= cameraPos.x + 0.05f)
                    &&
                (transform.position.z >= cameraPos.z - 0.05f && transform.position.z <= cameraPos.z + 0.05f))
                {
                    transform.position = cameraPos;
                }
                
            }

            /*
            if (PlayersManager.instance.actionState[myIndex] == _ACTION_STATE.IDLE ||
                PlayersManager.instance.actionState[myIndex] == _ACTION_STATE.CIR)
            {
                if (isOnLerp == true)
                {
                    transform.position = Vector3.Lerp(transform.position, cameraPos, Time.smoothDeltaTime * 5.0f);

                    if ((transform.position.x <= cameraPos.x - 0.2f && transform.position.x >= cameraPos.x - 0.2f) &&
(transform.position.z <= cameraPos.z - 0.2f && transform.position.z >= cameraPos.z - 0.2f))
                        isOnLerp = false;
                }
                else
                {
                    transform.position = cameraPos;
                }
             
                if (prevActionState == _ACTION_STATE.AIM ||
                    prevActionState == _ACTION_STATE.CIR_AIM ||
                    prevActionState == _ACTION_STATE.SHOT ||
                    prevActionState == _ACTION_STATE.CIR_AIM_SHOT)
                {
                    Debug.Log("러프1111111111111111111111111111111111");
                    transform.position = Vector3.Lerp(transform.position, cameraPos, Time.smoothDeltaTime * 5.0f);
                }
                else
                {
                    Debug.Log("노러프");
                    transform.position = cameraPos;
                } 

            }
            */
        }
    }

    public void SetCameraPos(float _posX, float _posY, float _posZ)
    {
        cameraPos.x = obj_player.transform.position.x + _posX;
        cameraPos.y = obj_player.transform.position.y + _posY;
        cameraPos.z = obj_player.transform.position.z + _posZ;

        transform.position = cameraPos;
    }

}






//        transform.RotateAround(obj_player.transform.localPosition,
//           PlayerManager.instance.tmp,  Time.smoothDeltaTime * 30);
// transform.localRotation = Quaternion.Euler(50, Mathf.Atan2(PlayerManager.instance.tmp.x, PlayerManager.instance.tmp.y) * Mathf.Rad2Deg, 0);
//Debug.Log(PlayerManager.instance.myDirection);
//transform.localRotation = Quaternion.Slerp(transform.rotation, obj_player.transform.rotation, Time.smoothDeltaTime * 5.0f);
//  transform.localRotation = Quaternion.Euler(50, Mathf.Atan2(PlayerManager.instance.tmp.x, PlayerManager.instance.tmp.y) * Mathf.Rad2Deg, 0);
// transform.position = Vector3.Lerp(transform.position, cameraPos, Time.smoothDeltaTime * 1.0f);
//  transform.LookAt(obj_player.transform);
// transform.position = cameraPos;
//this.transform.localRotation = Quaternion.LookRotation(PlayerManager.instance.myDirection);
// Vector3.MoveTowards(transform.position, cameraPos, Time.smoothDeltaTime * 8.0f);
// transform.eulerAngles = new Vector3(0, Mathf.Atan2(PlayerManager.instance.tmp.x, PlayerManager.instance.tmp.y) * Mathf.Rad2Deg, 0);



/*
if (tmpPlayerPos.x - 3.0f > obj_player.transform.position.x ||
    tmpPlayerPos.x + 3.0f < obj_player.transform.position.x ||
    tmpPlayerPos.z - 1.5f > obj_player.transform.position.z ||
    tmpPlayerPos.z + 1.5f < obj_player.transform.position.z)
{
    transform.position = Vector3.Lerp(transform.position, cameraPos, Time.smoothDeltaTime * 5.0f);
    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
     || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
    {
        // Debug.Log("참");
        // 카메라 위치 조정
        cameraPos.x = obj_player.transform.position.x;
        cameraPos.y = obj_player.transform.position.y + 5.5f;
        cameraPos.z = obj_player.transform.position.z - 3.5f;

        // transform.position = Vector3.Lerp(transform.position, cameraPos, Time.smoothDeltaTime * 5.0f);
    }
    else
    {
        //Debug.Log("거짓");
        tmpPlayerPos = obj_player.transform.position;
    }
    */
