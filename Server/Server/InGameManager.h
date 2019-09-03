#pragma once
#include "C_Global.h"

class C_ClientInfo;

class InGameManager
{
#ifdef DEBUG
	static const int WEAPON_SELTIME = 1 + 1;	// 무기 선택 시간(초 단위)
#else
	static const int WEAPON_SELTIME = 30 + 1;	// 무기 선택 시간(초 단위)
#endif

	// 53~34
	enum PROTOCOL_INGAME : __int64
	{
		TIMER_PROTOCOL        = ((__int64)0x1 << 53),	// 1초마다 보내는 타이머
		WEAPON_PROTOCOL       = ((__int64)0x1 << 52),	// 서버측:무기선택받아옴, 클라측:무기선택보내옴
		START_PROTOCOL        = ((__int64)0x1 << 51),	// 게임 시작 프로토콜
		MOVE_PROTOCOL		  = ((__int64)0x1 << 50),	// 이동 프로토콜
		DISCONNECT_PROTOCOL   = ((__int64)0x1 << 49),	// 접속 끊김 프로토콜
	};

	// 33~24
	enum RESULT_INGAME : __int64
	{
		INGAME_SUCCESS = ((__int64)0x1 << 33),
		INGAME_FAIL    = ((__int64)0x1 << 32),

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
	void PackPacket(char* _setptr, TCHAR* _str1, int& _size);				// 문자열 1개를 Pack하는 함수
	void PackPacket(char* _setptr, PositionPacket& _struct, unsigned char _playerBit, int& _size);
	void UnPackPacket(char* _getBuf, PositionPacket& _struct);
	void UnPackPacket(char* _getBuf, Weapon* &_weapon);

	void GetProtocol(PROTOCOL_INGAME& _protocol);								// 프로토콜을 얻음
	PROTOCOL_INGAME SetProtocol(STATE_PROTOCOL _state, PROTOCOL_INGAME _protocol, RESULT_INGAME _result);	// 프로토콜 + result(있다면)을 설정함

	PROTOCOL_INGAME GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf);	// buf와 Protocol을 동시에 얻는 함수
	bool WeaponSelectProcess(C_ClientInfo* _ptr, char* _buf);
	bool InitProcess(C_ClientInfo* _ptr, char* _buf);
	bool MoveProcess(C_ClientInfo* _ptr, char* _buf);

public:
	bool CanISelectWeapon(C_ClientInfo* _ptr);	// 무기 선택
	bool CanIStart(C_ClientInfo* _ptr);			// 시작 시 초기화
	bool CanIMove(C_ClientInfo* _ptr);			// 이동
	bool LeaveProcess(C_ClientInfo* _ptr, int _playerIndex);		// 종료 프로세스

	static unsigned long __stdcall TimerThread(void* _arg);	// 아이템 선택 시간을 세는 타이머 쓰레드
};