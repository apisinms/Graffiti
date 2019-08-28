#pragma once
#include "C_Socket.h"

class C_IOCP : public C_SyncCS<C_IOCP>
{
protected:
	HANDLE hcp;								// ����� �Ϸ� ��Ʈ
	vector<HANDLE> vThread;					// ���� �޸� �� �Ҵ����־���� (Init�޼���)
	C_Socket* listenSock;					// ������ ���� ������
	static DWORD WINAPI WorkerThread(LPVOID _this);

public:
	void IOCP_Init();
	void IOCP_End();
	bool IOCP_Register(HANDLE _handle, ULONG_PTR _key);
	virtual void IOCP_Accept(void* _ptr) = 0;
	virtual void IOCP_Read(void* _ptr, int _len) = 0;
	virtual void IOCP_Write(void* _ptr, int _len) = 0;
	virtual void IOCP_Disconnected(void* _ptr) = 0;
};