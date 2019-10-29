#pragma once
#include "C_Global.h"
#include "C_Sector.h"

class C_ClientInfo;

//// �� ����
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

// �� ����
struct TeamInfo
{
	vector<C_ClientInfo*> teamMemberList;	// �� ���� ���� �÷��̾��
	int teamKillScore;						// �� ���� ų ���ھ�
	int teamCaptureScore;					// �� ���� ���� ���ھ�
	int teamTotalScore;						// �� ���� ų + ���� ���ھ�

	TeamInfo() { memset(this, 0, sizeof(TeamInfo)); }
};


// ���� ����
struct RoomInfo
{
private:
	//HANDLE weaponTimerHandle;		// ���� ���� Ÿ�̸� �ڵ�
	HANDLE InGameTimerHandle;		// �ΰ��� Ÿ�̸� �ڵ�
	int weaponTimeElapsedSec;		// ���� ���� ��� �ð�(��)
	double carSpawnTimeElapsed;		// �ڵ��� ���� ��� �ð�
	int gameType;					// �� ���� ���� Ÿ�� ����
	int numOfPlayer;				// ���� �濡 �ִ� �÷��̾� ��
	int maxPlayer;					// �� �� �ִ� �÷��̾� ��
	ROOMSTATUS roomStatus;			// ���� ����
	vector<C_ClientInfo*>players;	// �������� ���Ϳ� ����
	C_Sector* sector;				// �� ���� ���� �Ŵ���
	TeamInfo* teamInfo;				// �� ����
	
public:
	enum GameType
	{
		_2vs2,
		_1vs1,
		
		_MAX_GAMETYPE	// ���� Ÿ�� ����
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
	
	vector<C_ClientInfo*> GetPlayers() { return players; }	// �÷��̾� ���͸� �������ֵ�, ������ ���纻�� ���޵ǹǷ� ������ ������ ����.

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