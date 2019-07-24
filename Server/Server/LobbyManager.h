#pragma once
#include "C_List.h"
#include "C_Global.h"
#include <tchar.h>

#define ENTER_ROOM_SUCCESS_MSG		TEXT("방 입장에 성공하였습니다.\n")
#define ENTER_ROOM_FAIL_MSG			TEXT("방 입장에 실패하였습니다.\n")
#define CREATE_ROOM_SUCCESS_MSG		TEXT("방을 생성하였습니다.\n")
#define CREATE_ROOM_FAIL_MSG		TEXT("더이상 방을 생성할 수 없습니다.\n")
#define ROOMLIST_NOEXIST_MSG		TEXT("개설된 방이 없습니다.\n")

#define MAX_ROOM_SIZE	200
#define ROOM_TITLE_LEN	255
#define ROOM_NUM_LEN	20

class C_ClientInfo;


struct RoomInfo
{
	static int incRoomNum;

	C_List<C_ClientInfo*>* userList;
	int roomNum;
	TCHAR roomTitle[ROOM_TITLE_LEN];

	// 생성자
	RoomInfo(C_ClientInfo* _user, TCHAR *_roomTitle)
	{
		userList = new C_List<C_ClientInfo*>();
		userList->Insert(_user);
		roomNum = ++incRoomNum;
		_tcscpy_s(roomTitle, ROOM_TITLE_LEN, _roomTitle);
	}

	~RoomInfo()
	{
		delete userList;
	}
};

class LobbyManager
{
	enum PROTOCOL_LOBBY : __int64
	{
		MATCHING_PROTOCOL = ((__int64)0x1 << 58),		// 매칭 프로토콜

		LOGOUT_PROTOCOL = ((__int64)0x1 << 56),			// LOGIN 매니저에서 사용되기 때문에
	};

	enum RESULT_LOBBY : __int64
	{
		MATCHING_SUCCESS = ((__int64)0x1 << 53),
		MATCHING_FAIL = ((__int64)0x1 << 52),

		NODATA = ((__int64)0x1 << 49)
	};


private:
	LobbyManager() {}
	~LobbyManager() {}
	static LobbyManager* instance;

	C_List<RoomInfo*>* roomList;

public:
	void Init();
	void End();
	static LobbyManager* GetInstance();
	static void Destroy();

private:
	void PackPacket(char* _setptr, TCHAR* _str1, int& _size);	// 문자열 1개를 Pack하는 함수
	void UnPackPacket(char* _getBuf, TCHAR* _str1);				// 문자열 1개를 UnPack하는 함수

	void GetProtocol(PROTOCOL_LOBBY& _protocol);								// 프로토콜을 얻음
	PROTOCOL_LOBBY SetProtocol(STATE_PROTOCOL _state, PROTOCOL_LOBBY _protocol, RESULT_LOBBY _result);	// 프로토콜 + result(있다면)을 설정함

	PROTOCOL_LOBBY GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf);	// buf와 Protocol을 동시에 얻는 함수

public:
	bool CanIMatch(C_ClientInfo* _ptr);			// 매칭을 할 수 있는가
};