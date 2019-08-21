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

bool RoomManager::CreateRoom(C_ClientInfo* _players[]/*, int _numOfPlayer 만약 3:3같은 모드가 추가될 경우 인원수를 명시해서 방 생성*/)
{
	//// 플레이어를 2명씩 나눠서 팀을 만들고, 이를 방 정보에 추가한다.
	//Team* team1 = new Team(_players[0], _players[1]);
	//Team* team2 = new Team(_players[2], _players[3]);

	//RoomInfo* room = new RoomInfo(team1, team2);

	RoomInfo* room = new RoomInfo(
		_players[0],
		_players[1],
		_players[2],
		_players[3]);

	// 각 플레이어들에게 현재 속한 방을 설정.(4 상수를 numOfPlayer로(만약 모드가있다면))
	for (int i = 0; i < 4; i++)
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
		// 방에 리스트를 뒤져서
		for (int i = 0; i < 4; i++)
		{
			// 자신을 찾으면
			if (_ptr == room->playerList[i])
			{
				// 다른 플레이어들에게 자신이 나간다고 알리고
				if (InGameManager::GetInstance()->LeaveProcess(_ptr, (i + 1)) == true)
				{
					// 방 인원수를 감소하고, 자신의 흔적을 지운다.
					room->curNumOfPlayer--;			// 방 인원수 감소
					room->playerList[i] = nullptr;
					_ptr->SetRoom(nullptr);

					break;
				}
			}
		}
		
		/// 그리고 같은 팀 2명이 모두 나가면 그냥 게임 끝나야함
		// 방금 나간사람이 마지막이었다면 방을 없앰
		if (room->curNumOfPlayer == 0)
		{
			roomList->Delete(room);
			printf("[방 소멸]사이즈:%d\n", roomList->GetCount());
		}
	}

	else 
		return false;
}