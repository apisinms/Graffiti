using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSceneManager : Singleton<EndSceneManager>
{
    public int[] playerNum { get; set; }
    public string[] nickName { get; set; }
    public NetworkManager.Score[] scores { get; set; }
    public int gameType { get; set; }
    public int myIndex { get; set; }

private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void GoToLobby()
    {
        // 로비로 갈때 지워줘야 하므로 
        Destroy(gameObject);
    }
}
