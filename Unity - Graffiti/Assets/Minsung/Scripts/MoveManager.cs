using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    public Transform[] curPlayerPos = new Transform[4];
	private int index;

	NetworkManager networkManager;
	Vector3 pos;

	float startTime;
	Vector3 destLength;
	float distCovered;
	float fracJourney;

	void Awake()
    {
		Application.targetFrameRate = 60;
		networkManager = NetworkManager.instance;
		pos = new Vector3();
		StartCoroutine(this.CheckQuit());
	}
	void Start()
	{
	}
	void Update()
	{
        // 인덱스 이므로 -1 해줘야 한다.
        index = networkManager.GetPosPlayerNum - 1;

        pos.x = networkManager.GetPosX;
        pos.y = curPlayerPos[index].position.y;
        pos.z = networkManager.GetPosZ;

        curPlayerPos[index].position = Vector3.Lerp(
            curPlayerPos[index].position,
            pos,
            Time.deltaTime * 10.0f);

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