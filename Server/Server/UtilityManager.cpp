#include "UtilityManager.h"
#include "C_Global.h"
#include <Windows.h>
#include <atlstr.h>

UtilityManager* UtilityManager::instance;         // �ʱ�ȭ

UtilityManager* UtilityManager::GetInstance()
{
	// �ν��Ͻ��� ���ٸ� �ν��Ͻ��� �����ϰ� �����Ѵ�.
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

// �����ڵ� -> UTF8
void UtilityManager::UnicodeToUTF8(wchar_t* _strUnicode, char* _strUTF8)
{
	int nLen = WideCharToMultiByte(CP_UTF8, 0, _strUnicode, lstrlenW(_strUnicode), NULL, 0, NULL, NULL);
	WideCharToMultiByte(CP_UTF8, 0, _strUnicode, lstrlenW(_strUnicode), _strUTF8, nLen, NULL, NULL);


	//_strUTF8 = CW2A(_strUnicode, CP_UTF8);
	//int nLen = WideCharToMultiByte(CP_UTF8, 0, _strUnicode, lstrlenW(_strUnicode), NULL, 0, NULL, NULL);
	//WideCharToMultiByte(CP_UTF8, 0, _strUnicode, lstrlenW(_strUnicode), _strUTF8, nLen, NULL, NULL);
}

// UTF8 -> �����ڵ� 
void UtilityManager::UTF8ToUnicode(char* _strUTF8, wchar_t* _strUnicode)
{
	//_strUnicode = CA2W(_strUTF8, CP_UTF8);
	int nLen = MultiByteToWideChar(CP_UTF8, 0, _strUTF8, strlen(_strUTF8), NULL, NULL);
	MultiByteToWideChar(CP_UTF8, 0, _strUTF8, strlen(_strUTF8), _strUnicode, nLen);
}

//{
//	// ��Ƽ����Ʈ -> �����ڵ�
//	int UtilityManager::MultiByteToUnicode(char* _strMultibyte, wchar_t* _strUnicode)
//	{
//		int nLen = MultiByteToWideChar(CP_ACP, MB_PRECOMPOSED, _strMultibyte, strlen(_strMultibyte), NULL, NULL);
//		MultiByteToWideChar(CP_ACP, MB_PRECOMPOSED, _strMultibyte, strlen(_strMultibyte), _strUnicode, nLen);
//
//		return nLen;	// �� ����Ʈ ���� ������
//	}
//
//	// �����ڵ� -> ��Ƽ����Ʈ
//	void UtilityManager::UnicodeToMultiByte(wchar_t* _strUnicode, char* _strMultibyte)
//	{
//		int len = WideCharToMultiByte(CP_ACP, 0, _strUnicode, -1, NULL, 0, NULL, NULL);
//		WideCharToMultiByte(CP_ACP, 0, _strUnicode, -1, _strMultibyte, len, NULL, NULL);
//	}
//
//	// ��Ƽ����Ʈ -> UTF8 (��Ƽ����Ʈ -> �����ڵ�(UTF-16) -> UTF-8 )
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
//	// UTF8 -> ��Ƽ����Ʈ (UTF-8 -> �����ڵ�(UTF-16) -> ��Ƽ����Ʈ)
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