using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttribute : MonoBehaviour
{
    public static PlayerAttribute instance;
    public readonly string[] playerTag = new string[4];
    public int myLocalNum { get; set; }
    public int myNetworkNum { get; set; }
    public float speed { get; set; }

    void Awake()
    {
        if (instance == null)
            instance = this;

        playerTag[0] = "Player1"; playerTag[1] = "Player2";
        playerTag[2] = "Player3"; playerTag[3] = "Player3";

        speed = 4.0f;
    }

    private void Start()
    {
        for (int i = 0; i < playerTag.Length; i++)
        {
            Debug.Log(playerTag[i]);
        }

    }

}
