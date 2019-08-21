#pragma once
#pragma comment(lib, "ws2_32")
#include <WinSock2.h>
#include <Windows.h>

class C_ComfortableCS
{
private:
	CRITICAL_SECTION cs;

public:
	C_ComfortableCS()
	{
		InitializeCriticalSection(&cs);
	}
	~C_ComfortableCS()
	{
		DeleteCriticalSection(&cs);
	}
	void Enter()
	{
		EnterCriticalSection(&cs);
	}
	void Leave()
	{
		LeaveCriticalSection(&cs);
	}
};