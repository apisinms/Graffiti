#pragma once
#include "C_Global.h"
#include "C_Sector.h"

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
	// 여기도 private으로 바꿔야할듯
private:
	list<C_ClientInfo*>::iterator curIter;	// 현재 반복자 위치

	void* weaponTimerHandle;		// 무기 선택 타이머 핸들
	int numOfPlayer;				// 현재 방에 있는 플레이어 수
	ROOMSTATUS roomStatus;			// 방의 상태
	list<C_ClientInfo*>playerList;	// 유저들을 리스트에 저장
	C_Sector* sector;

public:
	RoomInfo(C_ClientInfo** _playerList, int _numOfPlayer);

	bool LeaveRoom(C_ClientInfo* _player);

	bool GetPlayer(C_ClientInfo* &_ptr);	// 리스트에 있는 플레이어를 담아보냄

	bool IsPlayerListEmpty() { return playerList.empty(); }

	int GetPlayerListSize() { return (int)playerList.size(); }

	void SetWeaponTimerHandle(void* _handle) { weaponTimerHandle = _handle; }
	void* GetWeaponTimerHandle() { return weaponTimerHandle; }

	void SetNumOfPlayer(int _numOfPlayer) { numOfPlayer = _numOfPlayer; }
	int GetNumOfPlayer() { return numOfPlayer; }

	void SetRoomStatus(ROOMSTATUS _roomStatus) { roomStatus = _roomStatus; }
	ROOMSTATUS GetRoomStatus() { return roomStatus; }
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
	bool CreateRoom(C_ClientInfo* _players[], int _numOfPlayer);
	bool DeleteRoom(RoomInfo* _room);
	bool CheckLeaveRoom(C_ClientInfo* _ptr);
};