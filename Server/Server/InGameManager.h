#pragma once
#include "C_Global.h"
#include "C_SyncCS.h"

#define MEMBER_PER_TEAM	2

#define TIMER_INTERVAL	50	// Ÿ�̸� ����
#define TIMER_INTERVAL_TIMES_MILLISEC	(TIMER_INTERVAL * 0.001)	// Ÿ�̸� ���� * �и��ʴ���

#define WEAPON_SELTIME			5 + 3			// ���� ���� �ð�(3�� ������ �ε� + Ŭ�� Invoke �ð� ���)
#define CAR_SPAWN_TIME_2vs2_SEC	5				// �� ���� �ֱ�(2vs2)
#define CAR_SPAWN_TIME_1vs1_SEC	7 				// �� ���� �ֱ�(1vs1)
#define CAPTURE_BONUS_TIME_SEC	10 				// ���� ���ʽ� �ð� �ֱ�
#define MAX_LOADING_WAIT_TIME	10				// �ִ� �ε� ���ð�
#define READY_TIME				3				// ���� ���� �ð�
#define GAME_END_WAIT_TIME		3				// ���� ������ ��� ��ٷȴٰ� ���ھ��� �� �ð�
#define TIME_SYNC_INTERVAL		5				// Ŭ�� �ð� ����ȭ �ֱ�

class C_ClientInfo;

class InGameManager : public C_SyncCS<InGameManager>
{
	vector<WeaponInfo*> weaponInfo;			// ������ ���� ���� ����
	vector<GameInfo*> gameInfo;				// ������ ���� ���� ����
	vector<LocationInfo*> locationInfo;		// ������ ��ġ ���� ����

	// 53~34
	enum PROTOCOL_INGAME : __int64
	{
		TIMER_PROTOCOL          = ((__int64)0x1 << 53),	// 1�ʸ��� ������ Ÿ�̸�
		WEAPON_PROTOCOL         = ((__int64)0x1 << 52),	// ������:���⼱�ù޾ƿ�, Ŭ����:���⼱�ú�����
		INFO_PROTOCOL           = ((__int64)0x1 << 51),	// ������ Ŭ��� ����
		START_PROTOCOL          = ((__int64)0x1 << 50),	// ���� ���� ��������
		LOADING_PROTOCOL        = ((__int64)0x1 << 49),	// �ε� ���� ��������
		UPDATE_PROTOCOL		    = ((__int64)0x1 << 48),	// �̵� ��������
		FOCUS_PROTOCOL          = ((__int64)0x1 << 47),	// ��Ŀ�� ��������
		GOTO_LOBBY_PROTOCOL     = ((__int64)0x1 << 46),	// �κ�� ����
		CAPTURE_PROTOCOL        = ((__int64)0x1 << 45),	// ����
		ITEM_PROTOCOL           = ((__int64)0x1 << 44),	// ������ ��������
		GAME_END_PROTOCOL		= ((__int64)0x1 << 43),	// ���� ���� ��������(���ھ� ���������)

		DISCONNECT_PROTOCOL   = ((__int64)0x1 << 34),	// ���� ���� ��������
	};

   // 33~10
	enum RESULT_INGAME : __int64
	{
		// INGAME_PROTOCOL ����
		INGAME_SUCCESS = ((__int64)0x1 << 33),
		INGAME_FAIL = ((__int64)0x1 << 32),

		// TIMER_PROTOCOL ����
		WEAPON		= ((__int64)0x1 << 31),	// ���� ���� ���� ��
		INGAME_SYNC = ((__int64)0x1 << 30),	// �ΰ��� �ð� ��ũ ���߱��

		// WEAPON_PROTOCOL ����
		NOTIFY_WEAPON = ((__int64)0x1 << 31),	// ���⸦ �˷���

		// INFO_PROTOCOL ����
		NICKNAME = ((__int64)0x1 << 31),		// �г��� �˷���

		// START_PROTOCOL ����
		INIT_INFO   = ((__int64)0x1 << 31),
		READY_START = ((__int64)0x1 << 30),
		READY_END  = ((__int64)0x1 << 29),

		// UPDATE_PROTOCOL ����
		ENTER_SECTOR           = ((__int64)0x1 << 31),		// ���� ����
		EXIT_SECTOR            = ((__int64)0x1 << 30),		// ���� ����
		UPDATE_PLAYER          = ((__int64)0x1 << 29),		// �÷��̾� ��� �ֽ�ȭ
		FORCE_MOVE             = ((__int64)0x1 << 28),		// ���� �̵�
		GET_OTHERPLAYER_STATUS = ((__int64)0x1 << 27),		// �ٸ� �÷��̾� ���� ���
		BULLET_HIT             = ((__int64)0x1 << 26),		// �Ѿ� ����
		RESPAWN                = ((__int64)0x1 << 25),		// ������ ��û ���� �� ������ �������� ����
		CAR_SPAWN              = ((__int64)0x1 << 24),		// �ڵ��� ���� 
		CAR_HIT                = ((__int64)0x1 << 23),		// �ڵ����� ġ��
		KILL                   = ((__int64)0x1 << 22),		// �÷��̾����� ����

		// DISCONNECT_PROTOCOL ����
		WEAPON_SEL           = ((__int64)0x1 << 31),
		BEFORE_LOAD          = ((__int64)0x1 << 30),
		MAX_LOADING_TIMEWAIT = ((__int64)0x1 << 29),
		ABORT                = ((__int64)0x1 << 28),

		// CAPTRUE_PROTOCOL ����
		BONUS = ((__int64)0x1 << 31),

		// GAME_END_PROTOCOL ����
		GAME_END_TEXT_SHOW = ((__int64)0x1 << 31),
		SCORE_SHOW = ((__int64)0x1 << 30),


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
	void PackPacket(char * _setptr, double _time, int & _size);
	void PackPacket(char* _setptr, int _num, TCHAR* _string, int& _size);
	void PackPacket(char* _setptr, int _num1, int _num2, int& _size);
	void PackPacket(char* _setptr, int _num, float _posX, float _posZ, int& _size);
	void PackPacket(char* _setptr, int _num, Weapon* _struct, int& _size);
	void PackPacket(char* _setptr, IngamePacket& _struct, int& _size);
	void PackPacket(char* _setptr, IngamePacket& _struct, int _code, int& _size);
	void PackPacket(char* _setptr, GameInfo* &_gameInfo, vector<WeaponInfo*>& _weaponInfo, int& _size);
	void PackPacket(char* _setptr, RoomInfo* _room, int& _size);	// �濡 �ִ� �÷��̾���� ���ھ �����ش�.
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
	bool CaptureProcess(C_ClientInfo* _ptr, char* _buf);
	bool ItemGetProcess(C_ClientInfo* _ptr, char* _buf);	// ������ ����
	
	void InitalizePlayersInfo(RoomInfo* _room);

	bool CheckMovement(C_ClientInfo* _ptr, IngamePacket& _recvPacket);
	bool CheckIllegalMovement(C_ClientInfo* _ptr, IngamePacket& _recvPacket);
	void IllegalSectorProcess(C_ClientInfo* _ptr, IngamePacket& _recvPacket, INDEX _beforeIdx);
	void UpdateSectorAndSend(C_ClientInfo* _ptr, IngamePacket& _recvPacket, INDEX& _newIdx);

	bool CheckBullet(C_ClientInfo* _ptr, IngamePacket& _recvPacket);
	bool CheckBulletRange(C_ClientInfo* _shotPlayer, C_ClientInfo* _hitPlayer);
	bool CheckMaxFire(C_ClientInfo* _shotPlayer, int _numOfBullet);
	int GetNumOfBullet(int _shootCountBit, byte _hitPlayerNum);
	bool BulletHitProcess(C_ClientInfo* _shotPlayer, C_ClientInfo* _hitPlayer, int _numOfBullet, float _nowHealth);
	void BulletDecrease(C_ClientInfo* _shotPlayer, int _numOfBullet);
	bool CheckBulletHitAndGetHitPlayers(C_ClientInfo* _ptr, IngamePacket& _recvPacket, vector<C_ClientInfo*>& _hitPlayers);
	void BulletHitSend(C_ClientInfo* _shotPlayer, const vector<C_ClientInfo*>& _hitPlayers);
	bool CheckSameTeam(C_ClientInfo* _player, int _otherPlayerNum);

	void RefillBulletAndHealth(C_ClientInfo* _respawnPlayer);
	void RefillBullet(C_ClientInfo* _player);
	void RefillHealth(C_ClientInfo* _player);
	void ChangeHealthAmount(C_ClientInfo* _player, float _amount);

	void Kill(C_ClientInfo* _shotPlayer, C_ClientInfo* _hitPlayer);

	void AddCaptureBonus(RoomInfo* _room, int& _team1CaptureBonus, int& _team2CaptureBonus);	// ���� ���ʽ�

	void ListSendPacket(list<C_ClientInfo*>& _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize, bool _notFocusExcept);
	void ListSendPacket(vector<C_ClientInfo*>& _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize, bool _notFocusExcept);

public:
	bool IngameProtocolChecker(C_ClientInfo* _ptr);		// �ΰ��� �������� Ȯ��
	bool CanIGotoLobby(C_ClientInfo* _ptr);		// �κ�� �� �� �ִ���
	
	bool LeaveProcess(C_ClientInfo* _ptr);		// ���� ���μ���

public:
	static DWORD WINAPI InGameTimerThread(LPVOID _arg);

	void WeaponTimerChecker(RoomInfo* _room);
	void LoadingTimeChecker(RoomInfo* _room);
	void ReadyTimeChecker(RoomInfo* _room);
	void RespawnChecker(RoomInfo* _room);
	void CarSpawnChecker(RoomInfo* _room);
	void InGameTimeSync(RoomInfo* _room);
	void CaptureBonusTimeChecker(RoomInfo* _room);
	void GameEndTimeChecker(RoomInfo* _room);

public:
	GameInfo* GetGameInfo(int _gameType) { return gameInfo[_gameType]; }
	bool GameEndProcess(RoomInfo* _room, RESULT_INGAME _result = RESULT_INGAME::GAME_END_TEXT_SHOW);	// ���� ���� ó��

public:
	void UpdatePlayerList(C_ClientInfo* _ptr);
	void UpdatePlayerList(vector<C_ClientInfo*>& _players, C_ClientInfo* _exceptPlayer);

	void ResetPlayerInfo(C_ClientInfo* _player);
};