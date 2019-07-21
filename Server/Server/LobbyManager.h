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
		ENTER_ROOM_PROTOCOL = ((__int64)0x1 << 58),
		CREATE_ROOM_PROTOCOL = ((__int64)0x1 << 57),
		LOGOUT_PROTOCOL = ((__int64)0x1 << 56),
		ROOMLIST_PROTOCOL = ((__int64)0x1 << 55),
	};

	enum RESULT_LOBBY : __int64
	{
		ENTER_ROOM_SUCCESS = ((__int64)0x1 << 53),
		ENTER_ROOM_FAIL = ((__int64)0x1 << 52),

		CREATE_ROOM_SUCCESS = ((__int64)0x1 << 53),
		CREATE_ROOM_FAIL = ((__int64)0x1 << 52),

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

private:
	bool EnterRoomProcess(C_ClientInfo* _ptr, char* _buf);			// 방에 들어가는 처리
	RoomInfo* CheckEnterRoom(int _roomNum, RESULT_LOBBY& _result);	// 방에 들어갈수 있는지 검사

	bool CreateRoomProcess(C_ClientInfo* _ptr, char* _buf);			// 방을 생성하는 처리

	bool RoomListProcess(C_ClientInfo* _ptr, char* _buf);			// 룸 리스트를 얻어서 클라에게 보내주는 처리
	bool GetRoomList(TCHAR* _msg);									// 룸 리스트를 얻어 _msg에 저장

public:
	bool CanIEnterRoom(C_ClientInfo* _ptr);		// 방에 입장 가능한가
	bool CanICreateRoom(C_ClientInfo* _ptr);	// 방을 생성할 수 있는가
	bool CanIGetRoomList(C_ClientInfo* _ptr);	// 방 목록을 얻어올 수 있는가
	bool CanILeaveLobby(C_ClientInfo* _ptr);	// 로비로 갈 수 있는가
	bool CheckLeaveRoom(C_ClientInfo* _ptr);	// 방을 나갈 수 있는가(CHAT_STATE에서 호출하게 됨)

	C_ClientInfo* GetRoomClient(int _roomNum);	// _roomNum 번호에 해당하는 방에 있는 유저들의 정보를 얻어옴

	//POSITION RecvProcess(C_ClientInfo* _ptr);
	//bool SendProcess(C_ClientInfo* _ptr);
};