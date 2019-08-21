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

	bool Insert(C_ClientInfo* _info);   // 리스트에 삽입하는 함수
	bool Remove(C_ClientInfo* _info);   // 리스트에서 노드 + 데이터 삭제하는 함수
	int GetCount();         // 갯수를 리턴해주는 함수
	bool SearchCheck();      // 검색중인지 검사하는 함수
	void SearchStart();      // 검색을 시작하는 함수(푸쉬)
	C_ClientInfo* SearchData();         // 검색해서 데이터를 리턴하는 함수
	void SearchEnd();      // 검색을 종료하는 함수(팝)
};