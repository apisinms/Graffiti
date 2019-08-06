using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    public Transform player3;
    public Transform player4;

    void Update()
    {
        switch(NetworkManager.instance.GetPosPlayerNum)
        {
            case 1:
                {
                    Vector3 vector = new Vector3(NetworkManager.instance.GetPosX, transform.position.y, NetworkManager.instance.GetPosZ);
                    player1.position = vector;
                }
                break;
        }
    }
}
