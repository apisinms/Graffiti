using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    //블루팀 레드팀 애들을 일단 전부 등록시켜놓고
    public GameObject[] obj_redTeam = new GameObject[2];
    public GameObject[] obj_blueTeam = new GameObject[2];

    [SerializeField]
    GameObject obj_player;

    Vector3 cameraPos;
    Vector3 tmpPlayerPos;


    public GameObject test;

    void Start()
    {
        /*
        switch (NetworkManager.instance.MyPlayerNum)
        {
            case 1:
                player = redTeam[0];
                CameraPosition = player.transform.position; 
                break;
            case 2:
                player = redTeam[1];
                CameraPosition = player.transform.position;
                break;
            case 3:
                player = blueTeam[0];
                CameraPosition = player.transform.position;
                break;
            case 4:
                player = blueTeam[1];
                CameraPosition = player.transform.position;
                break;
        }
        */


        tmpPlayerPos = obj_player.transform.position;

    }

    void LateUpdate()
    {

        cameraPos.x = obj_player.transform.position.x;
        cameraPos.y = obj_player.transform.position.y + 5.45f;
        cameraPos.z = obj_player.transform.position.z - 3.0f;

        transform.position = cameraPos; // Vector3.MoveTowards(transform.position, cameraPos, Time.smoothDeltaTime * 8.0f);

     


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

