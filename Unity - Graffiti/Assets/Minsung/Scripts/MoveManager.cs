using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
	[SerializeField]
	private Transform[] curPlayerPos = new Transform[4];
	private float speed = 2.0f;

	NetworkManager networkManager;
	Vector3 pos;

	void Awake()
	{
		Application.targetFrameRate = 60;
		networkManager = NetworkManager.instance;
		pos = new Vector3();

		// 초기값 설정
		for (int i = 0; i < 4; i++)
		{
			networkManager.SetPosX(i, curPlayerPos[i].position.x);
			networkManager.SetPosZ(i, curPlayerPos[i].position.z);
			networkManager.SetPosPlayerNum(i, i + 1);
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

			if (networkManager.GetPosX(i) != curPlayerPos[i].position.x ||
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