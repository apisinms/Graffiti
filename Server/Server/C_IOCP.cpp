#include "stdafx.h"

void C_IOCP::IOCP_Init()
{
	// ����� �Ϸ� ��Ʈ ���� 
	hcp = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, 0);
	if (hcp == NULL)
	{
		LogManager::GetInstance()->ErrQuitMsgBox("CreateCompletionPort()");
	}

	// CPU ���� Ȯ�� 
	SYSTEM_INFO si;
	GetSystemInfo(&si);

	vThread.reserve((int)si.dwNumberOfProcessors * 2);	// ���Ϳ� �̸� CPU������ŭ �Ҵ�

	// �����带 �����Ѵ�.
	HANDLE hThread;
	for (int i = 0; i < (int)si.dwNumberOfProcessors * 2; i++)
	{
		//hThread = CreateThread(NULL, 0, WorkerThread, this, 0, NULL);
		hThread = (HANDLE)_beginthreadex(NULL, 0, (_beginthreadex_proc_type)WorkerThread, this, 0, NULL);
		if (hThread == NULL)
		{
			LogManager::GetInstance()->ErrQuitMsgBox("createThread()");
		}
		vThread.push_back(hThread);
	}

}

void C_IOCP::IOCP_End()
{
	// ��� �����尡 ����ǵ��� ���������� �Ϸ���Ŷ�� �־��ش�.
	for (size_t i = 0; i < vThread.size(); i++)
		PostQueuedCompletionStatus(hcp, THREAD_END, NULL, NULL);

	// ���ͳ��� ��� �����尡 ����Ǳ⸦ ��ٸ���.(���� STL�� �� ������ ������)
	WaitForMultipleObjects((DWORD)vThread.size(), vThread.data(), TRUE, INFINITE);

	// ��� �������ڵ��� �ݳ��Ѵ�.
	vector<HANDLE>::iterator it;
	for (it = vThread.begin(); it != vThread.end(); ++it)
		CloseHandle((HANDLE)*it);

	vector<HANDLE>().swap(vThread);	// �� ���Ϳ� swap�Ͽ� vThread ���͸� ������ Clear�����ش�.
}

bool C_IOCP::IOCP_Register(HANDLE _handle, ULONG_PTR _key)
{
	if (CreateIoCompletionPort(_handle, this->hcp, _key, 0) == NULL)
		return false;

	return true;
}

// �۾��� ������ �Լ�
DWORD CALLBACK C_IOCP::WorkerThread(LPVOID _this)
{
	int retval;
	C_IOCP* iocp = (C_IOCP*)_this;

	while (1)
	{
		int cbTransferred;
		WSAOVERLAPPED_EX* overlapped;
		DWORD_PTR key = 0;

		retval = GetQueuedCompletionStatus(iocp->hcp, (DWORD*)&cbTransferred,
			&key, (LPOVERLAPPED *)&overlapped, INFINITE);

		// �����尡 ����Ƕ�� ��Ŷ�� �Ѿ�°Ŷ�� �����带 ������
		if (cbTransferred == THREAD_END)
			return 0;
		
		void* ptr = overlapped->ptr;	// �Ѱ����� overlapped->ptr�� ĳ�����Ͽ� ����Ѵ�.

		if (retval == 0 || cbTransferred == 0)
		{
			LogManager::GetInstance()->WSAOverlappedResultPrintf("GetQueuedCompletionStatus", overlapped);
			iocp->IOCP_Disconnected(ptr);
			ptr = nullptr;
		}

		if (ptr != nullptr)
		{
			switch (overlapped->type)
			{
			case IO_TYPE::IO_RECV:
				iocp->IOCP_Read(ptr, cbTransferred);
				break;
			case IO_TYPE::IO_SEND:
				iocp->IOCP_Write(ptr, cbTransferred);
				break;
			case IO_TYPE::IO_ACCEPT:
				iocp->IOCP_Accept(ptr);
				break;
			}
		}
	}
}