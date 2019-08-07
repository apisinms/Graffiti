using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    //블루팀 레드팀 애들을 일단 전부 등록시켜놓고
    public GameObject[] redTeam = new GameObject[2];
    public GameObject[] blueTeam = new GameObject[2];
    GameObject player;
    Vector3 CameraPosition;
    
    void Start()
    {
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
    }
    void Update()
    {    
        // 카메라 위치 조정
        CameraPosition.x = player.transform.position.x;
        CameraPosition.y = player.transform.position.y + 10.0f;
        CameraPosition.z = player.transform.position.z - 3.5f;

        transform.position = Vector3.Lerp(transform.position, CameraPosition, Time.smoothDeltaTime * 10f);
    }
}
