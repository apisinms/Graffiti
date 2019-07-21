#include "SessionManager.h"
#include "C_ClientInfo.h"
#include "C_LoginState.h"

SessionManager* SessionManager::instance;
SessionManager* SessionManager::GetInstance()
{
	// �ν��Ͻ��� ���ٸ� �ν��Ͻ��� �����ϰ� �����Ѵ�.
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

// ���� ����, �ּ������� Ŭ���̾�Ʈ�� �����Ҵ��ϴ� �Լ�
bool SessionManager::AddSession(SOCKET _sock, SOCKADDR_IN _addr)
{
	C_LoginState* state = new C_LoginState();
	C_ClientInfo* ptr = new C_ClientInfo(nullptr, state, _sock, _addr);
	return clientList->Insert(ptr);
}

C_ClientInfo* SessionManager::FindWithSocket(SOCKET _sock)
{
	C_ClientInfo* info = nullptr;   // ��ȸ�� �� Ŭ���� ������ �ϳ��� ��Ƽ� Ȱ���� ����

	   // ���� Ŭ���̾�Ʈ ����Ʈ�� �˻����� �ƴ϶��, ����Ʈ�� ��ȸ�ϸ� �� set�� ������ �Ѵ�.
	if (clientList->SearchCheck() == false)
	{
		info = nullptr;
		clientList->SearchStart();
		while (1)
		{
			info = clientList->SearchData();   // Ŭ���̾�Ʈ�� �ϳ��� �޾ƿ´�.

			// ������ �� Ȯ���ߴٸ� �����Ѵ�.
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