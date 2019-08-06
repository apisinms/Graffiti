#include "C_Encrypt.h"

C_Encrypt* C_Encrypt::instance;

C_Encrypt* C_Encrypt::GetInstance()
{
	if (instance == nullptr)
		instance = new C_Encrypt();
	
	return instance;
}

void C_Encrypt::Destroy()
{
	delete instance;
}

bool C_Encrypt::Encrypt(char* _src, char* _dest, int _size)
{
	int key = KEY;

	if (!_src || !_dest || _size <= 0)
		return false;

	for (int i = 0; i < _size; i++)
	{
		_dest[i] = _src[i] ^ key >> 8;
		key = (_dest[i] + key) * C1 + C2;
	}

	return true;
}

bool C_Encrypt::Decrypt(char* _src, char* _dest, int _size)
{
	char prevBlock;
	int key = KEY;


	if (!_src || !_dest || _size <= 0)
		return false;

	for (int i = 0; i < _size; i++)
	{
		prevBlock = _src[i];
		_dest[i] = _src[i] ^ key >> 8;
		key = (prevBlock + key) * C1 + C2;
	}

	return true;
}