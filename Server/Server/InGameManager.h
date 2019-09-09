#pragma once
#include "C_Global.h"

class C_ClientInfo;

class InGameManager
{
#ifdef DEBUG
	static const int WEAPON_SELTIME = 1 + 1;	// ���� ���� �ð�(�� ����)
#else
	static const int WEAPON_SELTIME = 30 + 1;	// ���� ���� �ð�(�� ����)
#endif

	// 53~34
	enum PROTOCOL_INGAME : __int64
	{
		TIMER_PROTOCOL        = ((__int64)0x1 << 53),	// 1�ʸ��� ������ Ÿ�̸�
		WEAPON_PROTOCOL       = ((__int64)0x1 << 52),	// ������:���⼱�ù޾ƿ�, Ŭ����:���⼱�ú�����
		START_PROTOCOL        = ((__int64)0x1 << 51),	// ���� ���� ��������
		MOVE_PROTOCOL		  = ((__int64)0x1 << 50),	// �̵� ��������
		DISCONNECT_PROTOCOL   = ((__int64)0x1 << 49),	// ���� ���� ��������
	};

   // 33~24
	enum RESULT_INGAME : __int64
	{
		// INGAME_PROTOCOL ����
		INGAME_SUCCESS = ((__int64)0x1 << 33),
		INGAME_FAIL    = ((__int64)0x1 << 32),

		// MOVE_PROTOCOL ����
		ENTER_SECTOR        = ((__int64)0x1 << 31),		// ���� ����
		EXIT_SECTOR         = ((__int64)0x1 << 30),		// ���� ����
		UPDATE_PLAYER       = ((__int64)0x1 << 29),		// �÷��̾� ��� �ֽ�ȭ
		FORCE_MOVE          = ((__int64)0x1 << 28),		// ���� �̵�
		GET_OTHERPLAYER_POS = ((__int64)0x1 << 27),		// �ٸ� �÷��̾� ������ ���

		NODATA = ((__int64)0x1 << 24)
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
	void PackPacket(char* _setptr, PositionPacket& _struct, int& _size);
	void UnPackPacket(char* _getBuf, int& _num);
	void UnPackPacket(char* _getBuf, PositionPacket& _struct);
	void UnPackPacket(char* _getBuf, Weapon* &_weapon);

	void GetProtocol(PROTOCOL_INGAME& _protocol);		// ���������� ����
	void GetResult(char* _buf, RESULT_INGAME& _result);				// result�� ����
	PROTOCOL_INGAME SetProtocol(STATE_PROTOCOL _state, PROTOCOL_INGAME _protocol, RESULT_INGAME _result);	// �������� + result(�ִٸ�)�� ������

	PROTOCOL_INGAME GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf);	// buf�� Protocol�� ���ÿ� ��� �Լ�
	bool WeaponSelectProcess(C_ClientInfo* _ptr, char* _buf);
	bool InitProcess(C_ClientInfo* _ptr, char* _buf);
	bool MoveProcess(C_ClientInfo* _ptr, char* _buf);
	bool GetPosProcess(C_ClientInfo* _ptr, char* _buf);	// ��ġ�� ����ִ� �Լ�

public:
	bool CanISelectWeapon(C_ClientInfo* _ptr);	// ���� ����
	bool CanIStart(C_ClientInfo* _ptr);			// ���� �� �ʱ�ȭ
	bool CanIMove(C_ClientInfo* _ptr);			// �̵�
	bool LeaveProcess(C_ClientInfo* _ptr, int _playerIndex);		// ���� ���μ���

	void ListSendPacket(list<C_ClientInfo*> _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize);
	
	static unsigned long __stdcall TimerThread(void* _arg);	// ������ ���� �ð��� ���� Ÿ�̸� ������
};