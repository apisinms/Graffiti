#pragma once
#include "C_Global.h"

//#define ENTER_ROOM_SUCCESS_MSG		TEXT("방 입장에 성공하였습니다.\n")
//#define ENTER_ROOM_FAIL_MSG			TEXT("방 입장에 실패하였습니다.\n")
//#define CREATE_ROOM_SUCCESS_MSG		TEXT("방을 생성하였습니다.\n")
//#define CREATE_ROOM_FAIL_MSG		TEXT("더이상 방을 생성할 수 없습니다.\n")
//#define ROOMLIST_NOEXIST_MSG		TEXT("개설된 방이 없습니다.\n")
//
//#define MAX_ROOM_SIZE	200
//#define ROOM_TITLE_LEN	255
//#define ROOM_NUM_LEN	20

class C_ClientInfo;

class LobbyManager
{
	// 53~34
	enum PROTOCOL_LOBBY : __int64
	{
		MATCH_PROTOCOL        = ((__int64)0x1 << 53),
		MATCH_CANCEL_PROTOCOL = ((__int64)0x1 << 52),
		GOTO_INGAME_PROTOCOL  = ((__int64)0x1 << 51),      // 인게임 상태로 진입 프로토콜
		LOGOUT_PROTOCOL       = ((__int64)0x1 << 50),

		//LOGOUT_PROTOCOL = ((__int64)0x1 << 56),			// LOGIN 매니저에서 사용되기 때문에
	};

	// 33~24
	enum RESULT_LOBBY : __int64
	{
		LOBBY_SUCCESS = ((__int64)0x1 << 33),		 // 로비에서 성공 처리
		LOBBY_FAIL    = ((__int64)0x1 << 32),        // 로비에서 실패 처리

		NODATA = ((__int64)0x1 << 10)
	};

private:
	LobbyManager() {}
	~LobbyManager() {}
	static LobbyManager* instance;

public:
	void Init();
	void End();
	static LobbyManager* GetInstance();
	static void Destroy();

private:
	void PackPacket(char* _setptr, int _num, int& _size);		// 정수 1개를 Pack하는 함수
	void PackPacket(char* _setptr, TCHAR* _str1, int& _size);	// 문자열 1개를 Pack하는 함수
	void UnPackPacket(char* _getBuf, TCHAR* _str1);				// 문자열 1개를 UnPack하는 함수
	void UnPackPacket(char* _getBuf, int& _gameType);

	void GetProtocol(PROTOCOL_LOBBY& _protocol);								// 프로토콜을 얻음
	PROTOCOL_LOBBY SetProtocol(STATE_PROTOCOL _state, PROTOCOL_LOBBY _protocol, RESULT_LOBBY _result);	// 프로토콜 + result(있다면)을 설정함

	PROTOCOL_LOBBY GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf);	// buf와 Protocol을 동시에 얻는 함수
	void SendPacket_Room(C_ClientInfo* _ptr, char* buf, PROTOCOL_LOBBY protocol);

public:
	bool CanIMatch(C_ClientInfo* _ptr);			// 매칭을 할 수 있는가
	bool CanICancelMatch(C_ClientInfo* _ptr);	// 매칭을 취소 할 수 있는가
	bool CanILeaveLobby(C_ClientInfo* _ptr);	// 로그아웃 할 수 있는가
	bool CanIGotoInGame(C_ClientInfo* _ptr);	// 인게임 상태로 갈 수 있는가
};