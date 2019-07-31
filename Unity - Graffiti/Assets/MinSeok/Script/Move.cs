using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    int speed = 7;
    // Use this for initialization
    void Start () {
        InvokeRepeating("MoveCheck", 0.05f, 0.05f);
    }

    private void MoveCheck()
    {
        if(NetworkManager.instance.CheckInGameSuccess() == true)
        {
            Vector3 vector = new Vector3(NetworkManager.instance.GetPosX, transform.position.y, NetworkManager.instance.GetPosZ);
            transform.position = vector;
        }

        CancelInvoke("MoveCheck");
    }

    // Update is called once per frame
    void Update ()
    {
        float keyHorizontal = Input.GetAxis("Horizontal");

        float keyVertical = Input.GetAxis("Vertical");

        transform.Translate(Vector3.right * speed * Time.smoothDeltaTime * keyHorizontal, Space.World);

        transform.Translate(Vector3.forward * speed * Time.smoothDeltaTime * keyVertical, Space.World);


        NetworkManager.instance.MayIIMove(transform.position.x, transform.position.z);
        
        //Vector3 vector = new Vector3(NetworkManager.instance.GetPosX, transform.position.y, NetworkManager.instance.GetPosZ);
        //transform.position = vector;

    }
}
