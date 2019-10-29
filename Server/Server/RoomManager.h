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

// 팀 정보
struct TeamInfo
{
	vector<C_ClientInfo*> teamMemberList;	// 이 팀에 속한 플레이어들
	int teamKillScore;						// 이 팀의 킬 스코어
	int teamCaptureScore;					// 이 팀의 점령 스코어
	int teamTotalScore;						// 이 팀의 킬 + 점령 스코어

	TeamInfo() { memset(this, 0, sizeof(TeamInfo)); }
};


// 방의 정보
struct RoomInfo
{
private:
	//HANDLE weaponTimerHandle;		// 무기 선택 타이머 핸들
	HANDLE InGameTimerHandle;		// 인게임 타이머 핸들
	int weaponTimeElapsedSec;		// 무기 선택 경과 시간(초)
	double carSpawnTimeElapsed;		// 자동차 스폰 경과 시간
	int gameType;					// 이 방의 게임 타입 정보
	int numOfPlayer;				// 현재 방에 있는 플레이어 수
	int maxPlayer;					// 이 방 최대 플레이어 수
	ROOMSTATUS roomStatus;			// 방의 상태
	vector<C_ClientInfo*>players;	// 유저들을 벡터에 저장
	C_Sector* sector;				// 이 방의 섹터 매니저
	TeamInfo* teamInfo;				// 팀 정보
	
public:
	enum GameType
	{
		_2vs2,
		_1vs1,
		
		_MAX_GAMETYPE	// 게임 타입 개수
	};


public:
	RoomInfo(int _gameType, const list<C_ClientInfo*>& _playerList, int _numOfPlayer);
	
	bool LeaveRoom(C_ClientInfo* _player);



	bool IsPlayerListEmpty() { return players.empty(); }


	void SetInGameTimerHandle(HANDLE _handle) { InGameTimerHandle = _handle; }
	HANDLE GetInGameTimerHandle() { return InGameTimerHandle; }

	int GetWeaponTimeElapsed() { return weaponTimeElapsedSec; }
	void SetWeaponTimeElasped(int _elaspedTime) { weaponTimeElapsedSec = _elaspedTime; }

	double GetCarSpawnTimeElapsed() { return carSpawnTimeElapsed; }
	void SetCarSpawnTimeElasped(double _elaspedTime) { carSpawnTimeElapsed = _elaspedTime; }

	int GetGameType() { return gameType; }
	void SetGameType(int _gameType) { gameType = _gameType; }

	void SetNumOfPlayer(int _numOfPlayer) { numOfPlayer = _numOfPlayer; }
	int GetNumOfPlayer() { return numOfPlayer; }

	int GetMaxPlayer() { return maxPlayer; }

	void SetRoomStatus(ROOMSTATUS _roomStatus) { roomStatus = _roomStatus; }
	ROOMSTATUS GetRoomStatus() { return roomStatus; }
	
	vector<C_ClientInfo*> GetPlayers() { return players; }	// 플레이어 벡터를 리턴해주되, 어차피 복사본이 전달되므로 원본은 영향이 없다.

	C_ClientInfo* GetPlayerByIndex(int _idx) { return players[_idx]; }
	
	C_Sector* GetSector() { return sector; }
	
	TeamInfo& GetTeamInfo(int _teamNum) { return teamInfo[_teamNum]; }
};

class RoomManager
{
	//C_List<RoomInfo*>* roomList;
	list<RoomInfo*> roomList;

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
	bool CreateRoom(list<C_ClientInfo*>&_players, int _numOfPlayer);
	bool DeleteRoom(RoomInfo* _room);
	bool CheckLeaveRoom(C_ClientInfo* _ptr);
};