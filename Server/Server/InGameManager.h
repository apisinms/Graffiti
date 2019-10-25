#pragma once
#include "C_Global.h"
#include "C_SyncCS.h"

class C_ClientInfo;

class InGameManager : public C_SyncCS< InGameManager>
{
#ifdef DEBUG
	static const int WEAPON_SELTIME = 5 + 1;	// ���� ���� �ð�(�� ����)
	int numOfPacketSent             = 0;		// ��Ŷ ���� Ƚ��
#else
	static const int WEAPON_SELTIME = 30 + 1;	// ���� ���� �ð�(�� ����)
#endif

	vector<WeaponInfo*> weaponInfo;		// ������ ���� ���� ����
	vector<GameInfo*> gameInfo;			// ������ ���� ���� ����
	vector<RespawnInfo*> respawnInfo;	// ������ ������ ���� ����

	// 53~34
	enum PROTOCOL_INGAME : __int64
	{
		TIMER_PROTOCOL        = ((__int64)0x1 << 53),	// 1�ʸ��� ������ Ÿ�̸�
		WEAPON_PROTOCOL       = ((__int64)0x1 << 52),	// ������:���⼱�ù޾ƿ�, Ŭ����:���⼱�ú�����
		NICKNAME_PROTOCOL     = ((__int64)0x1 << 51),	// ������ �г����� ������
		START_PROTOCOL        = ((__int64)0x1 << 50),	// ���� ���� ��������
		LOADING_PROTOCOL      = ((__int64)0x1 << 49),	// �ε� ���� ��������
		UPDATE_PROTOCOL		  = ((__int64)0x1 << 48),	// �̵� ��������
		FOCUS_PROTOCOL        = ((__int64)0x1 << 47),	// ��Ŀ�� ��������

		DISCONNECT_PROTOCOL   = ((__int64)0x1 << 34),	// ���� ���� ��������
	};

   // 33~10
	enum RESULT_INGAME : __int64
	{
		// INGAME_PROTOCOL ����
		INGAME_SUCCESS = ((__int64)0x1 << 33),
		INGAME_FAIL    = ((__int64)0x1 << 32),

		// WEAPON_PROTOCOL ����
		NOTIFY_WEAPON = ((__int64)0x1 << 31),	// ���⸦ �˷���

		// UPDATE_PROTOCOL ����
		ENTER_SECTOR            = ((__int64)0x1 << 31),		// ���� ����
		EXIT_SECTOR             = ((__int64)0x1 << 30),		// ���� ����
		UPDATE_PLAYER           = ((__int64)0x1 << 29),		// �÷��̾� ��� �ֽ�ȭ
		FORCE_MOVE              = ((__int64)0x1 << 28),		// ���� �̵�
		GET_OTHERPLAYER_STATUS  = ((__int64)0x1 << 27),		// �ٸ� �÷��̾� ���� ���
		BULLET_HIT              = ((__int64)0x1 << 26),		// �Ѿ� ����
		RESPAWN				    = ((__int64)0x1 << 25),		// ������ ��û ���� �� ������ �������� ����
		CAR_SPAWN			    = ((__int64)0x1 << 24),		// �ڵ��� ���� 
		CAR_HIT					= ((__int64)0x1 << 23),		// �ڵ����� ġ��

		NODATA = ((__int64)0x1 << 10)
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
	void PackPacket(char* _setptr, int _num, TCHAR* _string, int& _size);
	void PackPacket(char* _setptr, int _num, float _posX, float _posZ, int& _size);
	void PackPacket(char* _setptr, int _num, Weapon* _struct, int& _size);
	void PackPacket(char* _setptr, IngamePacket& _struct, int& _size);
	void PackPacket(char* _setptr, GameInfo* &_gameInfo, vector<WeaponInfo*>& _weaponInfo, int& _size);
	void UnPackPacket(char* _getBuf, int& _num);
	void UnPackPacket(char* _getBuf, float& _posX, float& _posZ);
	void UnPackPacket(char* _getBuf, IngamePacket& _struct);
	void UnPackPacket(char* _getBuf, Weapon* &_weapon);

	void GetProtocol(PROTOCOL_INGAME& _protocol);		// ���������� ����
	void GetResult(char* _buf, RESULT_INGAME& _result);				// result�� ����
	PROTOCOL_INGAME SetProtocol(STATE_PROTOCOL _state, PROTOCOL_INGAME _protocol, RESULT_INGAME _result);	// �������� + result(�ִٸ�)�� ������

	PROTOCOL_INGAME GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf);	// buf�� Protocol�� ���ÿ� ��� �Լ�
	bool WeaponSelectProcess(C_ClientInfo* _ptr, char* _buf);
	bool LoadingProcess(C_ClientInfo* _ptr);
	bool InitProcess(C_ClientInfo* _ptr, char* _buf);
	bool UpdateProcess(C_ClientInfo* _ptr, char* _buf);
	bool GetPosProcess(C_ClientInfo* _ptr, char* _buf);		// ��ġ�� ����ִ� �Լ�
	bool OnFocusProcess(C_ClientInfo* _ptr);		// ��Ŀ�� On���� ó�� �Լ�(�ٸ� �÷��̾� �ΰ��� ���� ������)
	bool HitAndRunProcess(C_ClientInfo* _ptr, char* _buf);	// ���Ҵ� ����

	void InitalizePlayersInfo(RoomInfo* _room);

	bool CheckMovement(C_ClientInfo* _ptr, IngamePacket& _recvPacket);
	bool CheckIllegalMovement(C_ClientInfo* _ptr, IngamePacket& _recvPacket);
	void IllegalSectorProcess(C_ClientInfo* _ptr, IngamePacket& _recvPacket, INDEX _beforeIdx);
	void UpdateSectorAndSend(C_ClientInfo* _ptr, IngamePacket& _recvPacket, INDEX& _newIdx);

	bool CheckBullet(C_ClientInfo* _ptr, IngamePacket& _recvPacket);
	bool CheckBulletRange(C_ClientInfo* _shotPlayer, C_ClientInfo* _hitPlayer);
	bool CheckMaxFire(C_ClientInfo* _shotPlayer, int _numOfBullet);
	int GetNumOfBullet(int& _shootCountBit, byte _hitPlayerNum);
	bool BulletHitProcess(C_ClientInfo* _shotPlayer, C_ClientInfo* _hitPlayer, int _numOfBullet);
	void BulletDecrease(C_ClientInfo* _shotPlayer, int _numOfBullet);

	void RefillBulletAndHealth(C_ClientInfo* _respawnPlayer);
public:
	bool CanISelectWeapon(C_ClientInfo* _ptr);	// ���� ����
	bool LoadingSuccess(C_ClientInfo* _ptr);	// �ε� ���� ó��
	bool CanIStart(C_ClientInfo* _ptr);			// ���� �� �ʱ�ȭ
	bool CanIUpdate(C_ClientInfo* _ptr);		// ������Ʈ
	bool CanIChangeFocus(C_ClientInfo* _ptr);	// ��Ŀ�� ����
	bool LeaveProcess(C_ClientInfo* _ptr, int _playerIndex);		// ���� ���μ���

	void ListSendPacket(list<C_ClientInfo*> _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize, bool _notFocusExcept);
	void ListSendPacket(vector<C_ClientInfo*> _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize, bool _notFocusExcept);

	static unsigned long __stdcall WeaponSelectTimerThread(void* _arg);	// ������ ���� �ð��� ���� Ÿ�̸� ������
	static DWORD WINAPI CarSpawnerThread(LPVOID _arg);

	static void RespawnWaitAndRevive(C_ClientInfo* _player);
};