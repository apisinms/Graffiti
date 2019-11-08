#include "stdafx.h"

void C_IOCP::IOCP_Init()
{
	// 입출력 완료 포트 생성 
	hcp = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, 0);
	if (hcp == NULL)
	{
		LogManager::GetInstance()->ErrQuitMsgBox("CreateCompletionPort()");
	}

	// CPU 갯수 확인 
	SYSTEM_INFO si;
	GetSystemInfo(&si);

	vThread.reserve((int)si.dwNumberOfProcessors * 2);	// 벡터에 미리 CPU갯수만큼 할당

	// 쓰레드를 생성한다.
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
	// 모든 쓰레드가 종료되도록 쓰레드종료 완료패킷을 넣어준다.
	for (size_t i = 0; i < vThread.size(); i++)
		PostQueuedCompletionStatus(hcp, THREAD_END, NULL, NULL);

	// 벡터내의 모든 쓰레드가 종료되기를 기다린다.(신형 STL은 이 문법이 가능함)
	WaitForMultipleObjects((DWORD)vThread.size(), vThread.data(), TRUE, INFINITE);

	// 모든 쓰레드핸들을 반납한다.
	vector<HANDLE>::iterator it;
	for (it = vThread.begin(); it != vThread.end(); ++it)
		CloseHandle((HANDLE)*it);

	vector<HANDLE>().swap(vThread);	// 빈 벡터와 swap하여 vThread 벡터를 완전히 Clear시켜준다.
}

bool C_IOCP::IOCP_Register(HANDLE _handle, ULONG_PTR _key)
{
	if (CreateIoCompletionPort(_handle, this->hcp, _key, 0) == NULL)
		return false;

	return true;
}

// 작업자 스레드 함수
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

		// 쓰레드가 종료되라고 패킷이 넘어온거라면 쓰레드를 종료함
		if (cbTransferred == THREAD_END)
			return 0;
		
		void* ptr = overlapped->ptr;	// 넘겨져온 overlapped->ptr을 캐스팅하여 사용한다.

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