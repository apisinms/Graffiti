using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using KetosGames.SceneTransition;
using static C_Global;
using static GameManager;
using static WeaponManager;


/// <summary>
/// NetworkManager_Networking.cs파일
/// 주로 네트워킹에 관련된 내용이 있다.
/// </summary>
public partial class NetworkManager : MonoBehaviour
{
    private BridgeClientToServer bridge;
    private IngamePacket tmpIngamePacket = new IngamePacket();
    private Vector3 tmpVec = new Vector3();
    private Vector3 tmpAngle = new Vector3();
    private QueueInfo[] info = new QueueInfo[MAX_QUEUE_SIZE];

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // 스크린 가로모드 고정
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;

            // 60프레임 설정
            Application.targetFrameRate = 60;


            // 절전모드 안되게
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // 백그라운드에서도 실행되게
            Application.runInBackground = true;

            // 처음 서버와 연결하는 부분
            IPEndPoint serverEndPoint = new IPEndPoint(serverIP, serverPort);
            tcpClient.Connect(serverEndPoint);

            // 서버와 연결 성공시 이진 쓰기, 이진 읽기용 스트림 생성
            if (tcpClient.Connected)
            {
                Debug.Log("서버 접속 성공");
                instance.br = new BinaryReader(tcpClient.GetStream());
                instance.bw = new BinaryWriter(tcpClient.GetStream());

                queue = new Queue<QueueInfo>();    // 처리되야할 작업들을 담을 큐 생성

                ThreadManager.GetInstance.Init();
            }
        }

        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (bridge == null)
            bridge = BridgeClientToServer.instance;
    }

    private void Update()
    {
        // 클라이언트가 연결되었고, 큐에 내용이 있다면 RecvProcess를 호출하여 recv에 관한 처리를 한다.
        if (tcpClient.Connected && queue.Count > 0)
            RecvProcess();

        // 인터넷이 끊기면 어플을 종료함(나중에 메시지박스 같은거 띄우고 확인버튼 누르면 종료시키면 될듯)
        if (Application.internetReachability == NetworkReachability.NotReachable)
            Application.Quit();
    }


    private void RecvProcess()
    {
        // 큐에 저장된 '모든' 패킷을 꺼내온다.
        for (int i = 0; i < queue.Count; i++)
        {
            /*// 만약에 큐에 쌓인게 (최대 큐사이즈 / 2)보다 많다면 이전에 쌓인건 걍 스킵한다.
            if(queue.Count >= (MAX_QUEUE_SIZE * 0.5)
               && queue.Count < MAX_QUEUE_SIZE)
            {
               // 이전에 있던것들 절반은 다 없애버림 걍
               for (int j = 0; j < (MAX_QUEUE_SIZE * 0.5); j++)
                  queue.Dequeue();

               //i = (int)MAX_QUEUE_SIZE * 0.5;
               i = (int)MAX_QUEUE_SIZE / 2;

               continue;   // 건너 뜀
            }*/

            info[i] = queue.Dequeue();   // 하나씩 꺼내와서 처리

            // 얻어온 패킷으로 state, protocol, result를 각각 추출한다.
            GetProtocol(info[i].packet, out state, out protocol, out result);

            // 얻어온 정보를 switch하여 처리한다.
            switch (state)
            {
                // Login 상태일 때
                case STATE_PROTOCOL.LOGIN_STATE:
                    {
                        switch (protocol)
                        {
                            // 회원가입
                            case PROTOCOL.JOIN_PROTOCOL:
                                {
                                    switch (result)
                                    {
                                        case RESULT.JOIN_SUCCESS:
                                        case RESULT.ID_EXIST:
                                            {
                                                lock (key)
                                                {
                                                    sysMsg = string.Empty;
                                                    UnPackPacket(info[i].packet, out sysMsg);
                                                    Debug.Log(sysMsg);
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;

                            // 로그인
                            case PROTOCOL.LOGIN_PROTOCOL:
                                {
                                    switch (result)
                                    {
                                        // 로그인 성공하면 닉네임을 보내준다.
                                        case RESULT.LOGIN_SUCCESS:
                                            {
                                                lock (key)
                                                {
                                                    sysMsg = string.Empty;
                                                    UnPackPacket(info[i].packet, out sysMsg, out nickName);
                                                    Debug.Log(sysMsg);
                                                }
                                            }
                                            break;

                                        case RESULT.ID_EXIST:
                                        case RESULT.ID_ERROR:
                                        case RESULT.PW_ERROR:
                                            {
                                                lock (key)
                                                {
                                                    sysMsg = string.Empty;
                                                    UnPackPacket(info[i].packet, out sysMsg);
                                                    Debug.Log(sysMsg);
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;

                // Lobby 상태일 때
                case STATE_PROTOCOL.LOBBY_STATE:
                    {
                        switch (protocol)
                        {
                            case PROTOCOL.GOTO_INGAME_PROTOCOL:
                                {
                                    switch (result)
                                    {
                                        case RESULT.LOBBY_SUCCESS:
                                            {
                                                lock (key)
                                                {
                                                    UnPackPacket(info[i].packet, out myPlayerNum);

                                                    Debug.Log(myPlayerNum);

                                                    // 클라가 매칭 성공을 수신했다라는 프로토콜 셋팅
                                                    PROTOCOL gotoIngameProtocol = SetProtocol(
                                                        STATE_PROTOCOL.LOBBY_STATE,
                                                        PROTOCOL.GOTO_INGAME_PROTOCOL,
                                                        RESULT.NODATA);

                                                    // 시작 프로토콜 + 플레이어 번호 패킹 및 전송
                                                    int packetSize;
                                                    PackPacket(ref sendBuf, gotoIngameProtocol, out packetSize);
                                                    bw.Write(sendBuf, 0, packetSize);

                                                    Debug.Log("매칭 성공!!");
                                                }
                                            }
                                            break;

                                        case RESULT.LOBBY_FAIL:
                                            Debug.Log("매칭 실패!!");
                                            break;
                                    }
                                }
                                break;

                            case PROTOCOL.MATCH_CANCEL_PROTOCOL:
                                {
                                    switch (result)
                                    {
                                        case RESULT.LOBBY_SUCCESS:
                                            Debug.Log("매칭 취소 성공!");
                                            break;

                                        case RESULT.LOBBY_FAIL:
                                            Debug.Log("매칭 취소 실패!");
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;

                case STATE_PROTOCOL.INGAME_STATE:
                    {
                        switch (protocol)
                        {
                            // 타이머 프로토콜이 넘겨져오면
                            case PROTOCOL.TIMER_PROTOCOL:
                                {
                                    // result 생략

                                    // 넘겨온 초를 string으로 변환해서 sysMsg에 저장한다.
                                    lock (key)
                                    {
                                        sysMsg = string.Empty;

                                        int sec = 0;
                                        UnPackPacket(info[i].packet, out sec);
                                        sysMsg = sec.ToString() + "초";
                                    }
                                }
                                break;

                            // (자신포함)상대방이 자신의 총 정보를 넘겨주면 그걸로 셋팅함
                            case PROTOCOL.WEAPON_PROTOCOL:
                                {
                                    switch (result)
                                    {
                                        case RESULT.NOTIFY_WEAPON:
                                            {
                                                lock (key)
                                                {
                                                    int playerNum;
                                                    WeaponPacket weapon = new WeaponPacket();

                                                    UnPackPacket(info[i].packet, out playerNum, ref weapon);
                                                    bridge.SetWeapon(playerNum, ref weapon);
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;

                            // 모든 플레이어의 닉네임을 설정함(본인 포함)
                            case PROTOCOL.NICKNAME_PROTOCOL:
                                {
                                    lock (key)
                                    {
                                        int playerNum;
                                        string nickName;
                                        UnPackPacket(info[i].packet, out playerNum, out nickName);

                                        bridge.SetPlayerNickName(nickName, (playerNum - 1));
                                    }
                                }
                                break;

                            // 게임 시작 프로토콜
                            case PROTOCOL.START_PROTOCOL:
                                {
                                    // result 생략

                                    lock (key)
                                    {
                                        // 모든 게임 정보를 받아가지고 옴
                                        GameInfo gameInfo = new GameInfo();
                                        WeaponInfo[] weapons = null;

                                        UnPackPacket(info[i].packet, ref gameInfo, ref weapons);

                                        // 받은 정보를 브릿지에 세팅함
                                        bridge.SetGameInfoToBridge(ref gameInfo, ref weapons);

                                        // 패킷 구조체를 플레이어 수만큼 할당
                                        ingamePackets = new IngamePacket[gameInfo.maxPlayer];
                                    }
                                }
                                break;

                            // 로딩 프로토콜
                            case PROTOCOL.LOADING_PROTOCOL:
                                {
                                    switch (result)
                                    {
                                        // 모두가 로딩 되었다면
                                        case RESULT.INGAME_SUCCESS:
                                            {
                                                lock (key)
                                                {
                                                    SceneLoader.Instance.waitOtherPlayer = true;
                                                    GameManager.instance.LoadingComplete();
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;

                            // 움직임 프로토콜
                            case PROTOCOL.UPDATE_PROTOCOL:
                                {
                                    switch (result)
                                    {
                                        // 정상적인 업데이트 시
                                        case RESULT.INGAME_SUCCESS:
                                            {
                                                lock (key)
                                                {
                                                    UnPackPacket(info[i].packet, ref tmpIngamePacket);
                                                    bridge.OnUpdate(ref tmpIngamePacket);
                                                }
                                            }
                                            break;

                                        // 다른 플레이어가 섹터 입장 시
                                        case RESULT.ENTER_SECTOR:
                                            {
                                                lock (key)
                                                {
                                                    UnPackPacket(info[i].packet, ref tmpIngamePacket);
                                                    bridge.EnterSectorProcess(ref tmpIngamePacket);
                                                }
                                            }
                                            break;

                                        // 다른 플레이어가 섹터 퇴장 시
                                        case RESULT.EXIT_SECTOR:
                                            {
                                                lock (key)
                                                {
                                                    UnPackPacket(info[i].packet, ref tmpIngamePacket);    // 사실 이 부분도 int 하나만 갖고도 될 일
                                                    bridge.ExitSectorProcess(ref tmpIngamePacket);
                                                }
                                            }
                                            break;

                                        // 섹터 입장 시 새로 입장한 인접섹터에 있는 플레이어 리스트 갱신
                                        case RESULT.UPDATE_PLAYER:
                                            {
                                                lock (key)
                                                {
                                                    byte playerBit = 0;
                                                    UnPackPacket(info[i].packet, out playerBit);

                                                    bridge.UpdatePlayerProcess(playerBit);
                                                }
                                            }
                                            break;

                                        case RESULT.FORCE_MOVE:
                                            {
                                                lock (key)
                                                {
                                                    UnPackPacket(info[i].packet, ref tmpIngamePacket);    // 포지션 패킷 가져옴
                                                    bridge.ForceMoveProcess(ref tmpIngamePacket);
                                                }
                                            }
                                            break;

                                        case RESULT.GET_OTHERPLAYER_STATUS:
                                            {
                                                lock (key)
                                                {
                                                    UnPackPacket(info[i].packet, ref tmpIngamePacket);            // 패킷을 받고
                                                    bridge.GetOtherPlayerStatus(ref tmpIngamePacket);

                                                }
                                            }
                                            break;

                                        case RESULT.BULLET_HIT:
                                            {
                                                lock (key)
                                                {
                                                    UnPackPacket(info[i].packet, ref tmpIngamePacket);       // 패킷을 받고

                                                    // 지금 받은 체력이 더 낮아야만 체력을 업데이트 해준다. 그리고 죽었으면 업데이트 안한다.
                                                    if (playersManager.hp[tmpIngamePacket.playerNum - 1] > tmpIngamePacket.health
                                                       && playersManager.actionState[tmpIngamePacket.playerNum - 1] != _ACTION_STATE.DEATH)
                                                    {
                                                        bridge.HealthChanger(ref tmpIngamePacket);
                                                    }
                                                }
                                            }
                                            break;

                                        case RESULT.RESPAWN:
                                            {
                                                lock (key)
                                                {
                                                    UnPackPacket(info[i].packet, ref tmpIngamePacket);
                                                    bridge.RespawnProcess(ref tmpIngamePacket);
                                                }
                                            }
                                            break;

                                        // 차 스폰
                                        case RESULT.CAR_SPAWN:
                                            {
                                                lock (key)
                                                {
                                                    int seed = 0;

                                                    UnPackPacket(info[i].packet, out seed);
                                                    bridge.CarSpawn(seed);
                                                }
                                            }
                                            break;

                                        // 다른 플레이어 뺑소니 당함
                                        case RESULT.CAR_HIT:
                                            {
                                                lock (key)
                                                {
                                                    int playerNum;
                                                    float posX, posZ;

                                                    UnPackPacket(info[i].packet, out playerNum, out posX, out posZ);
                                                    bridge.OtherPlayerHitByCar(playerNum, posX, posZ);
                                                }
                                            }
                                            break;
                                        // 누군가가 총에 맞아 숨짐
                                        case RESULT.KILL:
                                            {
                                                lock (key)
                                                {
                                                    int killer, victim;

                                                    UnPackPacket(info[i].packet, out killer, out victim);
                                                    bridge.KillProcess(killer, victim);
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;

                            // 포커스 프로토콜(추후에 게임 정보가 추가된다면, 위치정보 뿐만 아니라 체력이나 기타 정보도 업데이트 해줘야한다!)
                            case PROTOCOL.FOCUS_PROTOCOL:
                                {
                                    // result 생략

                                    lock (key)
                                    {
                                        UnPackPacket(info[i].packet, ref tmpIngamePacket);         // 포지션 패킷 가져옴
                                        bridge.ForceMoveProcess(ref tmpIngamePacket);
                                    }
                                }
                                break;

                            case PROTOCOL.CAPTURE_PROTOCOL:
                                {
                                    switch (result)
                                    {
                                        case RESULT.BONUS:
                                            {
                                                lock (key)
                                                {
                                                    int score1, score2;

                                                    UnPackPacket(info[i].packet, out score1, out score2);   // 점령 보너스 받아옴
                                                    Debug.Log("팀1 점령점수 : " + score1 + ", 팀2 점령점수 : " + score2);
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;

                            // 다른사람 끊김 프로토콜
                            case PROTOCOL.DISCONNECT_PROTOCOL:
                                {
                                    switch (result)
                                    {
                                        // 강종
                                        case RESULT.ABORT:
                                            {
                                                lock (key)
                                                {
                                                    // 끊긴 놈을 꺼준다.
                                                    UnPackPacket(info[i].packet, out quitPlayerNum);
                                                    bridge.OnOtherPlayerDisconnected(quitPlayerNum);
                                                }
                                            }
                                            break;

                                        // 무기 선택 도중 종료
                                        case RESULT.WEAPON_SEL:
                                            {
                                                lock (key)
                                                {
                                                    SceneManager.LoadScene("LobbyMenuScene");

                                                    // 다시 서버로 잘 받았다고 보낸다.(서버에서도 이 클라의 상태를 바꿔줘야 함)
                                                    SendGotoLobby();
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }

    }

    // 다른 플레이어 위치 얻기 요청
    public void RequestOtherPlayerStatus(int _playerNum)
    {
        int packetSize = 0;

        // 다른 클라의 위치 요청 프로토콜
        PROTOCOL reqProtocol = SetProtocol(
           STATE_PROTOCOL.INGAME_STATE,
           PROTOCOL.UPDATE_PROTOCOL,
           RESULT.GET_OTHERPLAYER_STATUS);

        // 플레이어라면 위치를 요청한다.
        PackPacket(ref sendBuf, reqProtocol, _playerNum, out packetSize);
        bw.Write(sendBuf, 0, packetSize);
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void Disconnect()
    {
        // 뒷정리
        bw.Close();
        br.Close();
        tcpClient.Close();
    }
}