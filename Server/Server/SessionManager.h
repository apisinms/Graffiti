#pragma once
#include "C_ClientInfo.h"

class SessionManager
{
private:
	SessionManager() {}
	~SessionManager() {}
	static SessionManager* instance;
	list<C_ClientInfo*> clientList;

public:
	void Init();
	void End();
	static SessionManager* GetInstance();
	static void Destroy();

public:
	bool AddSession(SOCKET _sock, SOCKADDR_IN _addr);
	C_ClientInfo* FindWithSocket(SOCKET _sock);
	void Remove(C_ClientInfo* _info);   // 리스트에서 노드 + 데이터 삭제하는 함수
	int GetSize();						// 갯수를 리턴해주는 함수
	bool IsClientExist(C_ClientInfo* _client);	// 매개변수로 넘어온 클라가 존재하는지
};