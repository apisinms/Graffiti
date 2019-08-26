#include "stdafx.h"
#include "RoomManager.h"
#include "InGameManager.h"

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

bool RoomManager::CreateRoom(C_ClientInfo* _players[], int _numOfPlayer)
{
	// 플레이어 목록을 토대로 방을 만듦.
	RoomInfo* room = new RoomInfo(_players, _numOfPlayer);

	// 각 플레이어들에게 현재 속한 방을 설정.
	for (int i = 0; i < _numOfPlayer; i++)
		_players[i]->SetRoom(room);

	// 방 리스트에 추가
	bool ret = roomList->Insert(room);
	printf("[방생성] 갯수 : %d\n", roomList->GetCount());

	return ret;
}

bool RoomManager::CheckLeaveRoom(C_ClientInfo* _ptr)
{
	// 속한 방이 있다면
	if (_ptr->GetRoom() != nullptr)
	{
		RoomInfo* room = _ptr->GetRoom();

		// 방을 나갈 수 있는지 검사해서 나갔다면
		if (room->LeaveRoom(_ptr) == true)
		{
			/// 그리고 같은 팀 2명이 모두 나가면 그냥 게임 끝나야함

			// 방금 나간사람이 마지막이었다면 방을 없앰
			if (room->IsPlayerListEmpty() == true)
			{
				roomList->Delete(room);
				printf("[방 소멸]사이즈:%d\n", roomList->GetCount());
			}
		}
	}

	else 
		return false;
}