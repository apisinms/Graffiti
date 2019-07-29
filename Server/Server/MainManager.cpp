#include "MainManager.h"
#include "C_Encrypt.h"
#include "DatabaseManager.h"
#include "LogManager.h"
#include "SessionManager.h"
#include "LoginManager.h"
#include "LobbyManager.h"
#include "ChatManager.h"
#include "C_ClientInfo.h"
#include "UtilityManager.h"
#include "RoomManager.h"
#include "MatchManager.h"
#include <locale.h>

// 초기화
MainManager* MainManager::instance;			// 싱글톤을 위한 정적 포인터

// 싱글톤 생성
MainManager* MainManager::GetInstance()
{
	// 인스턴스가 없다면 인스턴스를 생성하고 리턴한다.
	if (instance == nullptr)
		instance = new MainManager();
	
	return instance;
}

// 소켓 초기화
void MainManager::Init()
{
	IC_CS cs;

	setlocale(LC_ALL, "Korean");	// ★★★★★한국어로 설정★★★★★

	// 윈속 초기화
	WSADATA wsa;
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
		LogManager::GetInstance()->ErrQuitMsgBox("WSAStartup()");

	// socket()
	listenSock = new C_Socket();
	listenSock->Socket(AF_INET, SOCK_STREAM, 0);

	// bind()
	listenSock->Bind(AF_INET, INADDR_ANY, SERVERPORT);	// 리턴값으로 딱히 처리하지 않음(어차피 err_quit라)

	// listen()
	listenSock->Listen(SOMAXCONN);

	IOCP_Init();
	DatabaseManager::GetInstance()->Init();	// DB매니저에서 초기화 하는 작업 수행
	LoginManager::GetInstance()->Init();	// 로그인 매니저에서 초기화 해야하는 작업을 진행한다(예:회원가입 리스트 불러오기)
	LobbyManager::GetInstance()->Init();
	RoomManager::GetInstance()->Init();
	MatchManager::GetInstance()->Init();
	ChatManager::GetInstance()->Init();

	SetConsoleCtrlHandler((PHANDLER_ROUTINE)CtrlHandler, TRUE);	// 종료를 감지하는 핸들러 함수 등록
}

// 통신
void MainManager::Run()
{
	// 여기 메인쓰레드에서 클라의 Accept()를 수행한다.
	while (1)
	{
		SOCKET clientSock = listenSock->Accept();

		// 소켓과 입출력 완료 포트 등록
		if (IOCP_Register((HANDLE)clientSock, NULL) == true)
		{
			// 오버랩 구조체를 초기화한뒤, 완료 패킷을 보낸다.
			WSAOVERLAPPED_EX overlapped;
			memset(&overlapped, 0, sizeof(WSAOVERLAPPED_EX));
			overlapped.ptr = SessionManager::GetInstance()->FindWithSocket(clientSock);
			overlapped.type = IO_TYPE::IO_ACCEPT;
			PostQueuedCompletionStatus(hcp, -1, NULL, (LPOVERLAPPED)&overlapped);
		}
	}
}

// 이 Destroy 함수에서 싱글톤 객체들의 Destroy도 호출하여 자원을 반납한다.
void MainManager::Destroy()
{
	C_Encrypt::GetInstance()->Destroy();
	//FileManager<UserInfo>::GetInstance()->Destroy();
	UtilityManager::GetInstance()->Destroy();
	DatabaseManager::GetInstance()->Destroy();
	LogManager::GetInstance()->Destroy();
	LoginManager::GetInstance()->Destroy();
	LobbyManager::GetInstance()->Destroy();
	RoomManager::GetInstance()->Destroy();
	MatchManager::GetInstance()->Destroy();
	ChatManager::GetInstance()->Destroy();
	SessionManager::GetInstance()->Destroy();

	MessageBeep(0);	// 잘 종료되나 메시지비프 울림
	delete instance;
}

void MainManager::IOCP_Accept(void* _ptr)
{
	C_ClientInfo* ptr = (C_ClientInfo*)_ptr;
	
	// accept가 되고나면 바로 비동기 Recv를 해준다.
	ptr->WSA_Recv(nullptr);
}
void MainManager::IOCP_Read(void* _ptr, int _len)
{
	C_ClientInfo* ptr = (C_ClientInfo*)_ptr;

	int result = ptr->CompleteRecv(_len);

	switch (result)
	{
	case SOC_ERROR:
		IOCP_Disconnected(ptr);
		break;
	case SOC_FALSE:
		break;
	
		// 모두 받았을 때(SOC_TRUE) 바로 WSA_Recv를 다시 켜준다.(Recv하고나서 Recv를 켜주는게 맞다)
	case SOC_TRUE:
		ptr->GetCurrentState()->Read(ptr);
		if (!ptr->WSA_Recv(nullptr))
		{
			LogManager::GetInstance()->ErrorPrintf("WSA_Recv()");
			IOCP_Disconnected(ptr);
		}
		break;
	}
	
}
void MainManager::IOCP_Write(void* _ptr, int _len)
{
	C_ClientInfo* ptr = (C_ClientInfo*)_ptr;

	int result = ptr->CompleteSend(_len);

	switch (result)
	{
	case SOC_ERROR:
		IOCP_Disconnected(ptr);
		break;
		// 다 못보낸 경우
	case SOC_FALSE:
		break;
		// 다 보낸 경우
	case SOC_TRUE:
		ptr->GetCurrentState()->Write(ptr);
		break;
	}
}

void MainManager::End()
{
	LoginManager::GetInstance()->End();
	LobbyManager::GetInstance()->End();
	RoomManager::GetInstance()->End();
	MatchManager::GetInstance()->End();
	ChatManager::GetInstance()->End();
	DatabaseManager::GetInstance()->End();
	LogManager::GetInstance()->End();
	IOCP_End();
}

void MainManager::IOCP_Disconnected(void* _ptr)
{
	C_ClientInfo* ptr = (C_ClientInfo*)_ptr;

	// 접속 로그에 기록해줌
	LogManager::GetInstance()->ConnectFileWrite("[TCP 서버] 클라이언트 종료: IP 주소=%s, 포트 번호=%d\n",
		inet_ntoa(ptr->GetAddress().sin_addr), ntohs(ptr->GetAddress().sin_port));

	
	//LobbyManager::GetInstance()->CheckLeaveRoom(ptr);	// 방삭제
	LoginManager::GetInstance()->LoginListDelete(ptr);	// 로그인 목록에서도 지워준다.
	SessionManager::GetInstance()->Remove(ptr);			// 데이터까지 완전 종료되는 Remove를 호출
	
}

BOOL WINAPI MainManager::CtrlHandler(DWORD _fdwCtrlType)
{
	switch (_fdwCtrlType)
	{
		// 종료를 감지하는 모든 값이 들어오면 뒷정리한다.
	case CTRL_C_EVENT:
	case CTRL_CLOSE_EVENT:
	case CTRL_LOGOFF_EVENT:
	case CTRL_SHUTDOWN_EVENT:
	case CTRL_BREAK_EVENT:
	default:
		MainManager::GetInstance()->End();
		MainManager::GetInstance()->Destroy();
		break;
	}

	return TRUE;
}