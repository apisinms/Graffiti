using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    //블루팀 레드팀 애들을 일단 전부 등록시켜놓고
    public GameObject[] redTeam = new GameObject[2];
    public GameObject[] blueTeam = new GameObject[2];

    private float distance = 1f;
    Vector3 CameraPosition;
    
    void Start()
    {
        //어느팀의 몇번인지에 따라서.
        CameraPosition = redTeam[0].transform.position; //일단 레드팀 1번새기가 카메라에 포커싱.
    }
    void Update()
    {    
        // 카메라 위치 조정
        CameraPosition.x = redTeam[0].transform.position.x;
        CameraPosition.y = redTeam[0].transform.position.y + 10.0f;
        CameraPosition.z = redTeam[0].transform.position.z - 3.5f;

        transform.position = Vector3.Lerp(transform.position, CameraPosition, Time.smoothDeltaTime * 10f);
    }
}
