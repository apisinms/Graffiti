using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    private Transform[] players = new Transform[4];
    private int index;

    void Awake()
    {
        for(int i=0; i<players.Length; i++)
        {
            players[i] = GameObject.FindGameObjectWithTag(PlayerAttribute.instance.playerTag[i]).transform;
        }
        Application.targetFrameRate = 60;
    }

	void Update()
	{
        index = NetworkManager.instance.GetPosPlayerNum -1;


        Debug.Log(NetworkManager.instance.GetPosPlayerNum);
        Vector3 vector = new Vector3(NetworkManager.instance.GetPosX, players[index].position.y, NetworkManager.instance.GetPosZ);
        players[index].position = vector;
	}
}