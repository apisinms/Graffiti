#pragma once
#include <list>
#include "C_List.h"
#include "C_Global.h"

class C_ClientInfo;

//// 팀 정보
//struct Team
//{
//	C_ClientInfo* player1;
//	C_ClientInfo* player2;
//
//	Team() {}
//	Team(C_ClientInfo* _player1, C_ClientInfo* _player2)
//	{
//		player1 = _player1;
//		player2 = _player2;
//	}
//};

// 방의 정보
struct RoomInfo
{
	void* timerHandle;

	ROOMSTATUS roomStatus;

	//Team* team1;
	//Team* team2;

	C_ClientInfo* playerList[4];	// 유저들을 포인터 배열로 저장

	/// 나중에 인원수가 더 많아지는 모드가 생기면...
	//C_List<C_ClientInfo*>*playerList;
	//int numOfPlayer;

	RoomInfo(
		C_ClientInfo* _p1, 
		C_ClientInfo* _p2, 
		C_ClientInfo* _p3, 
		C_ClientInfo* _p4)
	{
		timerHandle = NULL;

		roomStatus = ROOMSTATUS::ROOM_NONE;	// 방 생성시 초기 상태는 아무 상태도아님

		playerList[0] = _p1;
		playerList[1] = _p2;
		playerList[2] = _p3;
		playerList[3] = _p4;
	}

};

class RoomManager
{
	C_List<RoomInfo*>* roomList;

private:
	static RoomManager* instance;

	RoomManager() {}
	~RoomManager() {}
public:
	static RoomManager* GetInstance();
	static void Destroy();
	void Init();
	void End();

public:
	bool CreateRoom(C_ClientInfo* _players[]);
	bool CheckLeaveRoom(C_ClientInfo* _ptr);

};