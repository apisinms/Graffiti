#pragma once

class UtilityManager
{
private:
	UtilityManager() {};
	~UtilityManager() {};
	static UtilityManager* instance;

public:
	static UtilityManager* GetInstance();
	static void Destroy();

	void UnicodeToUTF8(wchar_t* _strUnicode, char* _strUTF8);
	void UTF8ToUnicode(char* _strUTF8, wchar_t* _strUnicode);

	//{
	//	//int MultiByteToUnicode(char* _strMultibyte, wchar_t* _strUnicode);
	//	//void UnicodeToMultiByte(wchar_t* _strUnicode, char* _strMultibyte);
	//	//void MultiByteToUTF8(char* _strMultibyte, char* _strUTF8);
	//	//void UTF8ToMultiByte(char* _strUTF8, char* _strMultibyte);
	//}
};