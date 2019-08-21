#pragma once
#include "C_IOCP.h"
class C_State;

class MainManager : public C_IOCP
{
private:
	static MainManager* instance;			// �̱����� ���� ���� ������
	MainManager() {}
	~MainManager() {};

public:
	static MainManager* GetInstance();
	static void Destroy();
	void Init();
	void Run();
	void End();
	static BOOL WINAPI CtrlHandler(DWORD _fdwCtrlType);	// �ڵ鷯 �ݹ��Լ�, ���Ḧ �����Ѵ�.

public:
	void IOCP_Accept(void* _ptr) override;
	void IOCP_Read(void* _ptr, int _len)override;
	void IOCP_Write(void* _ptr, int _len) override;
	void IOCP_Disconnected(void* _ptr)override;

//public:
//	void FDReset() override;
//	C_ClientInfo* FDAccept(C_Socket* _listenSock) override;
//	void FDSet(C_Socket* _sock, int _type) override;
//	int FDRead(C_ClientInfo* _client) override;
//	int FDWrite(C_ClientInfo* _client) override;
};