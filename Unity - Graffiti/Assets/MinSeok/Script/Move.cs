using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private int localNum;
	int speed = 7;

	// Use this for initialization
	void Start()
	{
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
				float keyHorizontal = Input.GetAxis("Horizontal");

				float keyVertical = Input.GetAxis("Vertical");

				transform.Translate(Vector3.right * speed * Time.smoothDeltaTime * keyHorizontal, Space.World);

				transform.Translate(Vector3.forward * speed * Time.smoothDeltaTime * keyVertical, Space.World);

				NetworkManager.instance.MayIIMove(transform.position.x, transform.position.z);
			}
		}
	}
}