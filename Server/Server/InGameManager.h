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
#ifdef DEBUG
	static const int WEAPON_SELTIME = 5 + 1;	// ���� ���� �ð�(�� ����)
#else
	static const int WEAPON_SELTIME = 30 + 1;	// ���� ���� �ð�(�� ����)
#endif
	

#ifdef __64BIT__
	enum PROTOCOL_INGAME : __int64
	{
		TIMER_PROTOCOL        = ((__int64)0x1 << 58),	// 1�ʸ��� ������ Ÿ�̸�
		WEAPON_PROTOCOL       = ((__int64)0x1 << 57),	// ������:���⼱�ù޾ƿ�, Ŭ����:���⼱�ú�����
		START_PROTOCOL        = ((__int64)0x1 << 56),	// ���� ���� ��������
		MOVE_PROTOCOL		  = ((__int64)0x1 << 55),	// �̵� ��������
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

	struct Position
	{
		int playerNum;
		float posX;
		float posZ;

	};

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
	void PackPacket(char* _setptr, const int &_sec, int& _size);
	void PackPacket(char* _setptr, TCHAR* _str1, int& _size);				// ���ڿ� 1���� Pack�ϴ� �Լ�
	void PackPacket(char* _setptr, Position& _struct, int& _size);
	void UnPackPacket(char* _getBuf, Position& _struct);
	void UnPackPacket(char* _getBuf, Weapon* &_weapon);

	void GetProtocol(PROTOCOL_INGAME& _protocol);								// ���������� ����
	PROTOCOL_INGAME SetProtocol(STATE_PROTOCOL _state, PROTOCOL_INGAME _protocol, RESULT_INGAME _result);	// �������� + result(�ִٸ�)�� ������

	PROTOCOL_INGAME GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf);	// buf�� Protocol�� ���ÿ� ��� �Լ�
	bool WeaponSelectProcess(C_ClientInfo* _ptr, char* _buf);
	bool MoveProcess(C_ClientInfo* _ptr, char* _buf);

public:
	bool CanISelectWeapon(C_ClientInfo* _ptr);	// ���� ����
	bool CanIIMove(C_ClientInfo* _ptr);			// �̵�

	static unsigned long __stdcall TimerThread(void* _arg);	// ������ ���� �ð��� ���� Ÿ�̸� ������
};