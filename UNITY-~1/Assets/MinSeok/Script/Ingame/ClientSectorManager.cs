using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSectorManager : UnityEngine.MonoBehaviour
{
    private const float CELL_SIZE = 2.5f;
    private const int MAX_COL = 4;
    private const int MAX_ROW = 4;

    public static ClientSectorManager instance; //싱글톤
    private Transform tf_player;
    public Transform tf_cube;

    struct _MatrixDefine //맵을 행렬로 나눔
    {
        public Vector3 min_col;   // i행 j열 셀의 왼쪽 면 좌표
        public Vector3 max_col;  //                오른쪽

        public Vector3 min_row;  // i행 j열 셀의 위쪽 면 좌표
        public Vector3 max_row; //                아래쪽
    };

    struct _Visited_MatrixIndex
    {
        public int curCol;
        public int curRow;
        public int prevCol;
        public int prevRow;
        public bool isVisiting;
    };

    private _MatrixDefine[,] matrix = new _MatrixDefine[MAX_COL, MAX_ROW]; // 4x4
    private _Visited_MatrixIndex myVisited; //행렬에서 내가 이미 방문중인 셀을 거를것임.
    private _Visited_MatrixIndex otherVisited; //플레이어 영역내에서 이미 방문중인 셀을 거를것임.

    private float x1 = 0;
    private float x2 = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;

        tf_player = PlayersManager.instance.obj_players[GameManager.instance.myIndex].transform;

        //모든 셀들의 영역을 계산해놓음.
        for (int i = 0; i < MAX_COL; i++)
        {
            for (int j = 0; j < MAX_ROW; j++)
            {
                //셀마다 행은 2.5씩증가
                matrix[i, j].min_col = new Vector3(x1, 0, 0);
                matrix[i, j].max_col = new Vector3(x1 = x1 + 2.5f, 0, 0);

                //셀마다 열은 -2.5씩 감소
                matrix[j, i].min_row = new Vector3(0, 0, x2);
                matrix[j, i].max_row = new Vector3(0, 0, x2 = x2 - 2.5f);

                //  Debug.Log(i + "행 " + j + "열: " + matrix[i, j].min_col + " " + matrix[i, j].max_col);           
            }
            x1 = 0f;
            x2 = 0f;
        }
    }

    private void Start()
    {
        myVisited.prevCol = myVisited.curCol = (int)Mathf.Abs(tf_player.localPosition.z / CELL_SIZE); //내가 속한 행 인덱스를 구함
        myVisited.prevRow = myVisited.curRow = (int)Mathf.Abs(tf_player.localPosition.x / CELL_SIZE); //열 인덱스를 구함
    }
    private void Update()
    {
       // GetMyArea();
    }

    public void CheckOthersEntryMyArea(Transform _pos) //다른오브젝트(플레이어)가 내 시야에 들어왔는지 검사.
    {
        //내 현재위치 행의 이전행(바로뒷행, 위쪽)이 없을때. 즉 인덱스가 0이다. 맵에서 맨 윗줄 일때
        if (myVisited.curCol - 1 < 0)
        {
            if (myVisited.curCol - 1 < 0 && myVisited.curRow - 1 < 0) //그중에서 맨왼쪽 칸 https://blog.naver.com/alstjrsladk/221628380935
            {
                //예시로한 큐브가 그 영역에 들어왔을때. 제외시켜야하는 영역을 거름
                if (tf_cube.localPosition.z <= matrix[myVisited.curCol, myVisited.curRow].min_row.z &&
                    tf_cube.localPosition.z >= matrix[myVisited.curCol + 1, myVisited.curRow].max_row.z &&
                    tf_cube.localPosition.x >= matrix[myVisited.curCol, myVisited.curRow].min_col.x &&
                    tf_cube.localPosition.x <= matrix[myVisited.curCol, myVisited.curRow + 1].max_col.x)
                {
                    Debug.Log("큐브가 0행 0열 기준 영역 인 <맨 윗행 맨 왼쪽 칸>");
                }
                else
                    Debug.Log("큐브가 0행 0열 기준 영역 아웃 <맨 윗행 맨 왼쪽 칸>");
            }
            else if (myVisited.curCol - 1 < 0 && myVisited.curRow + 1 > MAX_ROW - 1) //맨 오른쪽칸 https://blog.naver.com/alstjrsladk/221628381398
            {
                if (tf_cube.localPosition.z <= matrix[myVisited.curCol, myVisited.curRow].min_row.z &&
                    tf_cube.localPosition.z >= matrix[myVisited.curCol + 1, myVisited.curRow].max_row.z &&
                    tf_cube.localPosition.x >= matrix[myVisited.curCol, myVisited.curRow - 1].min_col.x &&
                    tf_cube.localPosition.x <= matrix[myVisited.curCol, myVisited.curRow].max_col.x)
                {
                    Debug.Log("큐브가 0행 3열 기준 영역 인 <맨 윗행 맨 오른쪽 칸>");
                }
                else
                    Debug.Log("큐브가 0행 3열 기준 영역 아웃 <맨 윗행 맨 오른쪽 칸>");
            }
            else //가운데 칸 두개는 공통임 범위가. https://blog.naver.com/alstjrsladk/221628381780 / https://blog.naver.com/alstjrsladk/221628381892
            {
                if (tf_cube.localPosition.z <= matrix[myVisited.curCol, myVisited.curRow].min_row.z &&
                    tf_cube.localPosition.z >= matrix[myVisited.curCol + 1, myVisited.curRow].max_row.z &&
                    tf_cube.localPosition.x >= matrix[myVisited.curCol, myVisited.curRow - 1].min_col.x &&
                    tf_cube.localPosition.x <= matrix[myVisited.curCol, myVisited.curRow + 1].max_col.x)
                {
                    Debug.Log("큐브가 0행 1, 2열 기준 영역 인 <맨 윗행 가운데 두개 칸>");
                }
                else
                    Debug.Log("큐브가 0행 1, 2열 기준 영역 아웃 <맨 윗행 가운데 두개 칸>");
            }
        }

        else if (myVisited.curCol + 1 > MAX_COL - 1) //내현재행 바로 다음행(아래 행)이 없을때. 인덱스가 3을 초과할때 즉 맨아랫줄
        {
            if (myVisited.curCol + 1 > MAX_COL - 1 && myVisited.curRow - 1 < 0) //그중에서 맨왼쪽칸 https://blog.naver.com/alstjrsladk/221628381981
            {
                if (tf_cube.localPosition.z <= matrix[myVisited.curCol - 1, myVisited.curRow].min_row.z &&
                    tf_cube.localPosition.z >= matrix[myVisited.curCol, myVisited.curRow].max_row.z &&
                    tf_cube.localPosition.x >= matrix[myVisited.curCol, myVisited.curRow].min_col.x &&
                    tf_cube.localPosition.x <= matrix[myVisited.curCol, myVisited.curRow + 1].max_col.x)
                {
                    Debug.Log("큐브가 3행 0열 기준 영역 인 <맨 아랫행 맨 왼쪽 칸>");
                }
                else
                    Debug.Log("큐브가 3행 0열 기준 영역 아웃 <맨 아랫행 맨 왼쪽 칸>");
            }

            else if (myVisited.curCol + 1 > MAX_COL - 1 && myVisited.curRow + 1 > MAX_ROW - 1) //오른쪽칸 https://blog.naver.com/alstjrsladk/221628382072
            {
                if (tf_cube.localPosition.z <= matrix[myVisited.curCol - 1, myVisited.curRow].min_row.z &&
                   tf_cube.localPosition.z >= matrix[myVisited.curCol, myVisited.curRow].max_row.z &&
                    tf_cube.localPosition.x >= matrix[myVisited.curCol, myVisited.curRow - 1].min_col.x &&
                    tf_cube.localPosition.x <= matrix[myVisited.curCol, myVisited.curRow].max_col.x)
                {
                    Debug.Log("큐브가 3행 3열 기준 영역 인 <맨 아랫행 맨 오른쪽 칸>");
                }
                else
                    Debug.Log("큐브가 3행 3열 기준 영역 아웃 <맨 아랫행 맨 오른쪽 칸>");
            }
            else //가운데 두개 칸 https://blog.naver.com/alstjrsladk/221628382201 / https://blog.naver.com/alstjrsladk/221628382279
            {
                if (tf_cube.localPosition.z <= matrix[myVisited.curCol - 1, myVisited.curRow].min_row.z &&
                tf_cube.localPosition.z >= matrix[myVisited.curCol, myVisited.curRow].max_row.z &&
                tf_cube.localPosition.x >= matrix[myVisited.curCol, myVisited.curRow - 1].min_col.x &&
                tf_cube.localPosition.x <= matrix[myVisited.curCol, myVisited.curRow + 1].max_col.x)
                {
                    Debug.Log("큐브가 3행 1, 2열 기준 영역 인 <맨 아랫행 가운데 두개 칸>");
                }
                else
                    Debug.Log("큐브가 3행 1, 2열 기준 영역 아웃 <맨 아랫행 가운데 두개 칸>");
            }
        }

        else if (myVisited.curRow - 1 < 0) //내 열의 바로이전(왼쪽)열이 없을때. 즉 맨 왼쪽 열
        {
            // https://blog.naver.com/alstjrsladk/221628382400 / https://blog.naver.com/alstjrsladk/221628382531
            if (tf_cube.localPosition.z <= matrix[myVisited.curCol - 1, myVisited.curRow].min_row.z &&
                tf_cube.localPosition.z >= matrix[myVisited.curCol + 1, myVisited.curRow].max_row.z &&
                tf_cube.localPosition.x >= matrix[myVisited.curCol, myVisited.curRow].min_col.x &&
                tf_cube.localPosition.x <= matrix[myVisited.curCol, myVisited.curRow + 1].max_col.x)
            {
                Debug.Log("큐브가 1, 2행 0열 기준 영역 인 <맨 왼쪽 열 가운데 두개 칸>");
            }
            else
                Debug.Log("큐브가 1, 2행 0열 기준 영역 아웃 <맨 왼쪽 열 가운데 두개 칸>");
        }

        else if (myVisited.curRow + 1 > MAX_ROW - 1) //내 열의 바로다음(오른쪽)열이 없을때. 즉 맨 오른쪽 열
        {
            // https://blog.naver.com/alstjrsladk/221628382655 / https://blog.naver.com/alstjrsladk/221628382734
            if (tf_cube.localPosition.z <= matrix[myVisited.curCol - 1, myVisited.curRow].min_row.z &&
               tf_cube.localPosition.z >= matrix[myVisited.curCol + 1, myVisited.curRow].max_row.z &&
                tf_cube.localPosition.x >= matrix[myVisited.curCol, myVisited.curRow - 1].min_col.x &&
                tf_cube.localPosition.x <= matrix[myVisited.curCol, myVisited.curRow].max_col.x)
            {
                Debug.Log("큐브가 1, 2행 3열 기준 영역 인 <맨 오른쪽 열 가운데 두개 칸>");
            }
            else
                Debug.Log("큐브가 1, 2행 3열 기준 영역 아웃 <맨 오른쪽 열 가운데 두개 칸>");
        }
        //전부아니고 맵전체 가운데칸 4개일때 https://blog.naver.com/alstjrsladk/221628382811
        else if (tf_cube.localPosition.z <= matrix[myVisited.curCol - 1, myVisited.curRow].min_row.z &&
            tf_cube.localPosition.z >= matrix[myVisited.curCol + 1, myVisited.curRow].max_row.z &&
            tf_cube.localPosition.x >= matrix[myVisited.curCol, myVisited.curRow - 1].min_col.x &&
            tf_cube.localPosition.x <= matrix[myVisited.curCol, myVisited.curRow + 1].max_col.x)
        {
            Debug.Log("큐브가 전체 맵 가운데 네칸 기준 영역 인");
        }
        else
            Debug.Log("큐브가 전체 맵 가운데 네칸 기준 영역 아웃");

    }

    public void GetMyArea()
    {      
        myVisited.curCol = (int)Mathf.Abs(tf_player.localPosition.z / CELL_SIZE); //내가 속한 행 인덱스를 구함
        myVisited.curRow = (int)Mathf.Abs(tf_player.localPosition.x / CELL_SIZE); //열 인덱스를 구함

        if(myVisited.prevCol == myVisited.curCol && myVisited.prevRow == myVisited.curRow)
        {
            //Debug.Log(myVisited.curCol + "행" + " " + myVisited.curRow + "열은 이미 방문중이다!");    
        }
        else
        {
            Debug.Log("내가 " + myVisited.prevCol + "행" + " " + myVisited.prevRow + "열 탈출");

            myVisited.prevCol = myVisited.curCol;
            myVisited.prevRow = myVisited.curRow;
            Debug.Log("내가 " + myVisited.curCol + "행" + " " + myVisited.curRow + "열 진입");
        }
        

        /* = 0; i < MAX_COL; i++)
        {
            for (int j = 0; j < MAX_ROW; j++)
            {
                //i행 j열 셀과의 충돌체크
                if ((tf_player.localPosition.x >= matrix[i, j].min_col.x && tf_player.localPosition.x <= matrix[i, j].max_col.x) &&
                    (tf_player.localPosition.z <= matrix[i, j].min_row.z && tf_player.localPosition.z >= matrix[i, j].max_row.z))
                {
                    //현재 충돌된 셀과 바로직전에 충돌되었던 셀이 같은놈(이미 충돌중)이면 건너뛴다.
                    if (myVisited.isVisiting == true && myVisited.curCol == i && myVisited.curRow == j)
                    {
                        //Debug.Log(i + "행" + " " + j + "열은 이미 방문중이다!");                       
                    }
                    else //새로운 셀과의 충돌시.
                    {
                        myVisited.isVisiting = false;
                        Debug.Log("내가 " + myVisited.curCol + "행" + " " + myVisited.curRow + "열 탈출");

                        //현재 충돌된 셀의 인덱스를 저장해둠
                        myVisited.curCol = i;
                        myVisited.curRow = j;
                        myVisited.isVisiting = true;

                        Debug.Log("내가 " + myVisited.curCol + "행" + " " + myVisited.curRow + "열 진입");
                    }
                    return;
                }
            }
        }
        */
        
    } //내 영역을구함

    public void GetMyArea(Transform _pos) //다른오브젝트의 영역을 구함
    {
        otherVisited.curCol = (int)Mathf.Abs(_pos.localPosition.z / CELL_SIZE); //내가 속한 행 인덱스를 구함
        otherVisited.curRow = (int)Mathf.Abs(_pos.localPosition.x / CELL_SIZE); //열 인덱스를 구함

        if (otherVisited.prevCol == otherVisited.curCol && otherVisited.prevRow == otherVisited.curRow)
        {
            //Debug.Log(myVisited.curCol + "행" + " " + myVisited.curRow + "열은 이미 방문중이다!");    
        }
        else
        {
            Debug.Log("큐브가 " + otherVisited.prevCol + "행" + " " + otherVisited.prevRow + "열 탈출");

            otherVisited.prevCol = otherVisited.curCol;
            otherVisited.prevRow = otherVisited.curRow;
            Debug.Log("큐브가 " + otherVisited.curCol + "행" + " " + otherVisited.curRow + "열 진입");
        }
        /*
        for (int i = 0; i < MAX_COL; i++)
        {
            for (int j = 0; j < MAX_ROW; j++)
            {
                //i행 j열 셀과의 충돌체크
                if ((_pos.localPosition.x >= matrix[i, j].min_col.x && _pos.localPosition.x <= matrix[i, j].max_col.x) &&
                    (_pos.localPosition.z <= matrix[i, j].min_row.z && _pos.localPosition.z >= matrix[i, j].max_row.z))
                {
                    //현재 충돌된 셀과 바로직전에 충돌되었던 셀이 같은놈(이미 충돌중)이면 건너뛴다.
                    if (otherVisited.isVisiting == true && otherVisited.curCol == i && otherVisited.curRow == j)
                    {
                        //Debug.Log(i + "행" + " " + j + "열은 이미 방문중이다!");                       
                    }
                    else //새로운 셀과의 충돌시.
                    {
                        otherVisited.isVisiting = false;
                        Debug.Log("큐브가 " + otherVisited.curCol + "행" + " " + otherVisited.curRow + "열 탈출");

                        //현재 충돌된 셀의 인덱스를 저장해둠
                        otherVisited.curCol = i;
                        otherVisited.curRow = j;
                        otherVisited.isVisiting = true;

                        Debug.Log("큐브가 " + otherVisited.curCol + "행" + " " + otherVisited.curRow + "열 진입");
                        CheckOthersEntryMyArea(_pos);
                    }
                    return;
                }
            }
        }
        */
    }

}