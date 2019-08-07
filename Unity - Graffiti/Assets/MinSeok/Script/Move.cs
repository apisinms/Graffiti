using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
	private int localNum;
	int speed = 7;

	Animator am_move;
	Vector3 beforePos;
	float keyHorizontal;
	float keyVertical;

	void Awake()
	{
		am_move = gameObject.GetComponent<Animator>();
		beforePos = transform.position;
	}

	// Use this for initialization
	void Start()
	{
		StartCoroutine(MovePlayer());

		switch (this.gameObject.tag)
		{
			case "Player1":
				localNum = 1;
				break;
			case "Player2":
				localNum = 2;
				break;
			case "Player3":
				localNum = 3;
				break;
			case "Player4":
				localNum = 4;
				break;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
			|| Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
		{
			if (NetworkManager.instance.MyPlayerNum == localNum)
			{
				keyHorizontal = Input.GetAxis("Horizontal");
				keyVertical = Input.GetAxis("Vertical");

				transform.Translate(Vector3.right * speed * Time.smoothDeltaTime * keyHorizontal, Space.World);
				transform.Translate(Vector3.forward * speed * Time.smoothDeltaTime * keyVertical, Space.World);
			}
		}
	}

	IEnumerator MovePlayer()
	{
		while (true)
		{
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
				Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
				if (NetworkManager.instance.MyPlayerNum == localNum)
				{
					NetworkManager.instance.MayIMove(transform.position.x, transform.position.z);
				}
			}
			yield return YieldInstructionCache.WaitForSeconds(0.08f);
		}
	}
}