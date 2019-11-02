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
	// 인스턴스가 없다면 인스턴스를 생성하고 리턴한다.
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

// 받은 소켓, 주소정보로 클라이언트를 동적할당하는 함수
bool SessionManager::AddSession(SOCKET _sock, SOCKADDR_IN _addr)
{
	C_ClientInfo* ptr = new C_ClientInfo(nullptr, _sock, _addr);
	
	int beforeCnt = (int)clientList.size();
	clientList.emplace_back(ptr);

	// 리스트에 넣기 전 크기가 더 커졌으면 넣은게 맞으니까 true리턴, 아니면 false리턴되겠지
	return (beforeCnt < (int)clientList.size());
}

C_ClientInfo* SessionManager::FindWithSocket(SOCKET _sock)
{
	C_ClientInfo* client = nullptr;	// 순회할 때 클라의 정보를 하나씩 담아서 활용할 변수
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
		std::cout << "SessionManager::Remove()에서 예외 발생" << endl;
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

	// 반복자 위치가 end가 아니면 존재하는 것
	if (iter != clientList.end())
	{
		return true;
	}

	else
	{
		return false;
	}
}
