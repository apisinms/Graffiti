using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : UnityEngine.MonoBehaviour
{
    private GameObject obj_player;
    private Vector3 cameraPos;
    private Vector3 tmpPlayerPos;

    void Awake()
    {
        obj_player = GameObject.FindGameObjectWithTag(GameManager.instance.myTag);
    }

    void Start()
    {
        //tmpPlayerPos = obj_player.transform.position;
        cameraPos.x = obj_player.transform.position.x;
        cameraPos.y = obj_player.transform.position.y + 7f;
        cameraPos.z = obj_player.transform.position.z - 5.3f;
        transform.position = cameraPos;
    }

    void LateUpdate()
    {
        if (PlayersManager.instance.attributeState[PlayersManager.instance.myIndex] == _ATTRIBUTE_STATE.ALIVE &&
            PlayersManager.instance.actionState[PlayersManager.instance.myIndex] != _ACTION_STATE.IDLE)
        {
            cameraPos.x = obj_player.transform.position.x;
            cameraPos.y = obj_player.transform.position.y + 7f;
            cameraPos.z = obj_player.transform.position.z - 5.3f;



            //        transform.RotateAround(obj_player.transform.localPosition,
            //           PlayerManager.instance.tmp,  Time.smoothDeltaTime * 30);

            

            // transform.localRotation = Quaternion.Euler(50, Mathf.Atan2(PlayerManager.instance.tmp.x, PlayerManager.instance.tmp.y) * Mathf.Rad2Deg, 0);
            //Debug.Log(PlayerManager.instance.myDirection);
            //transform.localRotation = Quaternion.Slerp(transform.rotation, obj_player.transform.rotation, Time.smoothDeltaTime * 5.0f);
            //  transform.localRotation = Quaternion.Euler(50, Mathf.Atan2(PlayerManager.instance.tmp.x, PlayerManager.instance.tmp.y) * Mathf.Rad2Deg, 0);
            //  transform.position = cameraPos;//Vector3.Lerp(transform.position, cameraPos, Time.smoothDeltaTime * 2.0f);
            //  transform.LookAt(obj_player.transform);

            transform.position = cameraPos;

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

