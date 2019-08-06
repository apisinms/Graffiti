using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

	int speed = 7;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
			|| Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
		{
			if (NetworkManager.instance.MyPlayerNum == 1)
			{
				float keyHorizontal = Input.GetAxis("Horizontal");

				float keyVertical = Input.GetAxis("Vertical");

				transform.Translate(Vector3.right * speed * Time.smoothDeltaTime * keyHorizontal, Space.World);

				transform.Translate(Vector3.forward * speed * Time.smoothDeltaTime * keyVertical, Space.World);

				NetworkManager.instance.MayIIMove(transform.position.x, transform.position.z);
			}
		}
	}
}