#include "RoomManager.h"

#include "C_ClientInfo.h"

RoomManager* RoomManager::instance;

RoomManager* RoomManager::GetInstance()
{
	// 인스턴스가 없다면 인스턴스를 생성하고 리턴한다.
	if (instance == nullptr)
		instance = new RoomManager();

	return instance;
}

void RoomManager::Init()
{
	roomList = new C_List<RoomInfo*>();
}
void RoomManager::End()
{
	delete roomList;
}

void RoomManager::Destroy()
{
	delete instance;
}

bool RoomManager::CreateRoom(
	C_ClientInfo* _player1, 
	C_ClientInfo* _player2, 
	C_ClientInfo* _player3, 
	C_ClientInfo* _myPlayer)
{
	// 플레이어를 2명씩 나눠서 팀을 만들고, 이를 방 정보에 추가한다.
	Team* team1 = new Team(_player1, _player2);
	Team* team2 = new Team(_player3, _myPlayer);

	RoomInfo* room = new RoomInfo(team1, team2);

	// 각 플레이어들에게 현재 속한 방을 설정.
	_player1->SetRoom(room);
	_player2->SetRoom(room);
	_player3->SetRoom(room);
	_myPlayer->SetRoom(room);

	// 방 리스트에 추가
	return roomList->Insert(room);
}