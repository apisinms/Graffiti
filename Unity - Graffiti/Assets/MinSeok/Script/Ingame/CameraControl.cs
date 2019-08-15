using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //블루팀 레드팀 애들을 일단 전부 등록시켜놓고
    private GameObject[] obj_redTeam = new GameObject[2];
    private GameObject[] obj_blueTeam = new GameObject[2];
    private GameObject obj_player;

    Vector3 cameraPos;
    Vector3 tmpPlayerPos;

    
    void Awake()
    {  
        obj_player = obj_redTeam[0] = GameObject.FindGameObjectWithTag(PlayerManager.instance.playerTag[0]);
        obj_redTeam[1] = GameObject.FindGameObjectWithTag(PlayerManager.instance.playerTag[1]);
        obj_blueTeam[0] = GameObject.FindGameObjectWithTag(PlayerManager.instance.playerTag[2]);
        obj_blueTeam[1] = GameObject.FindGameObjectWithTag(PlayerManager.instance.playerTag[3]);      
    }
    void Start()
    {
       
        /*
        switch (NetworkManager.instance.MyPlayerNum)
        {
            case 1:
                PlayerAttribute.instance.myNetworkNum = 1;
                obj_player = obj_redTeam[0];
                CameraPosition = player.transform.position; 
                break;
            case 2:
            PlayerAttribute.instance.myNetworkNum = 2;
                obj_player = obj_redTeam[1];
                CameraPosition = player.transform.position;
                break;
            case 3:
            PlayerAttribute.instance.myNetworkNum = 3;
                obj_player = obj_blueTeam[0];
                CameraPosition = player.transform.position;
                break;
            case 4:
            PlayerAttribute.instance.myNetworkNum = 4;
                obj_player = obj_blueTeam[1];
                CameraPosition = player.transform.position;
                break;
        }
        */


      tmpPlayerPos = obj_player.transform.position;

    }

    void LateUpdate()
    {
        if (PlayerManager.instance.moveFlag == true)
        {
            cameraPos.x = obj_player.transform.position.x;
            cameraPos.y = obj_player.transform.position.y + 8f;
            cameraPos.z = obj_player.transform.position.z - 5.5f;



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

}

