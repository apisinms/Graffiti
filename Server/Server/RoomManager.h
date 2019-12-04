#pragma once
#include "C_Global.h"
#include "C_Sector.h"
#include "C_Timer.h"

#define MAX_BUILDINGS_2VS2	5
#define MAX_BUILDINGS_1VS1	3

class C_ClientInfo;

// 팀 정보
struct TeamInfo
{
	vector<C_ClientInfo*> teamMemberList;	// 이 팀에 속한 플레이어들
	//int teamKillScore;						// 이 팀의 킬 스코어
	//int teamCaptureScore;					// 이 팀의 점령 스코어
	//int teamTotalScore;						// 이 팀의 킬 + 점령 스코어
	//int teamCaptureNum;						// 이 팀의 점령 개수

	TeamInfo() {}
};


// 방의 정보
struct RoomInfo
{
private:
	HANDLE InGameTimerHandle = nullptr;	// 인게임 타이머 쓰레드 핸들
	C_Timer* InGameTimer = nullptr;		// 인게임 타이머(게임 시간만)
	double InGameTimeSyncElapsed;		// 인게임 시간 동기화 경과 시간
	double carSpawnTimeElapsed;			// 자동차 스폰 경과 시간
	double captureBonusTimeElapsed;		// 점령 보너스 경과 시간
	int gameType;						// 이 방의 게임 타입 정보
	int numOfPlayer;					// 현재 방에 있는 플레이어 수
	int maxPlayer;						// 이 방 최대 플레이어 수
	ROOMSTATUS roomStatus;				// 방의 상태
	vector<C_ClientInfo*>players;		// 유저들을 벡터에 저장
	C_Sector* sector = nullptr;			// 이 방의 섹터 매니저
	TeamInfo* teamInfo = nullptr;		// 팀 정보
	vector<BuildingInfo*>buildings;		// 건물 정보
	
public:
	RoomInfo(int _gameType, const list<C_ClientInfo*>& _playerList, int _numOfPlayer);
	
	bool LeaveRoom(C_ClientInfo* _player);
	bool IsPlayerListEmpty() { return players.empty(); }

	void SetInGameTimerHandle(HANDLE _handle) { InGameTimerHandle = _handle; }
	HANDLE GetInGameTimerHandle() { return InGameTimerHandle; }

	C_Timer* GetInGameTimer() { return InGameTimer; }
	void SetInGameTimer(C_Timer* _timer) 
	{
		if (InGameTimer != nullptr)
		{
			delete InGameTimer;
		}
		InGameTimer = _timer;
	}

	double GetSyncTimeElapsed() { return InGameTimeSyncElapsed; }
	void SetSyncTimeElasped(double _elaspedTime) { InGameTimeSyncElapsed = _elaspedTime; }

	double GetCarSpawnTimeElapsed() { return carSpawnTimeElapsed; }
	void SetCarSpawnTimeElasped(double _elaspedTime) { carSpawnTimeElapsed = _elaspedTime; }

	double GetCaptureBonusTimeElapsed() { return captureBonusTimeElapsed; }
	void SetCaptureBonusTimeElasped(double _elaspedTime) { captureBonusTimeElapsed = _elaspedTime; }

	int GetGameType() { return gameType; }
	void SetGameType(int _gameType) { gameType = _gameType; }

	void SetNumOfPlayer(int _numOfPlayer) { numOfPlayer = _numOfPlayer; }
	int GetNumOfPlayer() { return numOfPlayer; }

	int GetMaxPlayer() { return maxPlayer; }

	void SetRoomStatus(ROOMSTATUS _roomStatus) { roomStatus = _roomStatus; }
	ROOMSTATUS GetRoomStatus() 
	{ 
		if (this == nullptr)
		{
			printf("GetRoomStatus() this가 nullptr임\n");
			return ROOMSTATUS::ROOM_NONE;
		}
		
		else
		{
			return roomStatus;
		}
	}
	
	vector<C_ClientInfo*>& GetPlayers() { return players; }	// 플레이어 벡터를 리턴해주되, 어차피 복사본이 전달되므로 원본은 영향이 없다.

	C_ClientInfo* GetPlayerByIndex(int _idx) { return players[_idx]; }
	C_ClientInfo* GetPlayerByPlayerNum(int _playerNum)
	{ 
		for (size_t i = 0; i < players.size(); i++)
		{
			if (players[i]->GetPlayerInfo()->GetPlayerNum() == _playerNum)
			{
				return players[i];
			}
		}
		return nullptr;
	}
	
	C_Sector* GetSector() { return sector; }
	void SetSector(C_Sector* _sector) 
	{ 
		if (sector != nullptr)
		{
			delete sector;
		}
		sector = _sector; 
	}
	
	TeamInfo& GetTeamInfo(int _teamNum) { return teamInfo[_teamNum]; }
	void DeleteTeam()
	{
		if (teamInfo != nullptr)
		{
			delete[] teamInfo;
			teamInfo = nullptr;
		}
	}

	vector<BuildingInfo*>& GetBuildings() { return buildings; }
};

class RoomManager : public C_SyncCS<RoomManager>
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
	bool DeleteRoom(RoomInfo* _room);		// 타이머 핸들 살았는지 보고 지우기
	bool OnlyDeleteRoom(RoomInfo* _room);	// 그냥 방만 지우기
	bool CheckLeaveRoom(C_ClientInfo* _ptr);
};