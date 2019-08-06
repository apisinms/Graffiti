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
		public bool isVisiting;
	};

	struct visitedMatrixIndex
	{
		public int col;
		public int row;
	};

	matrixDefine[,] matrix = new matrixDefine[10, 10]; // 10x10
	visitedMatrixIndex visited; //행렬에서 이미 방문중인 셀을 거를것임.


	void Awake()
	{
		instance = this;

		//모든 셀들의 영역을 계산해놓음.
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 10; j++)
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
				//i행 j열 셀과의 충돌체크
				if ((obj_player.transform.localPosition.x >= matrix[i, j].min_col.x && obj_player.transform.localPosition.x <= matrix[i, j].max_col.x) &&
					(obj_player.transform.localPosition.z <= matrix[i, j].min_row.z && obj_player.transform.localPosition.z >= matrix[i, j].max_row.z))
				{
					//현재 충돌된 셀과 바로직전에 충돌되었던 셀이 같은놈(이미 충돌중)이면 건너뛴다.
					if (matrix[visited.col, visited.row].isVisiting == true &&
						visited.col == i && visited.row == j)
					{
						//Debug.Log(i + "행" + " " + j + "열은 이미 방문중이다!");                       
					}
					else //새로운 셀과의 충돌시.
					{
						matrix[visited.col, visited.row].isVisiting = false;
				//		Debug.Log(visited.col + "행" + " " + visited.row + "열 탈출");

						//현재 충돌된 셀의 인덱스를 저장해둠
						visited.col = i;
						visited.row = j;

						matrix[i, j].isVisiting = true;
			//			Debug.Log(i + "행" + " " + j + "열 진입");
					}
					return;
				}
			}
		}
	}

}