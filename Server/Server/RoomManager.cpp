#include "stdafx.h"
#include "RoomManager.h"
#include "InGameManager.h"

#include "C_ClientInfo.h"

////////////////////////// RoomInfo 구조체 //////////////////////////////
RoomInfo::RoomInfo(int _gameType, const list<C_ClientInfo*>& _playerList, int _numOfPlayer)
{
	weaponTimerHandle = NULL;

	roomStatus = ROOMSTATUS::ROOM_NONE;	// 방 생성시 초기 상태는 아무 상태도아님

	gameType = _gameType;
	maxPlayer = numOfPlayer = _numOfPlayer;
	
	switch (gameType)
	{
		case GameType::_2vs2:
		{
			teamInfo = new TeamInfo[2];

			// 2명씩 끊어서 저장
			int teamIdx = 0;
			int i = 0;
			for (auto iter = _playerList.begin(); iter != _playerList.end(); ++iter)
			{
				teamInfo[teamIdx].teamMemberList.emplace_back(*iter);

				if ((++i) % 2 == 0)
					teamIdx++;
			}
		}
		break;

		case GameType::_1vs1:
		{
			teamInfo = new TeamInfo[2];	// 나중에 3개 이상 팀이 만약 생긴다면..(확장성 고려)

			// 각 팀에 1명씩 저장
			int i = 0;
			for (auto iter = _playerList.begin(); iter != _playerList.end(); ++iter)
			{
				teamInfo[i++].teamMemberList.emplace_back(*iter);
			}
		}
		break;
	}

	// 방 플레이어 리스트에 추가
	for (auto iter = _playerList.begin(); iter != _playerList.end(); ++iter)
	{
		players.emplace_back(*iter);
	}

	sector = new C_Sector();	// 이 방의 섹터관리자 생성
}

bool RoomInfo::LeaveRoom(C_ClientInfo* _player)
{	
	// 내가 나간다는 것을 같은 방에 있는 플레이어들에게 보내줌
	if (InGameManager::GetInstance()->LeaveProcess(_player, _player->GetPlayerInfo()->GetIngamePacket()->playerNum) == true)
	{
		// 방 인원수를 감소하고, 자신의 흔적을 지운다.
		numOfPlayer--;					// 방 인원수 감소
		players.erase(remove(players.begin(), players.end(), _player), players.end());		// 방의 플레이어 리스트에서 제거
		
		_player->SetRoom(nullptr);		// 플레이어의 방을 null로 설정

		return true;
	}

	return false;	// 못찾은 경우
}

//C_ClientInfo* RoomInfo::GetPlayerByNum(int _playerNum)
//{
//}

////////////////////////// RoomManager 클래스 //////////////////////////
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

bool RoomManager::CreateRoom(list<C_ClientInfo*>_players, int _numOfPlayer)
{
	int gameType = (_players.front())->GetGameType();	// 게임 타입!
	
	// 플레이어 목록을 토대로 방을 만듦.
	RoomInfo* room = new RoomInfo(
		gameType,	
		_players, _numOfPlayer);

	// 각 플레이어들에게 현재 속한 방을 설정.
	for (auto iter = _players.begin(); iter != _players.end(); ++iter)
	{
		((C_ClientInfo*)(*iter))->SetRoom(room);
	}

	// 방 리스트에 추가
	bool ret = roomList->Insert(room);
	switch ((RoomInfo::GameType)gameType)
	{
		case RoomInfo::GameType::_2vs2:
		{
			printf("[2:2][방생성] 총 갯수 : %d\n", roomList->GetCount());
		}
		break;

		case RoomInfo::GameType::_1vs1:
		{
			printf("[1:1][방생성] 총 갯수 : %d\n", roomList->GetCount());
		}
		break;
	}

	return ret;
}

bool RoomManager::DeleteRoom(RoomInfo* _room)
{
	if (roomList->Delete(_room) == true)
	{		
		printf("[방 소멸]사이즈:%d\n", roomList->GetCount());
		return true;
	}

	return false;
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

			return true;
		}
	}

	return false;
}