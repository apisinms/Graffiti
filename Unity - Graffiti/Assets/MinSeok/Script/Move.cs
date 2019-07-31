using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    int speed = 7;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        float keyHorizontal = Input.GetAxis("Horizontal");

        float keyVertical = Input.GetAxis("Vertical");

        transform.Translate(Vector3.right * speed * Time.smoothDeltaTime * keyHorizontal, Space.World);

        transform.Translate(Vector3.forward * speed * Time.smoothDeltaTime * keyVertical, Space.World);


    }
}
