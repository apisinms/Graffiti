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

bool RoomManager::CreateRoom(C_ClientInfo* _players[]/*, int _numOfPlayer 만약 3:3같은 모드가 추가될 경우 인원수를 명시해서 방 생성*/)
{
	// 플레이어를 2명씩 나눠서 팀을 만들고, 이를 방 정보에 추가한다.
	Team* team1 = new Team(_players[0], _players[1]);
	Team* team2 = new Team(_players[2], _players[3]);

	RoomInfo* room = new RoomInfo(team1, team2);

	// 각 플레이어들에게 현재 속한 방을 설정.(4 상수를 numOfPlayer로(만약 모드가있다면))
	for (int i = 0; i < 4; i++)
		_players[i]->SetRoom(room);

	// 방 리스트에 추가
	return roomList->Insert(room);
}

bool RoomManager::CheckLeaveRoom(C_ClientInfo* _ptr)
{
	// 속한 방이 있다면
	if (_ptr->GetRoom() != nullptr)
	{
		//// 이거를 자신을 제외한 모든 플레이어에게 종료 프로토콜 전송
		//if (_ptr->GetRoom()->team1->player1 != _ptr)
		//{
		//	// SendProtocol
		//}

		/// 해당 자리를 null로 만드는 코드.. 도저히 내 스스로 용납할 수가 없는 코드...ㅂㄷㅂㄷ
		if (_ptr->GetRoom()->team1->player1 == _ptr)
			_ptr->GetRoom()->team1->player1 = nullptr;

		else if (_ptr->GetRoom()->team1->player2 == _ptr)
			_ptr->GetRoom()->team1->player2 = nullptr;

		else if (_ptr->GetRoom()->team2->player1 == _ptr)
			_ptr->GetRoom()->team2->player1 = nullptr;

		else if (_ptr->GetRoom()->team2->player2 == _ptr)
			_ptr->GetRoom()->team2->player2 = nullptr;

		_ptr->SetRoom(nullptr);

		return true;
	}

	else 
		return false;
}