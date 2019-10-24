#pragma once

class UtilityManager
{
private:
	UtilityManager() {};
	~UtilityManager() {};
	static UtilityManager* instance;

public:
	void Init();
	void End();
	static UtilityManager* GetInstance();
	static void Destroy();

	void UnicodeToUTF8(wchar_t* _strUnicode, char* _strUTF8);
	void UTF8ToUnicode(char* _strUTF8, wchar_t* _strUnicode);

	float GetDotDistanceNoSqrt(float _posX1, float _posZ1, float _posX2, float _posZ2);
	float GetDotDistanceWithSqrt(float _posX1, float _posZ1, float _posX2, float _posZ2);

	int IsWritableMemory(void* _memoryAddr);
	int IsReadableMemory(void* _memoryAddr);

	//{
	//	//int MultiByteToUnicode(char* _strMultibyte, wchar_t* _strUnicode);
	//	//void UnicodeToMultiByte(wchar_t* _strUnicode, char* _strMultibyte);
	//	//void MultiByteToUTF8(char* _strMultibyte, char* _strUTF8);
	//	//void UTF8ToMultiByte(char* _strUTF8, char* _strMultibyte);
	//}
};