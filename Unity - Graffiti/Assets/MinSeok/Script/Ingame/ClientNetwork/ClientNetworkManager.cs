using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientNetworkManager : MonoBehaviour
{
    public static ClientNetworkManager instance;

    public readonly string[] playersTag = new string[4];
    public int myLocalNum { get; set; }
    public int myNetworkNum { get; set; }
    public string myTag { get; set; }

    private GameObject[] obj_players = new GameObject[3];

    void Awake()
    {
        if (instance == null)
            instance = this;

        playersTag[0] = "Player1"; playersTag[1] = "Player2";
        playersTag[2] = "Player3"; playersTag[3] = "Player4";

        myNetworkNum = 4; //예시로 번호부여함.  서버에서 샌드된번호로 해야함.

        switch (myNetworkNum) //서버로부터 받은 번호에따라서 
        {
            case 1:
                myTag = "Player1";
                break;
            case 2:
                myTag = "Player2";
                break;
            case 3:
                myTag = "Player3";
                break;
            case 4:
                myTag = "Player4";
                break;
        }

        // 자기번호를 제외한 플레이어 오브젝트에 나머지 3명을 등록.
        int j = 0;
        for (int i = 0; i < playersTag.Length; i++)
        {
            if (playersTag[i].Equals(myTag) == false)
            {
                obj_players[j] = GameObject.FindGameObjectWithTag(playersTag[i]);
                j++;
            }
        }

        GameObject.FindGameObjectWithTag(myTag).AddComponent<PlayerManager>(); //내가 부여받은 번호의 로빈과 합체.
    }

}
