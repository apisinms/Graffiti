using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using UnityEngine;


/// <summary>
/// NetworkManager_Networking.cs파일
/// 주로 네트워킹에 관련된 내용이 있다.
/// </summary>
public partial class NetworkManager : MonoBehaviour
{
    private PositionPacket tmpPosPacket = new PositionPacket();
    private Vector3 tmpVec = new Vector3();
    private Vector3 tmpAngle = new Vector3();

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

            //// 백그라운드에서도 실행되게
            //Application.runInBackground = true;

            // 처음 서버와 연결하는 부분
            IPEndPoint serverEndPoint = new IPEndPoint(serverIP, serverPort);
            tcpClient.Connect(serverEndPoint);

            // 서버와 연결 성공시 이진 쓰기, 이진 읽기용 스트림 생성
            if (tcpClient.Connected)
            {
                Debug.Log("서버 접속 성공");
                instance.br = new BinaryReader(tcpClient.GetStream());
                instance.bw = new BinaryWriter(tcpClient.GetStream());

                queue = new Queue<C_Global.QueueInfo>();    // 처리되야할 작업들을 담을 큐 생성

                ThreadManager.GetInstance.Init();
            }
        }

        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
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
        // 큐에 저장된 패킷을 꺼내온다.
        C_Global.QueueInfo info = queue.Dequeue();

        // 얻어온 패킷으로 state, protocol, result를 각각 추출한다.
        GetProtocol(info.packet, out state, out protocol, out result);

        // 얻어온 정보를 switch하여 처리한다.
        switch (state)
        {
            // Login 상태일 때
            case STATE_PROTOCOL.LOGIN_STATE:
                {
                    // Login 상태에서 프로토콜은 제외했다.(같은 값이 존재해서)


                    switch (result)
                    {
                        case RESULT.LOGIN_SUCCESS:
                        //case RESULT.JOIN_SUCCESS:
                        //case RESULT.LOGOUT_SUCCESS:
                        case RESULT.ID_EXIST:
                        //case RESULT.LOGOUT_FAIL:
                        case RESULT.ID_ERROR:
                        case RESULT.PW_ERROR:
                            {
                                lock (key)
                                {
                                    sysMsg = string.Empty;
                                    UnPackPacket(info.packet, out sysMsg);
                                    Debug.Log(sysMsg);
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
                                                UnPackPacket(info.packet, out myPlayerNum);

                                                Debug.Log(myPlayerNum);

                                                // 클라가 매칭 성공을 수신했다라는 프로토콜 셋팅
                                                PROTOCOL startProtocol = SetProtocol(
                                                      STATE_PROTOCOL.LOBBY_STATE,
                                                      PROTOCOL.START_PROTOCOL,
                                                      RESULT.NODATA);

                                                // 시작 프로토콜 + 플레이어 번호 패킹 및 전송
                                                int packetSize;
                                                PackPacket(ref sendBuf, startProtocol, out packetSize);
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
                                    UnPackPacket(info.packet, out sec);
                                    sysMsg = sec.ToString() + "초";
                                }
                            }
                            break;

							// (자신포함)상대방이 자신의 총 정보를 넘겨주면 그걸로 셋팅함
						case PROTOCOL.WEAPON_PROTOCOL:
							{
								switch(result)
								{
									case RESULT.NOTIFY_WEAPON:
										{
											int playerNum;
											WeaponPacket weapon = new WeaponPacket();

											UnPackPacket(info.packet, out playerNum, ref weapon);

											//무기정보 저장
											WeaponManager.instance.mainWeapon[playerNum - 1] = (_WEAPONS)weapon.mainW;
											WeaponManager.instance.subWeapon[playerNum - 1] = (_WEAPONS)weapon.subW;
										}
										break;
								}
							}
							break;

                        // 게임 시작 프로토콜
                        case PROTOCOL.START_PROTOCOL:
                            break;

                        // 움직임 프로토콜
                        case PROTOCOL.MOVE_PROTOCOL:
                            {
                                switch (result)
                                {
                                    // 정상 이동 시
                                    case RESULT.INGAME_SUCCESS:
                                        {
                                            lock (key)
                                            {
                                                UnPackPacket(info.packet, ref tmpPosPacket);
                                                posPacket[tmpPosPacket.playerNum - 1] = tmpPosPacket;   // 해당 플레이어 위치에 저장
                                            }
                                        }
                                        break;

                                    // 다른 플레이어가 섹터 입장 시
                                    case RESULT.ENTER_SECTOR:
                                        {
                                            lock (key)
                                            {
                                                UnPackPacket(info.packet, ref tmpPosPacket);
                                                PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].SetActive(true);   // 켜고

                                                // 이렇게 해줘야 이전 위치랑 보간을 안해서 캐릭터가 슬라이딩 되지 않음
                                                tmpVec = PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localPosition;
                                                tmpAngle = PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localEulerAngles;

                                                tmpVec.x = tmpPosPacket.posX;
                                                tmpVec.z = tmpPosPacket.posZ;
                                                tmpAngle.y = tmpPosPacket.rotY;

                                                PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localPosition = tmpVec;
                                                PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localEulerAngles = tmpAngle;
                                            }
                                        }
                                        break;

                                    // 다른 플레이어가 섹터 퇴장 시
                                    case RESULT.EXIT_SECTOR:
                                        {
                                            lock (key)
                                            {
                                                UnPackPacket(info.packet, ref tmpPosPacket);    // 사실 이 부분도 int 하나만 갖고도 될 일
                                                PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].SetActive(false);   // 끄고
                                                                                                                                    //posPacket[tmpPosPacket.playerNum - 1] = tmpPosPacket;   // 패킷 저장해주고
                                            }
                                        }
                                        break;

                                    // 섹터 입장 시 새로 입장한 인접섹터에 있는 플레이어 리스트 갱신
                                    case RESULT.UPDATE_PLAYER:
                                        {
                                            lock (key)
                                            {
                                                byte playerBit = 0;
                                                UnPackPacket(info.packet, out playerBit);

                                                // 다른 클라의 위치 요청 프로토콜
                                                PROTOCOL reqProtocol = SetProtocol(
                                                      STATE_PROTOCOL.INGAME_STATE,
                                                      PROTOCOL.MOVE_PROTOCOL,
                                                      RESULT.GET_OTHERPLAYER_POS);

                                                // 마스크 만들어서 어떤 플레이어가 같은 섹터에 있는지 확인하고, 오브젝트를 켜고 끔
                                                byte bitMask = (byte)PLAYER_BIT.PLAYER_1;
                                                int packetSize;
                                                for (int i = 0; i < C_Global.MAX_PLAYER; i++, bitMask >>= 1)
                                                {
                                                    // 본인은 걍 건너 뜀
                                                    if ((myPlayerNum - 1) == i)
                                                        continue;

                                                    // 섹터에 포함되어있는 플레이어인데
                                                    if ((playerBit & bitMask) > 0)
                                                    {
                                                        // 꺼져있는 플레이어라면 위치를 넘버로 요청한다.(얘네들만 갱신해주면 됨)
                                                        if (PlayersManager.instance.obj_players[i].activeSelf == false)
                                                        {
                                                            PackPacket(ref sendBuf, reqProtocol, (i + 1), out packetSize);
                                                            bw.Write(sendBuf, 0, packetSize);
                                                        }

                                                        // 켜져있다면 굳이 위치를 받아올 필요 없다.
                                                        else
                                                            continue;

                                                    }

                                                    // 섹터에 포함되어 있지 않다면 오브젝트를 꺼준다.(오브젝트가 켜진 경우만)
                                                    else
                                                    {
                                                        if (PlayersManager.instance.obj_players[i].activeSelf == true)
                                                            PlayersManager.instance.obj_players[i].SetActive(false);
                                                    }
                                                }
                                            }
                                        }
                                        break;

                                    case RESULT.FORCE_MOVE:
                                        {
                                            lock (key)
                                            {
                                                UnPackPacket(info.packet, ref tmpPosPacket);    // 포지션 패킷 가져옴

                                                // 스피드, 애니메이션 0으로
                                                tmpPosPacket.speed = 0.0f;
                                                tmpPosPacket.action = (int)_ACTION_STATE.IDLE;

                                                posPacket[tmpPosPacket.playerNum - 1] = tmpPosPacket;   // 이전 위치 패킷에 저장해줌

                                                // 원래 플레이어가 가지고 있던 정보를 임시 벡터에 가져옴
                                                tmpVec = PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localPosition;
                                                tmpAngle = PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localEulerAngles;

                                                // 변환된 값을 바로 대입
                                                tmpVec.x = tmpPosPacket.posX;
                                                tmpVec.z = tmpPosPacket.posZ;
                                                tmpAngle.y = tmpPosPacket.rotY;

                                                // 러프없이 강제로 대입해버림
                                                PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localPosition = tmpVec;
                                                PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localEulerAngles = tmpAngle;

                                                // 본인일 경우에는 카메라도 강제 셋팅 시켜준다.
                                                if (tmpPosPacket.playerNum == myPlayerNum)
                                                {
                                                    // 카메라도 설정
                                                    if (GameManager.instance.mainCamera != null)
                                                        GameManager.instance.mainCamera.SetCameraPos(0.0f, C_Global.camPosY, C_Global.camPosZ);
                                                }
                                            }
                                        }
                                        break;

                                    case RESULT.GET_OTHERPLAYER_POS:
                                        {
                                            lock (key)
                                            {
                                                UnPackPacket(info.packet, ref tmpPosPacket);            // 패킷을 받고
                                                posPacket[tmpPosPacket.playerNum - 1] = tmpPosPacket;   // 패킷 저장해주고

                                                // 플레이어 위치를 바로 대입해서 셋팅해버림
                                                tmpVec = PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localPosition;
                                                tmpAngle = PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localEulerAngles;

                                                tmpVec.x = tmpPosPacket.posX;
                                                tmpVec.z = tmpPosPacket.posZ;
                                                tmpAngle.y = tmpPosPacket.rotY;

                                                PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localPosition = tmpVec;
                                                PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localEulerAngles = tmpAngle;

                                                // 꺼져있다면 켜준다!
                                                if (PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].activeSelf == false)
                                                    PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].SetActive(true);
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
                                    UnPackPacket(info.packet, ref tmpPosPacket);         // 포지션 패킷 가져옴
                                    posPacket[tmpPosPacket.playerNum - 1] = tmpPosPacket;   // 이전 위치 패킷에 저장해줌

                                    // 원래 플레이어가 가지고 있던 정보를 임시 벡터에 가져옴
                                    tmpVec = PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localPosition;
                                    tmpAngle = PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localEulerAngles;

                                    // 변환된 값을 바로 대입
                                    tmpVec.x = tmpPosPacket.posX;
                                    tmpVec.z = tmpPosPacket.posZ;
                                    tmpAngle.y = tmpPosPacket.rotY;

                                    // 러프없이 강제로 대입해버림
                                    PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localPosition = tmpVec;
                                    PlayersManager.instance.obj_players[tmpPosPacket.playerNum - 1].transform.localEulerAngles = tmpAngle;
                                }
                            }
                            break;

                        // 다른사람 끊김 프로토콜
                        case PROTOCOL.DISCONNECT_PROTOCOL:
                            {
                                lock (key)
                                {
                                    // 끊긴 놈을 꺼준다.
                                    UnPackPacket(info.packet, out quitPlayerNum);

                                    if (PlayersManager.instance.obj_players[quitPlayerNum - 1].activeSelf == true)
                                        PlayersManager.instance.obj_players[quitPlayerNum - 1].SetActive(false);
                                }
                            }
                            break;
                    }
                }
                break;
        }
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