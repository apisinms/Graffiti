#pragma once
#include "C_List.h"

class C_ClientInfo;

// 팀 정보
struct Team
{
	C_ClientInfo* player1;
	C_ClientInfo* player2;

	Team() {}
	Team(C_ClientInfo* _player1, C_ClientInfo* _player2)
	{
		player1 = _player1;
		player2 = _player2;
	}
};

// 방의 정보
struct RoomInfo
{
	Team* team1;
	Team* team2;

	RoomInfo(Team* _team1, Team* _team2)
	{
		team1 = _team1;
		team2 = _team2;
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

};