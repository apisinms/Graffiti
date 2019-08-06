#include "SessionManager.h"
#include "C_ClientInfo.h"
#include "C_LoginState.h"

SessionManager* SessionManager::instance;
SessionManager* SessionManager::GetInstance()
{
	// 인스턴스가 없다면 인스턴스를 생성하고 리턴한다.
	if (instance == nullptr)
	{
		instance = new SessionManager();
		instance->clientList = new C_List<C_ClientInfo*>();
	}

	return instance;
}

void SessionManager::Destroy()
{
	delete instance;
}

// 받은 소켓, 주소정보로 클라이언트를 동적할당하는 함수
bool SessionManager::AddSession(SOCKET _sock, SOCKADDR_IN _addr)
{
	C_LoginState* state = new C_LoginState();
	C_ClientInfo* ptr = new C_ClientInfo(nullptr, state, _sock, _addr);
	return clientList->Insert(ptr);
}

C_ClientInfo* SessionManager::FindWithSocket(SOCKET _sock)
{
	C_ClientInfo* info = nullptr;   // 순회할 때 클라의 정보를 하나씩 담아서 활용할 변수

	   // 만약 클라이언트 리스트가 검색중이 아니라면, 리스트를 순회하며 각 set에 셋팅을 한다.
	if (clientList->SearchCheck() == false)
	{
		info = nullptr;
		clientList->SearchStart();
		while (1)
		{
			info = clientList->SearchData();   // 클라이언트를 하나씩 받아온다.

			// 끝까지 다 확인했다면 종료한다.
			if (info == nullptr)
				break;

			if (info->GetSocket() == _sock)
				break;
		}
		clientList->SearchEnd();
	}

	return info;
}

bool SessionManager::Insert(C_ClientInfo* _info)
{
	return clientList->Insert(_info);
}

bool SessionManager::Delete(C_ClientInfo* _info)
{
	return clientList->Delete(_info);
}

bool SessionManager::Remove(C_ClientInfo* _info)
{
	return clientList->Remove(_info);
}

int SessionManager::GetCount()
{
	return clientList->GetCount();
}

bool SessionManager::SearchCheck()
{
	return clientList->SearchCheck();
}

void SessionManager::SearchStart()
{
	clientList->SearchStart();
}

C_ClientInfo* SessionManager::SearchData()
{
	return clientList->SearchData();
}

void SessionManager::SearchEnd()
{
	clientList->SearchEnd();
}