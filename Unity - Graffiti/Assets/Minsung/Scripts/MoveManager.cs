using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
	[SerializeField]
	private Transform[] curPlayerPos    = new Transform[4];
	//private Vector3[] beforePlayerPos;
	//private int index = -1;
	private float speed = 2.0f;

	NetworkManager networkManager;
	Vector3 pos;

	float startTime;
	float destLength;
	float distMoved;
	float fracMoved;

	void Awake()
	{
		Application.targetFrameRate = 60;
		networkManager = NetworkManager.instance;
		pos = new Vector3();

		//beforePlayerPos = new Vector3[4];

		// 초기값 설정
		for (int i = 0; i < 4; i++)
		{
			networkManager.SetPosX(i, curPlayerPos[i].position.x);
			networkManager.SetPosZ(i, curPlayerPos[i].position.z);
			networkManager.SetPosPlayerNum(i, i+1);
		}

		StartCoroutine(this.CheckQuit());
	}


	// 플레이어를 뒤져봐서 위치가 다르면 업데이트
	void Update()
	{
		for (int i = 0; i < 4; i++)
		{
			// 자기 제외하고
			if ((networkManager.MyPlayerNum - 1) == i)
				continue;

			if(	networkManager.GetPosX(i) != curPlayerPos[i].position.x ||
				networkManager.GetPosZ(i) != curPlayerPos[i].position.z)
			{
				pos.x = networkManager.GetPosX(i);
				pos.y = curPlayerPos[i].position.y;
				pos.z = networkManager.GetPosZ(i);

				curPlayerPos[i].position = Vector3.Lerp(
					curPlayerPos[i].position,
					pos,
					Time.smoothDeltaTime * (speed * 3));
			}
		}



		{
			//if (networkManager.CheckMove() == true)
			//{
			//	index = networkManager.GetPosPlayerNum - 1;

			//	// 새롭게 갱신됐으니 이전 위치를 저장
			//	beforePlayerPos[index] = new Vector3(
			//		curPlayerPos[index].position.x,
			//		curPlayerPos[index].position.y,
			//		curPlayerPos[index].position.z);

			//	// 서버로부터 받은 위치를 저장
			//	pos.x = networkManager.GetPosX;
			//	pos.y = curPlayerPos[index].position.y;
			//	pos.z = networkManager.GetPosZ;

			//	curPlayerPos[index].position = pos;

			//	// 움직임이 시작된 시간 계산
			//	startTime = Time.time;

			//	// 목적지까지 거리 계산
			//	destLength = Vector3.Distance(beforePlayerPos[index], curPlayerPos[index].position);
			//}

			//if (index != -1)
			//{
			//	// 움직인거리 = 시간 * 속도
			//	distMoved = (Time.time - startTime) * speed;

			//	//// 두 위치가 같아졌다면 인덱스에 음수값준다.
			//	if (distMoved >= destLength)
			//	{
			//		index = -1;
			//		return;
			//	}

			//	// 움직임이 완료된 비율 = 현재 거리를 총 거리로 나눈다.
			//	fracMoved = distMoved / destLength;



			//	// 두 벡터 사이의 비율을 현재 위치로 설정한다.
			//	curPlayerPos[index].position = Vector3.Lerp(beforePlayerPos[index], curPlayerPos[index].position, fracMoved);

			//	//// 계산된 현재 위치가 다음 프레임에는 이전 위치가 된다.
			//	//beforePlayerPos[index] = curPlayerPos[index].position;
			//}
		}
	}

	IEnumerator CheckQuit()
	{
		while (true)
		{
			switch (networkManager.CheckQuit())
			{
				case 1:
					Destroy(GameObject.Find("red_1"));
					break;

				case 2:
					Destroy(GameObject.Find("red_2"));
					break;

				case 3:
					Destroy(GameObject.Find("blue_1"));
					break;

				case 4:
					Destroy(GameObject.Find("blue_2"));
					break;

				default:
					break;
			}

			yield return YieldInstructionCache.WaitForSeconds(0.25f);
		}
	}
}