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
};