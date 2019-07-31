#pragma once
#include "C_List.h"
#include "C_Global.h"
#include <tchar.h>

#define MAX_ROOM_SIZE	200
#define ROOM_TITLE_LEN	255
#define ROOM_NUM_LEN	20

class C_ClientInfo;

class InGameManager
{
	static const int itemSelTime = 10;	// ������ ���� �ð�(�� ����)
	

#ifdef __64BIT__
	enum PROTOCOL_INGAME : __int64
	{
		//ITEMSELECT_PROTOCOL = ((__int64)0x1 << 58),
		WEAPON_PROTOCOL     = ((__int64)0x1 << 58),
	};

	enum RESULT_INGAME : __int64
	{
		INGAME_SUCCESS = ((__int64)0x1 << 53),
		INGAME_FAIL    = ((__int64)0x1 << 52),

		NODATA = ((__int64)0x1 << 49)
	};
#endif

#ifdef __32BIT__
	enum PROTOCOL_INGAME : int
	{
		ITEMSELECT_PROTOCOL = ((int)0x1 << 26),
	};

	enum RESULT_INGAME : int
	{
		INGAME_SUCCESS = ((int)0x1 << 21),
		INGAME_FAIL    = ((int)0x1 << 20),

		NODATA = ((int)0x1 << 17)
	};
#endif

	struct Weapon
	{
		char mainW;
		char subW;
	}weapon;

private:
	InGameManager() {}
	~InGameManager() {}
	static InGameManager* instance;

public:
	void Init();
	void End();
	static InGameManager* GetInstance();
	static void Destroy();

private:
	void PackPacket(char* _setptr, TCHAR* _str1, int& _size);	// ���ڿ� 1���� Pack�ϴ� �Լ�
	void UnPackPacket(char* _getBuf, Weapon& _struct);
	void UnPackPacket(char* _getBuf, int& _num1, int& _num2);				// ���ڿ� 1���� UnPack�ϴ� �Լ�

	void GetProtocol(PROTOCOL_INGAME& _protocol);								// ���������� ����
	PROTOCOL_INGAME SetProtocol(STATE_PROTOCOL _state, PROTOCOL_INGAME _protocol, RESULT_INGAME _result);	// �������� + result(�ִٸ�)�� ������

	PROTOCOL_INGAME GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf);	// buf�� Protocol�� ���ÿ� ��� �Լ�
	bool ItemSelctProcess(C_ClientInfo* _ptr, char* _buf);
public:
	bool CanIItemSelect(C_ClientInfo* _ptr);	// ������ ����

	static unsigned long __stdcall TimerThread(void* _arg);	// ������ ���� �ð��� ���� Ÿ�̸� ������
};