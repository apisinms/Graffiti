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
	void Remove(C_ClientInfo* _info);   // ����Ʈ���� ��� + ������ �����ϴ� �Լ�
	int GetSize();						// ������ �������ִ� �Լ�
	bool IsClientExist(C_ClientInfo* _client);	// �Ű������� �Ѿ�� Ŭ�� �����ϴ���
};