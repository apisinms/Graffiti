using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSectorManager : MonoBehaviour
{
    public static ClientSectorManager instance; //싱글톤
    public GameObject obj_player;

    struct matrixDefine //맵을 행렬로 나눔
    {
        public Vector3 min_col;   // i행 j열 셀의 왼쪽 면 좌표
        public Vector3 max_col;  //                오른쪽

        public Vector3 min_row;  // i행 j열 셀의 위쪽 면 좌표
        public Vector3 max_row; //                아래쪽
    };

    matrixDefine[,] matrix = new matrixDefine[10,10]; // 10x10

    void Awake()
    {
        instance = this;

        //모든 셀들의 영역을 계산해놓음.
        for (int i=0; i<10; i++)
        {
            for (int j=0; j<10; j++)
            {
                //셀마다 행은 1씩증가
                matrix[i, j].min_col = new Vector3(0 + j, 0, 0);
                matrix[i, j].max_col = new Vector3(1 + j, 0, 0);

                //셀마다 열은 -1씩 감소
                matrix[j, i].min_row = new Vector3(0, 0, 0 - j);
                matrix[j, i].max_row = new Vector3(0, 0, -1 - j);
            }
        }
    }

    public void ProcessWhereAmI() //플레이어 영역을 구함
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                //몇행 몇열의 셀안에 플레이어가 들어가있느냐.
                if ((obj_player.transform.localPosition.x >= matrix[i , j].min_col.x && obj_player.transform.localPosition.x <= matrix[i, j].max_col.x) &&
                    (obj_player.transform.localPosition.z <= matrix[i , j].min_row.z && obj_player.transform.localPosition.z >= matrix[i, j].max_row.z))
                {
                    Debug.Log(i + "행" + " " + j + "열 진입");
                    break;
                }
            }
        }
    }

}
