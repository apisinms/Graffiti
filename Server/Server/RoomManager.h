#pragma once
#include "C_Global.h"
#include "C_Sector.h"
#include "C_Timer.h"

#define MAX_BUILDINGS_2VS2	5
#define MAX_BUILDINGS_1VS1	3

class C_ClientInfo;

// �� ����
struct TeamInfo
{
	vector<C_ClientInfo*> teamMemberList;	// �� ���� ���� �÷��̾��
	//int teamKillScore;						// �� ���� ų ���ھ�
	//int teamCaptureScore;					// �� ���� ���� ���ھ�
	//int teamTotalScore;						// �� ���� ų + ���� ���ھ�
	//int teamCaptureNum;						// �� ���� ���� ����

	TeamInfo() {}
};


// ���� ����
struct RoomInfo
{
private:
	HANDLE InGameTimerHandle = nullptr;	// �ΰ��� Ÿ�̸� ������ �ڵ�
	C_Timer* InGameTimer = nullptr;		// �ΰ��� Ÿ�̸�(���� �ð���)
	double InGameTimeSyncElapsed;		// �ΰ��� �ð� ����ȭ ��� �ð�
	double carSpawnTimeElapsed;			// �ڵ��� ���� ��� �ð�
	double captureBonusTimeElapsed;		// ���� ���ʽ� ��� �ð�
	int gameType;						// �� ���� ���� Ÿ�� ����
	int numOfPlayer;					// ���� �濡 �ִ� �÷��̾� ��
	int maxPlayer;						// �� �� �ִ� �÷��̾� ��
	ROOMSTATUS roomStatus;				// ���� ����
	vector<C_ClientInfo*>players;		// �������� ���Ϳ� ����
	C_Sector* sector = nullptr;			// �� ���� ���� �Ŵ���
	TeamInfo* teamInfo = nullptr;		// �� ����
	vector<BuildingInfo*>buildings;		// �ǹ� ����
	
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
			printf("GetRoomStatus() this�� nullptr��\n");
			return ROOMSTATUS::ROOM_NONE;
		}
		
		else
		{
			return roomStatus;
		}
	}
	
	vector<C_ClientInfo*>& GetPlayers() { return players; }	// �÷��̾� ���͸� �������ֵ�, ������ ���纻�� ���޵ǹǷ� ������ ������ ����.

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
	bool DeleteRoom(RoomInfo* _room);		// Ÿ�̸� �ڵ� ��Ҵ��� ���� �����
	bool OnlyDeleteRoom(RoomInfo* _room);	// �׳� �游 �����
	bool CheckLeaveRoom(C_ClientInfo* _ptr);
};