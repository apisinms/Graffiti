#pragma once

const int C1 = 52845;
const int C2 = 22719;
const int KEY = 78695;

class C_Encrypt
{
private:
	static C_Encrypt* instance;
	C_Encrypt() {}
	~C_Encrypt() {};
public:
	static C_Encrypt* GetInstance();
	static void Destroy();

public:
	bool Encrypt(char* _src, char* _dest, int _size);
	bool Decrypt(char* _src, char* _dest, int _size);
};