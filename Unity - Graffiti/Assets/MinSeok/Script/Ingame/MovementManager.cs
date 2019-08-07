using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : AnimatorManager
{
    Vector3 direction3 = Vector3.zero;
    protected override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    void Start()
    {
        switch (this.gameObject.tag)
        {
            case "Player1":
                PlayerAttribute.instance.myLocalNum = 1;
                break;
            case "Player2":
                PlayerAttribute.instance.myLocalNum = 2;
                break;
            case "Player3":
                PlayerAttribute.instance.myLocalNum = 3;
                break;
            case "Player4":
                PlayerAttribute.instance.myLocalNum = 4;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
       /*
    //       if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
    //           || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
    //       {
    ClientSectorManager.instance.ProcessWhereAmI();
            am_playerMovement.SetBool("idle_to_run", true);

            //	if (NetworkManager.instance.MyPlayerNum == localNum)
            //	{
            float keyHorizontal = direction3.x = Input.GetAxis("Horizontal");
            float keyVertical = direction3.z = Input.GetAxis("Vertical");

            if (direction3.magnitude > 1)
                direction3.Normalize();

            if (inputDirection != Vector3.zero)
                direction3 = inputDirection;
       Debug.Log(inputDirection);
            transform.Translate(Vector3.right * PlayerAttribute.instance.speed * Time.smoothDeltaTime * keyHorizontal, Space.World);
            transform.Translate(Vector3.forward * PlayerAttribute.instance.speed * Time.smoothDeltaTime * keyVertical, Space.World);
            //		NetworkManager.instance.MayIIMove(transform.position.x, transform.position.z);
            //	}
 //       }
 //       else
//        {
 //           am_playerMovement.SetBool("idle_to_run", false);
//        }
        */

    }

}