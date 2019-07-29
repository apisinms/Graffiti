#include "RoomManager.h"

#include "C_ClientInfo.h"

RoomManager* RoomManager::instance;

RoomManager* RoomManager::GetInstance()
{
	// �ν��Ͻ��� ���ٸ� �ν��Ͻ��� �����ϰ� �����Ѵ�.
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
	// �÷��̾ 2�� ������ ���� �����, �̸� �� ������ �߰��Ѵ�.
	Team* team1 = new Team(_player1, _player2);
	Team* team2 = new Team(_player3, _myPlayer);

	RoomInfo* room = new RoomInfo(team1, team2);

	// �� �÷��̾�鿡�� ���� ���� ���� ����.
	_player1->SetRoom(room);
	_player2->SetRoom(room);
	_player3->SetRoom(room);
	_myPlayer->SetRoom(room);

	// �� ����Ʈ�� �߰�
	return roomList->Insert(room);
}