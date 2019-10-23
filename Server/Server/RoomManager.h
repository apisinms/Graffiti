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
private:
	HANDLE weaponTimerHandle;		// 무기 선택 타이머 핸들
	int numOfPlayer;				// 현재 방에 있는 플레이어 수
	ROOMSTATUS roomStatus;			// 방의 상태
	vector<C_ClientInfo*>players;	// 유저들을 벡터에 저장
	C_Sector* sector;				// 이 방의 섹터 매니저
	HANDLE carSpawnerHandle;		// 자동차 스포너 핸들
	int gameType;					// 이 방의 게임 타입 정보

public:
	enum GameType
	{
		NORMAL,
	};


public:
	RoomInfo(C_ClientInfo** _playerList, int _numOfPlayer);

	bool LeaveRoom(C_ClientInfo* _player);

	// 플레이어 벡터를 리턴해주되, 어차피 복사본이 전달되므로 원본은 영향이 없다.
	vector<C_ClientInfo*> GetPlayers() { return players; }

	C_ClientInfo* GetPlayerByIndex(int _idx) {return players[_idx];}

	bool IsPlayerListEmpty() { return players.empty(); }

	int GetGameType() { return gameType; }
	void SetGameType(int _gameType) { gameType = _gameType; }

	void SetWeaponTimerHandle(HANDLE _handle) { weaponTimerHandle = _handle; }
	HANDLE GetWeaponTimerHandle() { return weaponTimerHandle; }

	void SetNumOfPlayer(int _numOfPlayer) { numOfPlayer = _numOfPlayer; }
	int GetNumOfPlayer() { return numOfPlayer; }

	void SetCarSpawnerHandle(HANDLE _handle) { carSpawnerHandle = _handle; }
	HANDLE GetCarSpawnerHandle() { return carSpawnerHandle; }

	void SetRoomStatus(ROOMSTATUS _roomStatus) { roomStatus = _roomStatus; }
	ROOMSTATUS GetRoomStatus() { return roomStatus; }

	C_Sector* GetSector() { return sector; }
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