#include "stdafx.h"
#include "C_ClientInfo.h"
#include "C_LoginState.h"

SessionManager* SessionManager::instance;

void SessionManager::Init()
{
}

void SessionManager::End()
{

}

SessionManager* SessionManager::GetInstance()
{
	// �ν��Ͻ��� ���ٸ� �ν��Ͻ��� �����ϰ� �����Ѵ�.
	if (instance == nullptr)
	{
		instance = new SessionManager();
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
	C_ClientInfo* ptr = new C_ClientInfo(nullptr, _sock, _addr);
	
	int beforeCnt = (int)clientList.size();
	clientList.emplace_back(ptr);

	// ����Ʈ�� �ֱ� �� ũ�Ⱑ �� Ŀ������ ������ �����ϱ� true����, �ƴϸ� false���ϵǰ���
	return (beforeCnt < (int)clientList.size());
}

C_ClientInfo* SessionManager::FindWithSocket(SOCKET _sock)
{
	C_ClientInfo* client = nullptr;	// ��ȸ�� �� Ŭ���� ������ �ϳ��� ��Ƽ� Ȱ���� ����
	for (auto iter = clientList.begin(); iter != clientList.end(); ++iter)
	{
		client = *iter;

		if (client->GetSocket() == _sock)
		{
			return client;
		}
	}

	return nullptr;
}

void SessionManager::Remove(C_ClientInfo* _info)
{
	clientList.remove(_info);
	delete _info;
	_info = nullptr;



	/*try
	{
		clientList.remove(_info);
		delete _info;
		_info = nullptr;
	}
	catch (const std::exception& _exception)
	{
		std::cout << "SessionManager::Remove()���� ���� �߻�" << endl;
		std::cout << _exception.what() << endl;
	}*/

}

int SessionManager::GetSize()
{
	return (int)clientList.size();
}

bool SessionManager::IsClientExist(C_ClientInfo* _client)
{
	auto iter = find(clientList.begin(), clientList.end(), _client);

	// �ݺ��� ��ġ�� end�� �ƴϸ� �����ϴ� ��
	if (iter != clientList.end())
	{
		return true;
	}

	else
	{
		return false;
	}
}
