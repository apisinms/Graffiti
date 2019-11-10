#include "stdafx.h"
#include "MainManager.h"
#include "DatabaseManager.h"
#include "LoginManager.h"
#include "LobbyManager.h"
#include "ChatManager.h"
#include "C_ClientInfo.h"
#include "RoomManager.h"
#include "MatchManager.h"
#include "InGameManager.h"
#include "RandomManager.h"
//#include <locale.h>

// �ʱ�ȭ
MainManager* MainManager::instance;			// �̱����� ���� ���� ������

// �̱��� ����
MainManager* MainManager::GetInstance()
{
	// �ν��Ͻ��� ���ٸ� �ν��Ͻ��� �����ϰ� �����Ѵ�.
	if (instance == nullptr)
		instance = new MainManager();
	
	return instance;
}

// ���� �ʱ�ȭ
void MainManager::Init()
{
	IC_CS cs;

	setlocale(LC_ALL, "Korean");	// �ڡڡڡڡ��ѱ���� �����ڡڡڡڡ�

	// ���� �ʱ�ȭ
	WSADATA wsa;
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
		LogManager::GetInstance()->ErrQuitMsgBox("WSAStartup()");

	// socket()
	listenSock = new C_Socket();
	listenSock->Socket(AF_INET, SOCK_STREAM, 0);

	// bind()
	listenSock->Bind(AF_INET, INADDR_ANY, SERVERPORT);	// ���ϰ����� ���� ó������ ����(������ err_quit��)

	// listen()
	listenSock->Listen(SOMAXCONN);

	IOCP_Init();
	RandomManager::GetInstance()->Init();
	LogManager::GetInstance()->Init();
	UtilityManager::GetInstance()->Init();
	C_Encrypt::GetInstance()->Init();
	SessionManager::GetInstance()->Init();
	DatabaseManager::GetInstance()->Init();	// DB�Ŵ������� �ʱ�ȭ �ϴ� �۾� ����
	LoginManager::GetInstance()->Init();	// �α��� �Ŵ������� �ʱ�ȭ �ؾ��ϴ� �۾��� �����Ѵ�(��:ȸ������ ����Ʈ �ҷ�����)
	LobbyManager::GetInstance()->Init();
	RoomManager::GetInstance()->Init();
	MatchManager::GetInstance()->Init();
	ChatManager::GetInstance()->Init();
	InGameManager::GetInstance()->Init();

	SetConsoleCtrlHandler((PHANDLER_ROUTINE)CtrlHandler, TRUE);	// ���Ḧ �����ϴ� �ڵ鷯 �Լ� ���
}

void MainManager::End()
{
	InGameManager::GetInstance()->End();
	ChatManager::GetInstance()->End();
	MatchManager::GetInstance()->End();
	RoomManager::GetInstance()->End();
	LobbyManager::GetInstance()->End();
	LoginManager::GetInstance()->End();	// �α��� �Ŵ������� �ʱ�ȭ �ؾ��ϴ� �۾��� �����Ѵ�(��:ȸ������ ����Ʈ �ҷ�����)
	DatabaseManager::GetInstance()->End();	// DB�Ŵ������� �ʱ�ȭ �ϴ� �۾� ����
	SessionManager::GetInstance()->End();
	C_Encrypt::GetInstance()->End();
	UtilityManager::GetInstance()->End();
	LogManager::GetInstance()->End();
	RandomManager::GetInstance()->End();
	IOCP_End();
}

// ���
void MainManager::Run()
{
	// ���� ���ξ����忡�� Ŭ���� Accept()�� �����Ѵ�.
	while (1)
	{
		SOCKET clientSock = listenSock->Accept();

		// ���ϰ� ����� �Ϸ� ��Ʈ ���
		if (IOCP_Register((HANDLE)clientSock, NULL) == true)
		{
			// ������ ����ü�� �ʱ�ȭ�ѵ�, �Ϸ� ��Ŷ�� ������.
			WSAOVERLAPPED_EX overlapped;
			memset(&overlapped, 0, sizeof(WSAOVERLAPPED_EX));
			overlapped.ptr = SessionManager::GetInstance()->FindWithSocket(clientSock);
			overlapped.type = IO_TYPE::IO_ACCEPT;
			PostQueuedCompletionStatus(hcp, -1, NULL, (LPOVERLAPPED)&overlapped);
		}
	}
}

// �� Destroy �Լ����� �̱��� ��ü���� Destroy�� ȣ���Ͽ� �ڿ��� �ݳ��Ѵ�.
void MainManager::Destroy()
{
	InGameManager::GetInstance()->Destroy();
	ChatManager::GetInstance()->Destroy();
	MatchManager::GetInstance()->Destroy();
	RoomManager::GetInstance()->Destroy();
	LobbyManager::GetInstance()->Destroy();
	LoginManager::GetInstance()->Destroy();	// �α��� �Ŵ������� �ʱ�ȭ �ؾ��ϴ� �۾��� �����Ѵ�(��:ȸ������ ����Ʈ �ҷ�����)
	DatabaseManager::GetInstance()->Destroy();	// DB�Ŵ������� �ʱ�ȭ �ϴ� �۾� ����
	SessionManager::GetInstance()->Destroy();
	C_Encrypt::GetInstance()->Destroy();
	UtilityManager::GetInstance()->Destroy();
	LogManager::GetInstance()->Destroy();
	RandomManager::GetInstance()->Destroy();

	MessageBeep(0);	// �� ����ǳ� �޽������� �︲
	delete instance;
}

void MainManager::IOCP_Accept(void* _ptr)
{
	IC_CS cs;	// ����ȭ

	C_ClientInfo* ptr = (C_ClientInfo*)_ptr;
	
	// accept�� �ǰ��� �ٷ� �񵿱� Recv�� ���ش�.
	ptr->WSA_Recv(nullptr);
}
void MainManager::IOCP_Read(void* _ptr, int _len)
{
	IC_CS cs;	// ����ȭ �ʼ�

	C_ClientInfo* ptr = (C_ClientInfo*)_ptr;

	int result = ptr->CompleteRecv(_len);

	switch (result)
	{
	case SOC_ERROR:
		IOCP_Disconnected(ptr);
		break;
	case SOC_FALSE:
		break;
	
		// ��� �޾��� ��(SOC_TRUE) �ٷ� WSA_Recv�� �ٽ� ���ش�.(Recv�ϰ��� Recv�� ���ִ°� �´�)
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
	IC_CS cs;		/// ����ȭ �ʼ� (���⿡�� ���ϸ� ���� �Դµ� �ϴ� �׽�Ʈ�� �ּ�ó���غô�)

	C_ClientInfo* ptr = (C_ClientInfo*)_ptr;

	int result = ptr->CompleteSend(_len);

	switch (result)
	{
	case SOC_ERROR:
		IOCP_Disconnected(ptr);
		break;
		// �� ������ ���
	case SOC_FALSE:
		break;
		// �� ���� ���
	case SOC_TRUE:
		ptr->GetCurrentState()->Write(ptr);
		break;
	}
}
void MainManager::IOCP_Disconnected(void* _ptr)
{
	IC_CS cs;	// ����ȭ
	InGameManager::IC_CS ingameManagerCS;	// �ΰ��� �Ŵ����� ����ȭ! �߿�!!

	C_ClientInfo* ptr = (C_ClientInfo*)_ptr;

	// �ּ� ������ ��Ʈ�� ���� 0�̶�� �̹� �������� �ʱ�ȭ �� �����̴�. �Լ� ����������.
	if (ptr->GetAddress().sin_port == 0)
	{
		return;
	}

	// ���� �α׿� �������
	LogManager::GetInstance()->ConnectFileWrite("[TCP ����] Ŭ���̾�Ʈ ����: IP �ּ�=%s, ��Ʈ ��ȣ=%d\n",
		inet_ntoa(ptr->GetAddress().sin_addr), ntohs(ptr->GetAddress().sin_port));

	printf("[TCP ����] Ŭ���̾�Ʈ ����: IP �ּ�=%s, ��Ʈ ��ȣ=%d\n",
		inet_ntoa(ptr->GetAddress().sin_addr), ntohs(ptr->GetAddress().sin_port));
	
	// 1. �α��� ��Ͽ��� ����
	// 2. �������ť(or����Ʈ)���� ����
	// 3. �� ��Ͽ��� ����
	// + ���� ���� ���� ������ ��� ���� �ٽ� ���?
	// + ���� �������� ���¿��ٸ� �ش� ���� �� + ���� ����?


	LoginManager::GetInstance()->LoginListDelete(ptr);	// �α��� ��Ͽ� �ִٸ� �����ش�.
	MatchManager::GetInstance()->WaitListRemove(ptr);	// ��Ī ��� ��Ͽ� �ִٸ� �����ش�.
	RoomManager::GetInstance()->CheckLeaveRoom(ptr);	// �濡 �ִٸ� �� �������� ������
	SessionManager::GetInstance()->Remove(ptr);			// ���� �Ŵ������� ����
}

BOOL WINAPI MainManager::CtrlHandler(DWORD _fdwCtrlType)
{
	switch (_fdwCtrlType)
	{
		// ���Ḧ �����ϴ� ��� ���� ������ �������Ѵ�.
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