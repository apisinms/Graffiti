#pragma once
#include "C_Global.h"
#include "C_SyncCS.h"

class C_ClientInfo;

class InGameManager : public C_SyncCS< InGameManager>
{
#ifdef DEBUG
	static const int WEAPON_SELTIME = 5 + 1;	// 무기 선택 시간(초 단위)
	int numOfPacketSent             = 0;		// 패킷 보낸 횟수
#else
	static const int WEAPON_SELTIME = 30 + 1;	// 무기 선택 시간(초 단위)
#endif

	vector<WeaponInfo*> weaponInfo;		// 보관할 무기 정보 벡터
	vector<GameInfo*> gameInfo;			// 보관할 게임 정보 벡터
	vector<RespawnInfo*> respawnInfo;	// 보관할 리스폰 정보 벡터

	// 53~34
	enum PROTOCOL_INGAME : __int64
	{
		TIMER_PROTOCOL        = ((__int64)0x1 << 53),	// 1초마다 보내는 타이머
		WEAPON_PROTOCOL       = ((__int64)0x1 << 52),	// 서버측:무기선택받아옴, 클라측:무기선택보내옴
		NICKNAME_PROTOCOL     = ((__int64)0x1 << 51),	// 본인의 닉네임을 보내줌
		START_PROTOCOL        = ((__int64)0x1 << 50),	// 게임 시작 프로토콜
		LOADING_PROTOCOL      = ((__int64)0x1 << 49),	// 로딩 여부 프로토콜
		UPDATE_PROTOCOL		  = ((__int64)0x1 << 48),	// 이동 프로토콜
		FOCUS_PROTOCOL        = ((__int64)0x1 << 47),	// 포커스 프로토콜

		DISCONNECT_PROTOCOL   = ((__int64)0x1 << 34),	// 접속 끊김 프로토콜
	};

   // 33~10
	enum RESULT_INGAME : __int64
	{
		// INGAME_PROTOCOL 공통
		INGAME_SUCCESS = ((__int64)0x1 << 33),
		INGAME_FAIL    = ((__int64)0x1 << 32),

		// WEAPON_PROTOCOL 개별
		NOTIFY_WEAPON = ((__int64)0x1 << 31),	// 무기를 알려줌

		// UPDATE_PROTOCOL 개별
		ENTER_SECTOR            = ((__int64)0x1 << 31),		// 섹터 진입
		EXIT_SECTOR             = ((__int64)0x1 << 30),		// 섹터 퇴장
		UPDATE_PLAYER           = ((__int64)0x1 << 29),		// 플레이어 목록 최신화
		FORCE_MOVE              = ((__int64)0x1 << 28),		// 강제 이동
		GET_OTHERPLAYER_STATUS  = ((__int64)0x1 << 27),		// 다른 플레이어 상태 얻기
		BULLET_HIT              = ((__int64)0x1 << 26),		// 총알 맞음
		RESPAWN				    = ((__int64)0x1 << 25),		// 리스폰 요청 수신 및 리스폰 프로토콜 전송
		CAR_SPAWN			    = ((__int64)0x1 << 24),		// 자동차 스폰 
		CAR_HIT					= ((__int64)0x1 << 23),		// 자동차에 치임

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

	void GetProtocol(PROTOCOL_INGAME& _protocol);		// 프로토콜을 얻음
	void GetResult(char* _buf, RESULT_INGAME& _result);				// result를 얻음
	PROTOCOL_INGAME SetProtocol(STATE_PROTOCOL _state, PROTOCOL_INGAME _protocol, RESULT_INGAME _result);	// 프로토콜 + result(있다면)을 설정함

	PROTOCOL_INGAME GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf);	// buf와 Protocol을 동시에 얻는 함수
	bool WeaponSelectProcess(C_ClientInfo* _ptr, char* _buf);
	bool LoadingProcess(C_ClientInfo* _ptr);
	bool InitProcess(C_ClientInfo* _ptr, char* _buf);
	bool UpdateProcess(C_ClientInfo* _ptr, char* _buf);
	bool GetPosProcess(C_ClientInfo* _ptr, char* _buf);		// 위치를 얻어주는 함수
	bool OnFocusProcess(C_ClientInfo* _ptr);		// 포커스 On시의 처리 함수(다른 플레이어 인게임 정보 보내줌)
	bool HitAndRunProcess(C_ClientInfo* _ptr, char* _buf);	// 뺑소니 당함

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
	bool CanISelectWeapon(C_ClientInfo* _ptr);	// 무기 선택
	bool LoadingSuccess(C_ClientInfo* _ptr);	// 로딩 성공 처리
	bool CanIStart(C_ClientInfo* _ptr);			// 시작 시 초기화
	bool CanIUpdate(C_ClientInfo* _ptr);		// 업데이트
	bool CanIChangeFocus(C_ClientInfo* _ptr);	// 포커스 변경
	bool LeaveProcess(C_ClientInfo* _ptr, int _playerIndex);		// 종료 프로세스

	void ListSendPacket(list<C_ClientInfo*> _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize, bool _notFocusExcept);
	void ListSendPacket(vector<C_ClientInfo*> _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize, bool _notFocusExcept);

	static unsigned long __stdcall WeaponSelectTimerThread(void* _arg);	// 아이템 선택 시간을 세는 타이머 쓰레드
	static DWORD WINAPI CarSpawnerThread(LPVOID _arg);

	static void RespawnWaitAndRevive(C_ClientInfo* _player);
};