#include "stdafx.h"
#include "C_Global.h"

UtilityManager* UtilityManager::instance;         // 초기화

void UtilityManager::Init()
{
}

void UtilityManager::End()
{
}

UtilityManager* UtilityManager::GetInstance()
{
	// 인스턴스가 없다면 인스턴스를 생성하고 리턴한다.
	if (instance == nullptr)
	{
		instance = new UtilityManager();
	}

	return instance;
}

void UtilityManager::Destroy()
{
	delete instance;
}

// 유니코드 -> UTF8
void UtilityManager::UnicodeToUTF8(wchar_t* _strUnicode, char* _strUTF8)
{
	int nLen = WideCharToMultiByte(CP_UTF8, 0, _strUnicode, lstrlenW(_strUnicode), NULL, 0, NULL, NULL);
	WideCharToMultiByte(CP_UTF8, 0, _strUnicode, lstrlenW(_strUnicode), _strUTF8, nLen, NULL, NULL);


	//_strUTF8 = CW2A(_strUnicode, CP_UTF8);
	//int nLen = WideCharToMultiByte(CP_UTF8, 0, _strUnicode, lstrlenW(_strUnicode), NULL, 0, NULL, NULL);
	//WideCharToMultiByte(CP_UTF8, 0, _strUnicode, lstrlenW(_strUnicode), _strUTF8, nLen, NULL, NULL);
}

// UTF8 -> 유니코드 
void UtilityManager::UTF8ToUnicode(char* _strUTF8, wchar_t* _strUnicode)
{
	//_strUnicode = CA2W(_strUTF8, CP_UTF8);
	int nLen = MultiByteToWideChar(CP_UTF8, 0, _strUTF8, (int)strlen(_strUTF8), NULL, NULL);
	MultiByteToWideChar(CP_UTF8, 0, _strUTF8, (int)strlen(_strUTF8), _strUnicode, nLen);
}

float UtilityManager::GetDotDistanceNoSqrt(float _posX1, float _posZ1, float _posX2, float _posZ2)
{
	return ((_posX2 - _posX1) * (_posX2 - _posX1)) + ((_posZ2 - _posZ1) * (_posZ2 - _posZ1));
}

float UtilityManager::GetDotDistanceWithSqrt(float _posX1, float _posZ1, float _posX2, float _posZ2)
{
	return sqrt(((_posX2 - _posX1) * (_posX2 - _posX1)) + ((_posZ2 - _posZ1) * (_posZ2 - _posZ1)));
}

int UtilityManager::IsWritableMemory(void* _memoryAddr)
{
	MEMORY_BASIC_INFORMATION64 memInfo = { 0, };
	size_t result = 0;

	result = VirtualQuery(_memoryAddr, (MEMORY_BASIC_INFORMATION*)&memInfo, sizeof(memInfo));

	if (result == 0)	// 커널 영역인 경우 VirtualQuery 자체가 Fail함.
	{	
		return -1;
	}

	else if (memInfo.Protect & (PAGE_EXECUTE_READWRITE | PAGE_READWRITE))
	{
		return ERROR_SUCCESS;
	}

	else
	{
		return memInfo.Protect;
	}
}
/*int UtilityManager::IsReadableMemory(void* _memoryAddr)
{
	//MEMORY_BASIC_INFORMATION64 memInfo = { 0, };
	MEMORY_BASIC_INFORMATION memInfo = { 0, };
	size_t  nResult = 0;

	//nResult = VirtualQuery(_memoryAddr, (MEMORY_BASIC_INFORMATION*)&memInfo, sizeof(memInfo));
	nResult = VirtualQuery(_memoryAddr, &memInfo, sizeof(memInfo));

	if (nResult == 0) // 커널 영역인 경우 VirtualQuery 자체가 Fail함.  
	{
		return -1;
	}

	else if (memInfo.State & MEM_COMMIT)
	{
		return  ERROR_SUCCESS;
	}

	else
	{
		return  memInfo.State;
	}
}*/

INT UtilityManager::IsReadableMemory(LPVOID pMemoryAddr)
{
	MEMORY_BASIC_INFORMATION MemInfo = { 0, };
	SIZE_T nResult = 0;

	nResult = VirtualQuery(pMemoryAddr, &MemInfo, sizeof(MemInfo));

	if (nResult == 0) // 커널 영역인 경우 VirtualQuery 자체가 Fail함.  
	{
		return -1;
	}
	else if (MemInfo.State & MEM_COMMIT)
	{
		return  ERROR_SUCCESS;
	}
	else
	{
		return  MemInfo.State;
	}
}

//{
//	// 멀티바이트 -> 유니코드
//	int UtilityManager::MultiByteToUnicode(char* _strMultibyte, wchar_t* _strUnicode)
//	{
//		int nLen = MultiByteToWideChar(CP_ACP, MB_PRECOMPOSED, _strMultibyte, strlen(_strMultibyte), NULL, NULL);
//		MultiByteToWideChar(CP_ACP, MB_PRECOMPOSED, _strMultibyte, strlen(_strMultibyte), _strUnicode, nLen);
//
//		return nLen;	// 쓴 바이트 수를 리턴함
//	}
//
//	// 유니코드 -> 멀티바이트
//	void UtilityManager::UnicodeToMultiByte(wchar_t* _strUnicode, char* _strMultibyte)
//	{
//		int len = WideCharToMultiByte(CP_ACP, 0, _strUnicode, -1, NULL, 0, NULL, NULL);
//		WideCharToMultiByte(CP_ACP, 0, _strUnicode, -1, _strMultibyte, len, NULL, NULL);
//	}
//
//	// 멀티바이트 -> UTF8 (멀티바이트 -> 유니코드(UTF-16) -> UTF-8 )
//	void UtilityManager::MultiByteToUTF8(char* _strMultibyte, char* _strUTF8)
//	{
//		wchar_t strUnicode[MSGSIZE];
//		int nLen = MultiByteToWideChar(CP_ACP, 0, _strMultibyte, strlen(_strMultibyte), NULL, NULL);
//		MultiByteToWideChar(CP_ACP, 0, _strMultibyte, strlen(_strMultibyte), strUnicode, nLen);
//
//		nLen = WideCharToMultiByte(CP_UTF8, 0, strUnicode, lstrlenW(strUnicode), NULL, 0, NULL, NULL);
//		WideCharToMultiByte(CP_UTF8, 0, strUnicode, lstrlenW(strUnicode), _strUTF8, nLen, NULL, NULL);
//	}
//
//	// UTF8 -> 멀티바이트 (UTF-8 -> 유니코드(UTF-16) -> 멀티바이트)
//	void UtilityManager::UTF8ToMultiByte(char* _strUTF8, char* _strMultibyte)
//	{
//		wchar_t strUnicode[MSGSIZE];
//		int nLen = MultiByteToWideChar(CP_UTF8, 0, _strUTF8, strlen(_strUTF8), NULL, NULL);
//		MultiByteToWideChar(CP_UTF8, 0, _strUTF8, strlen(_strUTF8), strUnicode, nLen);
//
//		int len = WideCharToMultiByte(CP_ACP, 0, strUnicode, -1, NULL, 0, NULL, NULL);
//		WideCharToMultiByte(CP_ACP, 0, strUnicode, -1, _strMultibyte, len, NULL, NULL);
//	}
//}