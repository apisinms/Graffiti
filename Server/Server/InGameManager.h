#pragma once
#include "C_Global.h"

class C_ClientInfo;

class InGameManager
{
#ifdef DEBUG
	static const int WEAPON_SELTIME = 1 + 1;	// ���� ���� �ð�(�� ����)
	int numOfPacketSent             = 0;		// ��Ŷ ���� Ƚ��
#else
	static const int WEAPON_SELTIME = 30 + 1;	// ���� ���� �ð�(�� ����)
#endif

	//list<WeaponInfo*> weaponInfoList;	// ������ ���� ��������Ʈ
	//list<GameInfo*> gameInfoList;		// ������ ���� ��������Ʈ
	vector<WeaponInfo*> weaponInfo;		// ������ ���� ���� ����
	vector<GameInfo*> gameInfo;			// ������ ���� ���� ����

	// 53~34
	enum PROTOCOL_INGAME : __int64
	{
		TIMER_PROTOCOL        = ((__int64)0x1 << 53),	// 1�ʸ��� ������ Ÿ�̸�
		WEAPON_PROTOCOL       = ((__int64)0x1 << 52),	// ������:���⼱�ù޾ƿ�, Ŭ����:���⼱�ú�����
		START_PROTOCOL        = ((__int64)0x1 << 51),	// ���� ���� ��������
		LOADING_PROTOCOL      = ((__int64)0x1 << 50),	// �ε� ���� ��������
		MOVE_PROTOCOL		  = ((__int64)0x1 << 49),	// �̵� ��������
		FOCUS_PROTOCOL        = ((__int64)0x1 << 48),	// ��Ŀ�� ��������

		DISCONNECT_PROTOCOL   = ((__int64)0x1 << 34),	// ���� ���� ��������
	};

   // 33~24
	enum RESULT_INGAME : __int64
	{
		// INGAME_PROTOCOL ����
		INGAME_SUCCESS = ((__int64)0x1 << 33),
		INGAME_FAIL    = ((__int64)0x1 << 32),

		// WEAPON_PROTOCOL ����
		NOTIFY_WEAPON = ((__int64)0x1 << 31),	// ���⸦ �˷���

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
	void PackPacket(char* _setptr, const int _num, int& _size);
	void PackPacket(char* _setptr, int _num, Weapon* _struct, int& _size);
	void PackPacket(char* _setptr, IngamePacket& _struct, int& _size);
	void PackPacket(char* _setptr, int _carSeed, GameInfo* &_gameInfo, vector<WeaponInfo*>& _weaponInfo, int& _size);
	void UnPackPacket(char* _getBuf, int& _num);
	void UnPackPacket(char* _getBuf, IngamePacket& _struct);
	void UnPackPacket(char* _getBuf, Weapon* &_weapon);

	void GetProtocol(PROTOCOL_INGAME& _protocol);		// ���������� ����
	void GetResult(char* _buf, RESULT_INGAME& _result);				// result�� ����
	PROTOCOL_INGAME SetProtocol(STATE_PROTOCOL _state, PROTOCOL_INGAME _protocol, RESULT_INGAME _result);	// �������� + result(�ִٸ�)�� ������

	PROTOCOL_INGAME GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf);	// buf�� Protocol�� ���ÿ� ��� �Լ�
	bool WeaponSelectProcess(C_ClientInfo* _ptr, char* _buf);
	bool LoadingProcess(C_ClientInfo* _ptr, char* _buf);
	bool InitProcess(C_ClientInfo* _ptr, char* _buf);
	bool MoveProcess(C_ClientInfo* _ptr, char* _buf);
	bool GetPosProcess(C_ClientInfo* _ptr, char* _buf);		// ��ġ�� ����ִ� �Լ�
	bool OnFocusProcess(C_ClientInfo* _ptr);				// ��Ŀ�� On���� ó�� �Լ�(�ٸ� �÷��̾� �ΰ��� ���� ������)

public:
	bool CanISelectWeapon(C_ClientInfo* _ptr);	// ���� ����
	bool LoadingSuccess(C_ClientInfo* _ptr);	// �ε� ���� ó��
	bool CanIStart(C_ClientInfo* _ptr);			// ���� �� �ʱ�ȭ
	bool CanIMove(C_ClientInfo* _ptr);			// �̵�
	bool CanIChangeFocus(C_ClientInfo* _ptr);	// ��Ŀ�� ����
	bool LeaveProcess(C_ClientInfo* _ptr, int _playerIndex);		// ���� ���μ���

	void ListSendPacket(list<C_ClientInfo*> _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize, bool _notFocusExcept = true);
	
	static unsigned long __stdcall WeaponSelectTimerThread(void* _arg);	// ������ ���� �ð��� ���� Ÿ�̸� ������
};