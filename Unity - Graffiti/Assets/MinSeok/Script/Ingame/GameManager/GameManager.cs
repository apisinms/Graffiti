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

        [MarshalAs(UnmanagedType.R4)]
        public float maxSpeed;     // 최대 이동속도

        [MarshalAs(UnmanagedType.R4)]
        public float maxHealth;    // 최대 체력

        [MarshalAs(UnmanagedType.I4)]
        public int responTime;     // 리스폰 시간

        [MarshalAs(UnmanagedType.I4)]
        public int gameTime;       // 게임 시간(ex 180초)

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
    public readonly string[] playersTag = new string[C_Global.MAX_PLAYER];
    public int myNetworkNum { get; set; }
    public int myIndex { get; set; }
    public int[] playersIndex { get; set; }

    public string myTag { get; set; }
    public bool myFocus { get; set; }

    public int CarSeed { get; set; }

    public CameraControl mainCamera;    // 메인 카메라

    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>();

#if NETWORK
      BridgeClientToServer.instance.Initialization_PlayerViewer();    // 게임 시작시 플레이어 뷰어 셋팅
        
      // 필요한 게임 정보를 브릿지에서 얻어옴
        gameInfo = BridgeClientToServer.instance.GetTempGameInfo;

#else
        gameInfo.gameType = 0;
        gameInfo.gameTime = 180;
        gameInfo.responTime = 5;
        CarSeed = 0;
        StartCoroutine(GameObject.Find("Spawner").GetComponent<PathCreation.Examples.PathSpawner>().Cor_SpawnPrefabs());
#endif
    }

    private void OnEnable()
    {
        // 이벤트 등록하고, 로딩 완료되면 호출하게 함
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        //로딩이 완료되었다고 서버로 보내준다.
        NetworkManager.instance.SendLoadingComplete();
    }

    void Awake()
    {
        if (instance == null)
            instance = this;

        playersTag[0] = "Player1"; playersTag[1] = "Player2";
        playersTag[2] = "Player3"; playersTag[3] = "Player4";
        playersIndex = new int[C_Global.MAX_PLAYER];

#if NETWORK
        myNetworkNum = NetworkManager.instance.MyPlayerNum;
        myIndex = myNetworkNum - 1;

#else
        myNetworkNum = 1; //예시로 번호부여함.  서버에서 샌드된번호로 해야함.
        myIndex = myNetworkNum - 1;
#endif
        myTag = playersTag[myIndex]; //내 태그등록
        myFocus = true;             // 포커스 On

        playersIndex[0] = myIndex;

        switch (myIndex) //0은 무조건 나, 1은팀, 2, 3은 적1, 적2
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

    public Image loadingImage;

    // 로딩 완료
    public void LoadingComplete()
    {
        loadingImage.enabled = false;
    }

    public void SetLocalAndNetworkActionState(int _idx, _ACTION_STATE _action)
    {
        NetworkManager.instance.SetActionState(_idx, (int)_action);
        PlayersManager.instance.actionState[_idx] = _action;
    }
}