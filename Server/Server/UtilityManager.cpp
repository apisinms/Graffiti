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
