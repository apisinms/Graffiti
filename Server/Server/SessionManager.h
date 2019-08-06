#pragma once
#include "C_List.h"
#include "C_ClientInfo.h"

class SessionManager
{
private:
	SessionManager() {}
	~SessionManager() {}
	static SessionManager* instance;
	C_List<C_ClientInfo*>* clientList;

public:
	void Init();
	void End();
	static SessionManager* GetInstance();
	static void Destroy();

public:
	bool AddSession(SOCKET _sock, SOCKADDR_IN _addr);
	C_ClientInfo* FindWithSocket(SOCKET _sock);

	bool Insert(C_ClientInfo* _info);   // ����Ʈ�� �����ϴ� �Լ�
	bool Remove(C_ClientInfo* _info);   // ����Ʈ���� ��� + ������ �����ϴ� �Լ�
	int GetCount();         // ������ �������ִ� �Լ�
	bool SearchCheck();      // �˻������� �˻��ϴ� �Լ�
	void SearchStart();      // �˻��� �����ϴ� �Լ�(Ǫ��)
	C_ClientInfo* SearchData();         // �˻��ؼ� �����͸� �����ϴ� �Լ�
	void SearchEnd();      // �˻��� �����ϴ� �Լ�(��)
};