
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GameInfo
    {
        [MarshalAs(UnmanagedType.I4)]
        public int gameType;       // 게임 타입(나중에 모드가 여러 개 생길 수도 있으니)

        [MarshalAs(UnmanagedType.I4)]
        public int maxPlayer;       // 최대 플레이어 수

        [MarshalAs(UnmanagedType.R4)]
        public float maxSpeed;     // 최대 이동속도

        [MarshalAs(UnmanagedType.R4)]
        public float maxHealth;    // 최대 체력

        [MarshalAs(UnmanagedType.I4)]
        public int respawnTime;     // 리스폰 시간

        [MarshalAs(UnmanagedType.I4)]
        public int gameTime;       // 게임 시간(ex 180초)

        [MarshalAs(UnmanagedType.I4)]
        public int killPoint;       // 킬 점수

        [MarshalAs(UnmanagedType.I4)]
        public int capturePoint;       // 점령 점수

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(GameInfo))];

            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (GameInfo)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(GameInfo));
            gch.Free();
        }
    };
    public GameInfo gameInfo;

    public static GameManager instance;
    public string[] playersTag;
    //public GameObject[] mapMode;
    public int myNetworkNum { get; set; }
    public int myIndex { get; set; }
    public int[] playersIndex { get; set; }

    public string myTag { get; set; }
    public bool myFocus { get; set; }

    public int CarSeed { get; set; }
    public int mathcingMode { get; set; }

    public CameraControl mainCamera;    // 메인 카메라
    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>();
#if NETWORK
        PlayersManager.instance.Initialization_GameInfo();              // 얻어온 정보대로 초기화
        BridgeClientToServer.instance.Initialization_PlayerViewer();    // 게임 시작시 플레이어 뷰어 셋팅

#else
        if (SceneManager.GetActiveScene().name == "MainGame_2vs2")
            gameInfo.gameType = 0;
        else if (SceneManager.GetActiveScene().name == "MainGame_1vs1")
            gameInfo.gameType = 1;

        gameInfo.gameType = 0;
        gameInfo.gameTime = 165;
        gameInfo.respawnTime = 5;
        CarSeed = 0;
        StartCoroutine(GameObject.Find("Spawner").GetComponent<PathCreation.Examples.PathSpawner>().Cor_SpawnPrefabs());
#endif
    }

    void Awake()
    {
        if (instance == null)
            instance = this;

#if NETWORK
        gameInfo = BridgeClientToServer.instance.GetTempGameInfo;   // 필요한 게임 정보를 브릿지에서 얻어옴

        playersIndex = new int[gameInfo.maxPlayer];
        playersTag = new string[gameInfo.maxPlayer];

        myNetworkNum = NetworkManager.instance.MyPlayerNum;
        myIndex = myNetworkNum - 1;

        SetPlayerTagAndIndex(gameInfo.maxPlayer);   // 태그 및 인덱스 설정
#else
        if (SceneManager.GetActiveScene().name == "MainGame_2vs2")
        {
            gameInfo.gameType = 0;
            gameInfo.maxPlayer = 4;
        }
        else if (SceneManager.GetActiveScene().name == "MainGame_1vs1")
        {
            gameInfo.gameType = 1;
            gameInfo.maxPlayer = 2;
        }
        gameInfo.maxSpeed = 4.0f;
        gameInfo.maxHealth = 100.0f;
        gameInfo.respawnTime = 3;
        gameInfo.gameTime = 180;
        gameInfo.killPoint = 25;
        gameInfo.capturePoint = 100;

        CarSeed = 0;

        playersIndex = new int[C_Global.MAX_CHARACTER];
        playersTag = new string[C_Global.MAX_CHARACTER];

        myNetworkNum = 1; //예시로 번호부여함.  서버에서 샌드된번호로 해야함.
        myIndex = myNetworkNum - 1;

        for (int i = 0; i < C_Global.MAX_CHARACTER; i++)
        {
            playersTag[i] = "Player" + (i + 1).ToString();
        }

        switch (myIndex) //0은 무조건 나, 1은팀, 2, 3은 적1, 적2
        {
            case 0:
                playersIndex[1] = 1;
                playersIndex[2] = 2;
                playersIndex[3] = 3;
                break;
        }
#endif
        myFocus = true;             // 포커스 On

        playersIndex[0] = myIndex;    // 내 인덱스 설정
        myTag = playersTag[myIndex]; // 내 태그등록
    }



    // focus말고 pause로 해야 조금 더 정확한 상황을 만들 수 있음
    private void OnApplicationPause(bool pause)
    {
        // 포커스 갖고 있던 상태에서 정지된 상태라면   패킷을 받을 수 없는 정지된 상태인 것
        if (pause == true)
        {
#if NETWORK
            // 포커스 갖고 있음
            if (myFocus == true)
            {
                NetworkManager.instance.MayIChangeFocus(false); // 포커스 잃음!(정지)
                myFocus = false;   // 포커스 off
            }
#endif
        }

        // 포커스 잃은 상태에서 정지가 풀렸다면 패킷을 받을 수 있는 정지 풀린 상태인 것
        else
        {
#if NETWORK
            // 포커스 잃었던 상태라면
            if (myFocus == false)
            {
                NetworkManager.instance.MayIChangeFocus(true); // 포커스 얻었다!(정지 풀림)
                myFocus = true;
            }
#endif
        }
    }


    // 로딩 완료
    public void LoadingComplete()
    {
#if NETWORK
      //////////////// 게임 시작 시 최초로 1회 내 위치정보를 서버로 전송해야함 /////////////////
      NetworkManager.instance.SendIngamePacket(true);

      // 1. 안들어온 놈들 꺼준다.
      GameObject notConnectedCharacter;
        for (int i = gameInfo.maxPlayer; i < C_Global.MAX_CHARACTER; i++)
        {
            notConnectedCharacter = GameObject.FindGameObjectWithTag("Player" + (i + 1).ToString());

         if (notConnectedCharacter != null)
         {
            notConnectedCharacter.SetActive(false);
         }
        }

        // 2. 아이콘도 꺼준다.
        GameObject icon = GameObject.Find("Items");
        var icons = icon.GetComponentsInChildren<bl_MiniMapItem>();
        for (int j = 0; j < icons.Length; j++)
        {
            if (icons[j].Target == null)
                icons[j].HideItem();
        }
#endif
    }

    public void SetLocalAndNetworkActionState(int _idx, _ACTION_STATE _action)
    {
        NetworkManager.instance.SetActionState(_idx, (int)_action);
        PlayersManager.instance.actionState[_idx] = _action;
    }

    public void SetPlayerTagAndIndex(int _num)
    {
        switch (gameInfo.gameType)
        {
            case (int)C_Global.GameType._2vs2:
                {
                    switch (_num)
                    {
                        case 2:
                            {
                                switch (myIndex)
                                {
                                    case 0:
                                        playersIndex[1] = 1;
                                        break;

                                    case 1:
                                        playersIndex[1] = 0;
                                        break;
                                }
                            }
                            break;

                        case 3:
                            {
                                switch (myIndex)
                                {
                                    case 0:
                                        playersIndex[1] = 1;
                                        playersIndex[2] = 2;
                                        break;

                                    case 1:
                                        playersIndex[1] = 0;
                                        playersIndex[2] = 2;
                                        break;

                                    case 2:
                                        playersIndex[1] = 0;   // 이 상황은 어쩔 수가 없음 팀이 없잖아
                                        playersIndex[2] = 1;
                                        break;
                                }
                            }
                            break;

                        case 4:
                            {
                                switch (myIndex)
                                {
                                    case 0:
                                        playersIndex[1] = 1;
                                        playersIndex[2] = 2;
                                        playersIndex[3] = 3;
                                        break;

                                    case 1:
                                        playersIndex[1] = 0;
                                        playersIndex[2] = 2;
                                        playersIndex[3] = 3;
                                        break;

                                    case 2:
                                        playersIndex[1] = 3;
                                        playersIndex[2] = 0;
                                        playersIndex[3] = 1;
                                        break;

                                    case 3:
                                        playersIndex[1] = 2;
                                        playersIndex[2] = 0;
                                        playersIndex[3] = 1;
                                        break;
                                }
                            }
                            break;
                    }

                    for (int i = 0; i < gameInfo.maxPlayer; i++)
                    {
                        playersTag[i] = "Player" + (i + 1).ToString();
                    }
                }
                break;

            case (int)C_Global.GameType._1vs1:
                {
                    switch (myIndex)
                    {
                        case 0:
                            playersIndex[1] = 1;
                            break;

                        case 1:
                            playersIndex[1] = 0;
                            break;
                    }

                    // 원래 하드코딩하면 안되는데 그냥 일단 임시로
                    playersTag[0] = "Player1";
                    playersTag[1] = "Player2";
                }
                break;
        }

    }
}